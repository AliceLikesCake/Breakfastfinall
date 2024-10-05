using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BreakfastOrderSystem.Site.Models.EFModels;
using System.Data.Entity;
using BreakfastOrderSystem.Site.Models.ViewModels;
using System.IO;
using BreakfastOrderSystem.Site.Models.Services;
using BreakfastOrderSystem.Site.Models.Dtos;
namespace BreakfastOrderSystem.Site.Controllers
{
    public class ProductCategoryController : Controller
    {
        // GET: 秀出商品分類
        public ActionResult Index()
        {
            var db = new AppDbContext();
            var data = db.ProductCategories
            .AsNoTracking()
       
            
            .Select(p => new ProductCategoryVm
            {
                Id = p.Id,
                Name = p.Name,
                Image= p.Image,

                DisplayOrder = p.DisplayOrder,
            })
            .OrderBy(p => p.DisplayOrder ?? int.MaxValue)
            .ToList();//逼迫它在TOLIST時就查詢

            return View(data);
        }

        private readonly ProductCategoryService _productCategoryService;


        public ProductCategoryController()
        {
            _productCategoryService = new ProductCategoryService();
        }

       //新增商品分類
        [HttpPost]
        public JsonResult Create(ProductCategoryVm model, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var productCategoryDto = new ProductCategoryDto
                {
                    Name = model.Name,
                    DisplayOrder = model.DisplayOrder
                };

                // 處理圖片邏輯
                if (model.DeleteImage)
                {
                    productCategoryDto.Image = null;
                }
                else if (Image != null && Image.ContentLength > 0)
                {
                    string uploadPath = System.Configuration.ConfigurationManager.AppSettings["uploadPath"].ToString();
                    string storageSite = System.Configuration.ConfigurationManager.AppSettings["storageSite"].ToString();
                    uploadPath = Path.Combine(uploadPath, "Products");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var fileName = Path.GetFileName(Image.FileName);
                    var fullPath = Path.Combine(uploadPath, fileName);
                    Image.SaveAs(fullPath);

                    productCategoryDto.Image = fileName;
                }

                try
                {
                    // 創建類別
                    var createdCategory = _productCategoryService.CreateProductCategory(productCategoryDto);

                    // 返回包含創建的類別數據
                    return Json(new
                    {
                        success = true,
                        message = "類別已成功創建",
                        data = new
                        {
                            Id = createdCategory.Id, // 假設返回類別的 Id
                            Name = createdCategory.Name,
                            DisplayOrder = createdCategory.DisplayOrder,
                            Image = createdCategory.Image
                        }
                    });
                }
                catch (ArgumentException ex)
                {
                    return Json(new { success = false, message = "類別已存在" });
                }
            }

            return Json(new { success = false, message = "表單驗證失敗" });
        }


        //編輯時依id尋找資料
        [HttpGet]
        public JsonResult Edit(int id)
        {
            var db = new AppDbContext();
            var productCategory = db.ProductCategories
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.DisplayOrder,
                    c.Image
                })
                .FirstOrDefault();

            if (productCategory != null)
            {
                return Json(new { success = true, data = productCategory }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "找不到該商品類別" }, JsonRequestBehavior.AllowGet);
            }
        }

        //更新商品類別
       //[HttpPost]
       // public ActionResult Update(ProductCategoryVm model, HttpPostedFileBase Image)
       // {
       //     if (ModelState.IsValid)
       //     {
       //         // 使用 AutoMapper 將 ViewModel 轉換為 DTO
       //         var productCategoryDto = MvcApplication._mapper.Map<ProductCategoryDto>(model);

       //         // 處理圖片邏輯
       //         if (model.DeleteImage)
       //         {
       //             // 如果標記為刪除圖片，將 DTO 中的圖片設置為 null
       //             productCategoryDto.Image = null;
       //         }
       //         else if (Image != null && Image.ContentLength > 0)
       //         {
       //             // 取得從配置檔中的路徑
       //             string uploadPath = System.Configuration.ConfigurationManager.AppSettings["uploadPath"].ToString();
       //             string storageSite = System.Configuration.ConfigurationManager.AppSettings["storageSite"].ToString();

       //             // 組合保存的圖片路徑
       //             uploadPath = Path.Combine(uploadPath, "Products");

       //             // 確保路徑存在，如果不存在則創建
       //             if (!Directory.Exists(uploadPath))
       //             {
       //                 Directory.CreateDirectory(uploadPath);
       //             }

       //             // 獲取文件名
       //             var fileName = Path.GetFileName(Image.FileName);

       //             // 保存圖片到指定路徑
       //             var fullPath = Path.Combine(uploadPath, fileName);
       //             Image.SaveAs(fullPath);

       //             // 保存圖片的存儲相對路徑到資料庫
       //             productCategoryDto.Image = Path.Combine(fileName);
       //         }


       //         try
       //         {
       //             // 調用服務層進行更新操作
       //             _productCategoryService.UpdateProductCategory(productCategoryDto);

       //             // 返回成功訊息
       //             return Json(new { success = true, message = "類別已成功更新" });
       //         }
       //         catch (ArgumentException ex)
       //         {
       //             // 返回錯誤訊息
       //             return Json(new { success = false, message = ex.Message });
       //         }
       //     }

       //     // 返回表單驗證失敗的錯誤訊息
       //     return Json(new { success = false, message = "表單驗證失敗" });
       // }






        [HttpDelete]
        public JsonResult Delete(int id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var productCategory = db.ProductCategories.Find(id);
                    if (productCategory == null)
                    {
                        return Json(new { success = false, message = "找不到該商品類別" });
                    }

                    db.ProductCategories.Remove(productCategory);
                    db.SaveChanges();

                    return Json(new { success = true, message = "類別已成功刪除" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "刪除過程中發生錯誤：" + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Update(ProductCategoryVm model, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var productCategoryDto = new ProductCategoryDto
                {
                    Id = (int)model.Id,
                    Name = model.Name,
                    DisplayOrder = model.DisplayOrder
                };

                // 圖片邏輯處理
                if (model.DeleteImage)
                {
                    productCategoryDto.Image = null;
                }
                else if (Image != null && Image.ContentLength > 0)
                {
                    string uploadPath = System.Configuration.ConfigurationManager.AppSettings["uploadPath"].ToString();
                    string storageSite = System.Configuration.ConfigurationManager.AppSettings["storageSite"].ToString();
                    uploadPath = Path.Combine(uploadPath, "Products");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var fileName = Path.GetFileName(Image.FileName);
                    var fullPath = Path.Combine(uploadPath, fileName);
                    Image.SaveAs(fullPath);

                    productCategoryDto.Image = fileName;
                }

                try
                {
                    // 更新類別
                    var updatedCategory = _productCategoryService.UpdateProductCategory(productCategoryDto);

                    // 返回包含更新後的類別數據
                    return Json(new
                    {
                        success = true,
                        message = "類別已成功更新",
                        data = new
                        {
                            Id = updatedCategory.Id, // 返回類別的 ID
                            Name = updatedCategory.Name,
                            DisplayOrder = updatedCategory.DisplayOrder,
                            Image = updatedCategory.Image
                        }
                    });
                }
                catch (ArgumentException ex)
                {
                    // 返回錯誤訊息
                    return Json(new { success = false, message = ex.Message });
                }
            }

            // 表單驗證失敗
            return Json(new { success = false, message = "表單驗證失敗" });
        }

    }
}
