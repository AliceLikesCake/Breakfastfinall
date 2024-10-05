using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BreakfastOrderSystem.Site.Models.Dtos;
using BreakfastOrderSystem.Site.Models.EFModels;
using BreakfastOrderSystem.Site.Models.Services;
using BreakfastOrderSystem.Site.Models.ViewModels;
using System.Data.Entity;

namespace BreakfastOrderSystem.Site.Controllers
{
    public class AddOnOptionController : Controller
    {
        // GET: AddOnCategory
        public ActionResult Index()
        {
            using (var db = new AppDbContext())
            {
                // 查詢 AddOnCategories 資料
                var data = db.AddOnCategories
                             .Select(category => new AddOnCategoryVm
                             {
                                 Id = category.Id,
                                 Name = category.Name,
                                 IsSingleChoice = category.IsSingleChoice,
                                 DisplayOrder = (int)category.DisplayOrder
                             })
                             .OrderBy(c => c.DisplayOrder) // 根據顯示順序排序
                             .ToList();

                return View(data); // 返回加選類別的資料
            }
        }


        //[HttpPost]
        //public ActionResult Create(AddOnCategoryVm model)
        //{
        //    using (var db = new AppDbContext())
        //    {
        //        // 1. 保存 AddOnCategory 信息
        //        var newCategory = new AddOnCategory
        //        {
        //            Name = model.Name,
        //            IsSingleChoice = model.IsSingleChoice,
        //            DisplayOrder = model.DisplayOrder
        //        };

        //        db.AddOnCategories.Add(newCategory);
        //        db.SaveChanges();

        //        // 2. 保存 AddOnOption 選項信息
        //        if (model.Options != null && model.Options.Count > 0)
        //        {
        //            foreach (var option in model.Options)
        //            {
        //                var newOption = new AddOnOption
        //                {
        //                    Name = option.Name,
        //                    Price = option.Price,
        //                    DisplayOrder = option.DisplayOrder,
        //                    AddOnCategoryId = newCategory.Id // 關聯剛剛的類別
        //                };
        //                db.AddOnOptions.Add(newOption);
        //            }
        //            db.SaveChanges();
        //        }

        //        return Json(new { success = true, message = "加選類別和選項已成功保存" });
        //    }
        //}


        private readonly AddOnOptionService _addOnOptionService;


        public AddOnOptionController()
        {
            _addOnOptionService = new AddOnOptionService();
        }


        [HttpPost]
        public ActionResult Create(AddOnCategoryVm model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 使用 AutoMapper 將 ViewModel 轉換為 DTO
                    var addOnCategoryDto = MvcApplication._mapper.Map<AddOnCategoryDto>(model);

                    // 調用 Service 檢查並創建 AddOnCategory 和 AddOnOption
                    var createdCategory = _addOnOptionService.CreateAddOnCategory(addOnCategoryDto);

                    // 返回成功訊息和創建的類別與選項數據
                    return Json(new
                    {
                        success = true,
                        message = "加選類別和選項已成功保存",
                        data = new
                        {
                            Id = createdCategory.Id,
                            Name = createdCategory.Name,
                            DiplayOrder= createdCategory.DisplayOrder,
                            Options = createdCategory.AddOnOptions.Select(option => new
                            {
                                OptionId = option.Id,
                                OptionName = option.Name,
                                OptionPrice = option.Price,
                                OptionDisplayOrder = option.DisplayOrder
                            })
                        }
                    });
                }
                else
                {
                    return Json(new { success = false, message = "表單驗證失敗" });
                }
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生錯誤: " + ex.Message });
            }
        }



        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var db = new AppDbContext())
            {
                // 1. 查找 AddOnCategory 
                var category = db.AddOnCategories.SingleOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return Json(new { success = false, message = "找不到該加選類別" }, JsonRequestBehavior.AllowGet);
                }

                // 2. 查找該 AddOnCategory 對應的 AddOnOptions
                var options = db.AddOnOptions
                                .Where(o => o.AddOnCategoryId == id)
                                .ToList();

                // 3. 手動創建 AddOnCategoryVm 並填充資料
                var categoryVm = new AddOnCategoryVm
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsSingleChoice = category.IsSingleChoice,
                    DisplayOrder = (int)category.DisplayOrder,
                    Options = options.Select(option => new AddOnOptionVm
                    {
                        Id = option.Id,
                        Name = option.Name,
                        Price = option.Price,
                        DisplayOrder = (int)option.DisplayOrder
                    }).ToList()
                };

                return Json(new { success = true, data = categoryVm }, JsonRequestBehavior.AllowGet);
            }
        }


        //[HttpPost]
        //public ActionResult Update(AddOnCategoryVm model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // 使用 AutoMapper 將 ViewModel 轉換為 DTO
        //            var addOnCategoryDto = MvcApplication._mapper.Map<AddOnCategoryDto>(model);

        //            // 調用 Service 檢查並更新 AddOnCategory 和 AddOnOptions
        //            _addOnOptionService.UpdateAddOnCategory(addOnCategoryDto);

        //            return Json(new { success = true, message = "加選類別和選項已成功更新" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "表單驗證失敗" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "發生錯誤: " + ex.Message });
        //    }
        //}

        [HttpPost]
        public ActionResult Update(AddOnCategoryVm model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 使用 AutoMapper 將 ViewModel 轉換為 DTO
                    var addOnCategoryDto = MvcApplication._mapper.Map<AddOnCategoryDto>(model);

                    // 調用 Service 檢查並更新 AddOnCategory 和 AddOnOptions
                    var updatedCategory = _addOnOptionService.UpdateAddOnCategory(addOnCategoryDto);

                    // 返回更新的數據
                    return Json(new
                    {
                        success = true,
                        message = "加選類別和選項已成功更新",
                        data = new
                        {
                            Id = updatedCategory.Id,
                            Name = updatedCategory.Name,
                            IsSingleChoice = updatedCategory.IsSingleChoice,
                            DisplayOrder = updatedCategory.DisplayOrder,
                            Options = updatedCategory.AddOnOptions.Select(o => new
                            {
                                Id = o.Id,
                                Name = o.Name,
                                Price = o.Price,
                                DisplayOrder = o.DisplayOrder
                            }).ToList() // 返回選項清單
                        }
                    });
                }
                else
                {
                    return Json(new { success = false, message = "表單驗證失敗" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生錯誤: " + ex.Message });
            }
        }


        [HttpDelete]
        public JsonResult Delete(int id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // 1. 查找 AddOnCategory
                    var category = db.AddOnCategories.Find(id);
                    if (category == null)
                    {
                        return Json(new { success = false, message = "找不到該加選類別" });
                    }

                    // 2. 查找並刪除對應的 AddOnOptions
                    var options = db.AddOnOptions.Where(o => o.AddOnCategoryId == id).ToList();
                    db.AddOnOptions.RemoveRange(options); // 刪除所有對應的 AddOnOptions

                    // 3. 刪除 AddOnCategory
                    db.AddOnCategories.Remove(category);
                    db.SaveChanges();

                    return Json(new { success = true, message = "加選類別及其選項已成功刪除" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "刪除過程中發生錯誤：" + ex.Message });
            }
        }

    }


}