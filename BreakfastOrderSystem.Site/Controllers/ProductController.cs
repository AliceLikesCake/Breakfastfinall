using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BreakfastOrderSystem.Site.Models.EFModels;
using BreakfastOrderSystem.Site.Models.ViewModels;
using Newtonsoft.Json;
using System.Data.Entity;

namespace BreakfastOrderSystem.Site.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            using (var db = new AppDbContext())
            {
                // 查詢商品和關聯的商品類別以及加選類別
                var products = db.Products
                    .Select(product => new ProductVm
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        IsAvailable = product.IsAvailable,
                        
                        Image = product.Image, // 商品圖片
                        ProductCategoryName = product.ProductCategory.Name, // 商品類別名稱
                        AddOnCategoryNames = db.ProductAddOnDetails
                            .Where(d => d.ProductId == product.Id)
                            .Select(d => d.AddOnCategory.Name)
                            .Distinct() // 去除重複的加選類別名稱
                            .ToList() // 取得與該商品相關的所有加選類別名稱
                    })
                    .OrderBy(p => p.ProductCategoryName) // 根據商品名稱排序
                    .ToList();

                products.ForEach(p =>
                {
                    if (!p.AddOnCategoryNames.Any())
                    {
                        p.AddOnCategoryNames.Add("無加選");
                    }
                });

                return View(products); // 返回前端
            }
        }

        public JsonResult GetProductCategories()
        {
            using (var db = new AppDbContext())
            {
                // 從資料庫中選取商品類別的名稱和 ID
                var categories = db.ProductCategories
                                   .Select(c => new
                                   {
                                       Id = c.Id,
                                       Name = c.Name
                                   })
                                   .ToList();

                // 返回 JSON 資料給前端
                return Json(categories, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAddOnCategories()
        {
            using (var db = new AppDbContext())
            {
                // 從資料庫中選取商品類別的名稱和 ID，並且包含對應的 AddOnOptions
                var addOnCategories = db.AddOnCategories
                                        .Select(c => new
                                        {
                                            Id = c.Id,
                                            Name = c.Name,
                                            Options = c.AddOnOptions.Select(o => new
                                            {
                                                OptionId = o.Id,
                                                OptionName = o.Name
                                            }).ToList()
                                        })
                                        .ToList();

                // 返回 JSON 資料給前端，包含類別及對應的選項
                return Json(addOnCategories, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]

        //public ActionResult Create(ProductVm model, HttpPostedFileBase Image)
        //{
        //    using (var db = new AppDbContext())
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            // 1. 創建並保存商品紀錄
        //            var product = new Product
        //            {
        //                Name = model.Name,
        //                Price = model.Price,
        //                IsAvailable = model.IsAvailable,
        //                ProductCategoryId = model.ProductCategoryId,
        //                DisplayOrder = model.DisplayOrder
        //            };

        //            // 處理圖片上傳
        //            if (Image != null && Image.ContentLength > 0)
        //            {
        //                // 獲取文件名
        //                var fileName = Path.GetFileName(Image.FileName);

        //                // 設定圖片保存的路徑
        //                var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);

        //                // 將圖片保存到伺服器
        //                Image.SaveAs(path);

        //                // 將圖片路徑保存到 product 對象
        //                product.Image = "/Content/images/" + fileName;
        //            }

        //            // 保存商品到資料庫
        //            db.Products.Add(product);
        //            db.SaveChanges(); // 保存商品，獲取新商品的 ID

        //            // 2. 保存加選類別的詳細紀錄
        //            if (!string.IsNullOrEmpty(model.AddOnDetailsJson))
        //            {
        //                // 反序列化加選類別的詳細資訊
        //                var addOnDetails = JsonConvert.DeserializeObject<List<ProductAddOnDetailVm>>(model.AddOnDetailsJson);

        //                if (addOnDetails != null && addOnDetails.Any())
        //                {
        //                    foreach (var detail in addOnDetails)
        //                    {
        //                        if (detail.Options != null && detail.Options.Any())
        //                        {
        //                            foreach (var option in detail.Options)
        //                            {
        //                                var addOnDetail = new ProductAddOnDetail
        //                                {
        //                                    ProductId = product.Id, // 使用剛剛創建的商品 ID
        //                                    AddOnCategoryId = detail.AddOnCategoryId,
        //                                    AddOnOptionId = option.OptionId,  // 選項 ID
        //                                    AddOnOptionName = option.OptionName  // 選項名稱
        //                                };
        //                                db.ProductAddOnDetails.Add(addOnDetail);
        //                            }
        //                        }
        //                    }
        //                    db.SaveChanges(); // 保存加選資訊
        //                }
        //            }

        //            return Json(new { success = true, message = "商品與加選資訊已成功保存" });
        //        }

        //        // 返回驗證錯誤
        //        return Json(new { success = false, message = "表單驗證失敗" });


        //    }
        //}
        [HttpPost]
        public ActionResult Create(ProductVm model, HttpPostedFileBase Image)
        {
            using (var db = new AppDbContext())
            {
                if (ModelState.IsValid)
                {
                    // 1. 創建並保存商品紀錄
                    var product = new Product
                    {
                        Name = model.Name,
                        Price = model.Price,
                        IsAvailable = model.IsAvailable,
                        ProductCategoryId = model.ProductCategoryId,
                        DisplayOrder = model.DisplayOrder
                    };

                    // 處理圖片上傳邏輯
                    if (Image != null && Image.ContentLength > 0)
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

                        product.Image = Path.Combine(fileName);
                    }

                    db.Products.Add(product);
                    db.SaveChanges(); // 保存商品，獲取新商品的 ID

                    // 2. 保存加選類別的詳細紀錄
                    if (!string.IsNullOrEmpty(model.AddOnDetailsJson))
                    {
                        var addOnDetails = JsonConvert.DeserializeObject<List<ProductAddOnDetailVm>>(model.AddOnDetailsJson);

                        if (addOnDetails != null && addOnDetails.Any())
                        {
                            foreach (var detail in addOnDetails)
                            {
                                if (detail.Options != null && detail.Options.Any())
                                {
                                    foreach (var option in detail.Options)
                                    {
                                        var addOnDetail = new ProductAddOnDetail
                                        {
                                            ProductId = product.Id,
                                            AddOnCategoryId = detail.AddOnCategoryId,
                                            AddOnOptionId = option.OptionId,
                                            AddOnOptionName = option.OptionName
                                        };
                                        db.ProductAddOnDetails.Add(addOnDetail);
                                    }
                                }
                            }
                            db.SaveChanges(); // 保存加選資訊
                        }
                    }

                    // 查詢該商品的所有加選類別名稱
                    var addOnCategoryNames = db.ProductAddOnDetails
                        .Where(p => p.ProductId == product.Id)
                        .Select(p => p.AddOnCategory.Name)
                        .Distinct()
                        .ToList();

                    // 查詢商品的類別名稱
                    var productCategoryName = db.ProductCategories
                        .Where(pc => pc.Id == product.ProductCategoryId)
                        .Select(pc => pc.Name)
                        .FirstOrDefault();

                    // 查詢所有加選項目的資料
                    var addOnDetailsList = db.ProductAddOnDetails
                        .Where(p => p.ProductId == product.Id)
                        .Select(p => new
                        {
                            p.AddOnCategoryId,
                            p.AddOnOptionId,
                            p.AddOnOptionName
                        })
                        .ToList();

                    // 返回成功訊息和新創建的商品資料，包括類別名稱、加選類別名稱和加選項目資料
                    var createdProduct = new
                    {
                        product.Id,
                        product.Name,
                        product.Price,
                        product.Image,
                        product.IsAvailable,
                        ProductCategory = productCategoryName, // 這裡是類別名稱
                        AddOnCategoryNames = addOnCategoryNames, // 包含加選類別名稱
                        AddOnDetails = addOnDetailsList // 包含所有加選項目資料
                    };

                    return Json(new { success = true, message = "商品與加選資訊已成功保存", data = createdProduct });
                }

                // 返回驗證錯誤
                return Json(new { success = false, message = "表單驗證失敗" });
            }
        }



        [HttpGet]
        public JsonResult Edit(int id)
        {
            using (var db = new AppDbContext())
            {
                // 查找對應的產品
                var product = db.Products
                                .Include(p => p.ProductAddOnDetails.Select(d => d.AddOnOption)) // 包含加選項和選項詳細資料
                                .FirstOrDefault(p => p.Id == id);

                if (product == null)
                {
                    return Json(new { success = false, message = "找不到該商品" }, JsonRequestBehavior.AllowGet);
                }

                // 構建返回的產品資料
                var productVm = new ProductVm
                {
                    Id = product.Id,
                    ProductCategoryId = product.ProductCategoryId,
                    ProductCategoryName = product.ProductCategory.Name,
                    Name = product.Name,
                    Price = product.Price,
                    DisplayOrder = (int)product.DisplayOrder,
                    IsAvailable = product.IsAvailable,
                    Image = product.Image,

                    // 構建 AddOnDetails，確保 Options 正確返回
                    AddOnDetails = product.ProductAddOnDetails
                        .GroupBy(a => new { a.AddOnCategoryId, a.AddOnCategory.Name }) // 將同一加選類別的選項聚合
                        .Select(g => new ProductAddOnDetailVm
                        {
                            AddOnCategoryId = g.Key.AddOnCategoryId,
                            AddOnCategoryName = g.Key.Name,
                            Options = g.Select(a => new OptionVm
                            {
                                OptionId = a.AddOnOptionId,
                                OptionName = a.AddOnOptionName
                            }).ToList() // 返回該類別下的所有選項
                        }).ToList()
                };

                // 返回 JSON 格式的產品資料
                return Json(new { success = true, data = productVm }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Update(ProductVm model, HttpPostedFileBase Image)
        {
            using (var db = new AppDbContext())
            {
                if (ModelState.IsValid)
                {
                    // 查找現有的商品
                    var product = db.Products.FirstOrDefault(p => p.Id == model.Id);
                    if (product == null)
                    {
                        return Json(new { success = false, message = "商品不存在" });
                    }

                    // 保存原始圖片路徑以便後續處理
                    var originalImage = product.Image;

                    // 更新商品屬性
                    product.Name = model.Name;
                    product.Price = model.Price;
                    product.IsAvailable = model.IsAvailable;
                    product.ProductCategoryId = model.ProductCategoryId;
                    product.DisplayOrder = model.DisplayOrder;

                    // 處理圖片邏輯
                    if (Image != null && Image.ContentLength > 0)
                    {
                        // 取得從配置檔中的路徑
                        string uploadPath = System.Configuration.ConfigurationManager.AppSettings["uploadPath"].ToString();
                        string storageSite = System.Configuration.ConfigurationManager.AppSettings["storageSite"].ToString();

                        // 組合保存的圖片路徑
                        uploadPath = Path.Combine(uploadPath, "Products");

                        // 確保路徑存在，如果不存在則創建
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // 獲取文件名
                        var fileName = Path.GetFileName(Image.FileName);

                        // 保存圖片到指定路徑
                        var fullPath = Path.Combine(uploadPath, fileName);
                        Image.SaveAs(fullPath);

                        // 保存圖片的存儲相對路徑到資料庫
                        product.Image = Path.Combine(fileName);
                    }

                    // 保存商品的變更
                    db.SaveChanges();

                    // 移除現有的加選類別與選項詳細記錄
                    var existingAddOnDetails = db.ProductAddOnDetails.Where(d => d.ProductId == product.Id).ToList();
                    db.ProductAddOnDetails.RemoveRange(existingAddOnDetails);
                    db.SaveChanges();

                    // 保存新的加選類別的詳細紀錄
                    if (!string.IsNullOrEmpty(model.AddOnDetailsJson))
                    {
                        var addOnDetails = JsonConvert.DeserializeObject<List<ProductAddOnDetailVm>>(model.AddOnDetailsJson);

                        if (addOnDetails != null && addOnDetails.Any())
                        {
                            foreach (var detail in addOnDetails)
                            {
                                if (detail.Options != null && detail.Options.Any())
                                {
                                    foreach (var option in detail.Options)
                                    {
                                        var addOnDetail = new ProductAddOnDetail
                                        {
                                            ProductId = product.Id,
                                            AddOnCategoryId = detail.AddOnCategoryId,
                                            AddOnOptionId = option.OptionId,
                                            AddOnOptionName = option.OptionName
                                        };
                                        db.ProductAddOnDetails.Add(addOnDetail);
                                    }
                                }
                            }
                            db.SaveChanges(); // 保存新的加選資訊
                        }
                    }

                    // 查詢該商品的所有加選類別名稱
                    var addOnCategoryNames = db.ProductAddOnDetails
                        .Where(p => p.ProductId == product.Id)
                        .Select(p => p.AddOnCategory.Name)
                        .Distinct()
                        .ToList();

                    // 查詢商品的類別名稱
                    var productCategoryName = db.ProductCategories
                        .Where(pc => pc.Id == product.ProductCategoryId)
                        .Select(pc => pc.Name)
                        .FirstOrDefault();

                    // 查詢所有加選項目的資料
                    var addOnDetailsList = db.ProductAddOnDetails
                        .Where(p => p.ProductId == product.Id)
                        .Select(p => new
                        {
                            p.AddOnCategoryId,
                            p.AddOnOptionId,
                            p.AddOnOptionName
                        })
                        .ToList();

                    // 返回成功訊息和新創建的商品資料，包括類別名稱、加選類別名稱和加選項目資料
                    var createdProduct = new
                    {
                        product.Id,
                        product.Name,
                        product.Price,
                        product.Image,
                        product.IsAvailable,
                        ProductCategory = productCategoryName, // 這裡是類別名稱
                        AddOnCategoryNames = addOnCategoryNames, // 包含加選類別名稱
                        AddOnDetails = addOnDetailsList // 包含所有加選項目資料
                    };

                    return Json(new { success = true, message = "商品與加選資訊已成功保存", data = createdProduct });
                
            }

                return Json(new { success = false, message = "表單驗證失敗" });
            }
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            using (var db = new AppDbContext())
            {
                // 查找對應的商品
                var product = db.Products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    return Json(new { success = false, message = "商品不存在" }, JsonRequestBehavior.AllowGet);
                }

                // 刪除該產品相關的加選類別與選項
                var addOnDetails = db.ProductAddOnDetails.Where(d => d.ProductId == id).ToList();
                db.ProductAddOnDetails.RemoveRange(addOnDetails);

                // 刪除該商品
                db.Products.Remove(product);
                db.SaveChanges();

                return Json(new { success = true, message = "商品已成功刪除" }, JsonRequestBehavior.AllowGet);
            }
        }
    }


}