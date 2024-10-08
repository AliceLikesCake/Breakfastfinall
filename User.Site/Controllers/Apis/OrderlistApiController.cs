﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using User.Site.Models.EFModels;
using User.Site.Models.ViewModels;

namespace User.Site.Controllers.Apis
{
    public class OrderlistApiController : ApiController
    {
        [HttpGet]
        [Route("api/orders/all")]
        public IHttpActionResult Get(int memberId)
        {
            var db = new AppDbContext();

            // 查询指定订单 ID 的订单详情
            var order = db.Orders
                .Where(o => o.MemberID == memberId)
                .Select(o => new OrderVm
                {
                    OrderID = o.Id,
                    OrderTime = o.OrderTime,
                    PickupTime = o.TakeTime,
                    TotalAmount = o.FinalTotal,
                    OrderStatus = o.OrderStatus,

                    // PointsEarned 从 PointDetails 中提取
                    PointsEarned = db.PointDetails
                        .Where(pd => pd.OrderId == o.Id)
                        .Select(pd => pd.Earned)
                        .FirstOrDefault(),

                    PointsUsed = o.PointsUsed,

                    // 查询 OrderDetails 的数据
                    Items = db.OrderDetails
                        .Where(od => od.OrderID == o.Id)
                        .Select(od => new OrderItemVm
                        {
                            Name = od.Product.Name,

                            // Description 从 OrderAddOnDetails 和 ProductAddOnDetails 提取
                            Description = db.OrderAddOnDetails
                                .Where(aod => aod.OrderDetailID == od.Id)
                                .Select(aod => db.ProductAddOnDetails
                                    .Where(pad => pad.Id == aod.ProductAddOnDetailsID)
                                    .Select(pad => pad.AddOnOptionName)
                                    .FirstOrDefault())
                                .FirstOrDefault(),

                            // 从 Products 表提取 Image
                            Image = db.Products
                                .Where(p => p.Name == od.Product.Name)
                                .Select(p => p.Image)
                                .FirstOrDefault(),

                            // 計算 Price：Subtotal + 加選項的總價格 (AddOnQuantity * AddOnOptionPrice)
                            Price = od.SubTotal + (db.OrderAddOnDetails
                                .Where(aod => aod.OrderDetailID == od.Id)
                                .Select(aod => (decimal?)(aod.AddOnQuantity * aod.AddOnOptionPrice))
                                .Sum() ?? 0), // 處理可能為 null 的情況
                            Quantity = od.ProductQuantity
                        }).ToList()
                });

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }



        [HttpPost]
        [Route("api/orders/cancel/{orderId}")]
        public IHttpActionResult CancelOrder(int orderId)
        {
            using (var db = new AppDbContext())
            {
                // 開啟交易來保證操作的原子性
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 找到對應的訂單
                        var order = db.Orders.FirstOrDefault(o => o.Id == orderId);

                        if (order == null)
                        {
                            // 如果找不到訂單，返回 404 Not Found
                            return Content(HttpStatusCode.NotFound, new { message = "訂單未找到" });
                        }

                        // 檢查訂單狀態是否允許取消
                        if (order.OrderStatus != 1)
                        {
                            // 如果不是 "未取餐" 狀態，則不能取消訂單，返回 400 Bad Request
                            return Content(HttpStatusCode.BadRequest, new { message = "訂單無法取消" });
                        }

                        // 查找會員資料
                        var member = db.Members.FirstOrDefault(m => m.Id == order.MemberID);

                        if (member == null)
                        {
                            // 如果找不到會員，返回 404 Not Found
                            return Content(HttpStatusCode.NotFound, new { message = "會員未找到" });
                        }

                        // 查找 PointDetail 中該訂單所對應的點數記錄
                        var pointDetail = db.PointDetails.FirstOrDefault(pd => pd.OrderId == order.Id);

                        if (pointDetail != null)
                        {
                            // 回收該訂單獲得的點數
                            if (pointDetail.Earned > 0)
                            {
                                member.Points -= pointDetail.Earned;

                                // 確保會員點數不會變成負數
                                if (member.Points < 0)
                                {
                                    member.Points = 0;
                                }

                                // 清除該訂單的獲得點數
                                pointDetail.Earned = 0;
                            }

                            // 檢查訂單是否使用了點數，退還使用的點數並更新 PointDetail
                            if (order.PointsUsed > 0)
                            {
                                member.Points += order.PointsUsed;
                                // 重設訂單的使用點數為 0，表示點數已退還
                                order.PointsUsed = 0;

                                // 同步更新 PointDetail 中的已使用點數為 0
                                pointDetail.Used = 0;
                            }

                            // 同步更新剩餘點數為會員最新的點數
                            pointDetail.Remaining = member.Points;
                        }

                        // 更新訂單狀態為 "已取消" (3)
                        order.OrderStatus = 3;

                        // 儲存變更到資料庫
                        db.SaveChanges();
                        transaction.Commit();

                        // 返回成功訊息
                        return Ok(new { message = "訂單已取消，並回收獲得點數和退還使用的點數。", orderId = orderId });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return InternalServerError(ex);
                    }
                }
            }
        }
    }
}
