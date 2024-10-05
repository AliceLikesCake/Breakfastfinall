using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BreakfastOrderSystem.Site.Models.Dtos;
using BreakfastOrderSystem.Site.Models.EFModels;
using System.Data.Entity;

namespace BreakfastOrderSystem.Site.Models.Repositories
{
    public class AddOnOptionRepositories
    {
        public bool IsExist(string name)
        {
            return GetByName(name) != null;
        }

        public EFModels.AddOnCategory GetByName(string name)
        {
            using (var db = new AppDbContext())
            {
                return db.AddOnCategories.FirstOrDefault(c => c.Name == name);
            }
        }

        //public void CreateAddOnCategory(AddOnCategoryDto dto)
        //{
        //    AddOnCategory addOnCategory = MvcApplication._mapper.Map<AddOnCategory>(dto);

        //    using (var db = new AppDbContext())
        //    {
        //        db.AddOnCategories.Add(addOnCategory);
        //        db.SaveChanges();
        //    }
        //}


        public AddOnCategory CreateAddOnCategory(AddOnCategoryDto dto)
        {
            // 使用 AutoMapper 將 DTO 轉換為實體
            AddOnCategory addOnCategory = MvcApplication._mapper.Map<AddOnCategory>(dto);

            using (var db = new AppDbContext())
            {
                // 儲存 AddOnCategory
                db.AddOnCategories.Add(addOnCategory);
                db.SaveChanges();

                // 如果有選項，則一起保存 AddOnOptions
                if (dto.Options != null && dto.Options.Any())
                {
                    foreach (var option in dto.Options)
                    {
                        var addOnOption = new AddOnOption
                        {
                            Name = option.Name,
                            Price = option.Price,
                            DisplayOrder = option.DisplayOrder,
                            AddOnCategoryId = addOnCategory.Id
                        };

                        db.AddOnOptions.Add(addOnOption);
                    }

                    db.SaveChanges();
                }

                // 返回新增的 AddOnCategory 實體，包括關聯的選項
                db.Entry(addOnCategory).Collection(c => c.AddOnOptions).Load();
                return addOnCategory;
            }
        }


        //public void UpdateProductCategory(AddOnCategoryDto dto)
        //{
        //    using (var db = new AppDbContext())
        //    {
        //        // 1. 查找對應的 AddOnCategory
        //        var existingCategory = db.AddOnCategories
        //                                 .Include(c => c.AddOnOptions)
        //                                 .SingleOrDefault(c => c.Id == dto.Id);

        //        if (existingCategory == null)
        //        {
        //            throw new ArgumentException("找不到該加選類別");
        //        }

        //        // 2. 更新 AddOnCategory 的屬性
        //        existingCategory.Name = dto.Name;
        //        existingCategory.IsSingleChoice = dto.IsSingleChoice;
        //        existingCategory.DisplayOrder = dto.DisplayOrder;

        //        // 3. 更新 AddOnOptions
        //        var existingOptions = existingCategory.AddOnOptions.ToList();

        //        // 刪除 DTO 中沒有的選項
        //        foreach (var option in existingOptions)
        //        {
        //            if (!dto.Options.Any(o => o.Id == option.Id))
        //            {
        //                db.AddOnOptions.Remove(option); // 刪除資料庫中有，但 DTO 中不存在的選項
        //            }
        //        }

        //        // 新增或更新 DTO 中的選項
        //        foreach (var dtoOption in dto.Options)
        //        {
        //            var existingOption = existingOptions.SingleOrDefault(o => o.Id == dtoOption.Id);

        //            if (existingOption == null)
        //            {
        //                // 新增選項
        //                var newOption = new AddOnOption
        //                {
        //                    Name = dtoOption.Name,
        //                    Price = dtoOption.Price,
        //                    DisplayOrder = dtoOption.DisplayOrder,
        //                    AddOnCategoryId = existingCategory.Id
        //                };
        //                db.AddOnOptions.Add(newOption);
        //            }
        //            else
        //            {
        //                // 更新現有的選項
        //                existingOption.Name = dtoOption.Name;
        //                existingOption.Price = dtoOption.Price;
        //                existingOption.DisplayOrder = dtoOption.DisplayOrder;
        //            }
        //        }

        //        // 4. 保存所有更改
        //        db.SaveChanges();
        //    }
        //}

        public AddOnCategory UpdateProductCategory(AddOnCategoryDto dto)
        {
            using (var db = new AppDbContext())
            {
                // 1. 查找對應的 AddOnCategory
                var existingCategory = db.AddOnCategories
                                         .Include(c => c.AddOnOptions)
                                         .SingleOrDefault(c => c.Id == dto.Id);

                if (existingCategory == null)
                {
                    throw new ArgumentException("找不到該加選類別");
                }

                // 2. 更新 AddOnCategory 的屬性
                existingCategory.Name = dto.Name;
                existingCategory.IsSingleChoice = dto.IsSingleChoice;
                existingCategory.DisplayOrder = dto.DisplayOrder;

                // 3. 更新 AddOnOptions
                var existingOptions = existingCategory.AddOnOptions.ToList();

                // 刪除 DTO 中沒有的選項
                foreach (var option in existingOptions)
                {
                    if (!dto.Options.Any(o => o.Id == option.Id))
                    {
                        db.AddOnOptions.Remove(option); // 刪除資料庫中有，但 DTO 中不存在的選項
                    }
                }

                // 新增或更新 DTO 中的選項
                foreach (var dtoOption in dto.Options)
                {
                    var existingOption = existingOptions.SingleOrDefault(o => o.Id == dtoOption.Id);

                    if (existingOption == null)
                    {
                        // 新增選項
                        var newOption = new AddOnOption
                        {
                            Name = dtoOption.Name,
                            Price = dtoOption.Price,
                            DisplayOrder = dtoOption.DisplayOrder,
                            AddOnCategoryId = existingCategory.Id
                        };
                        db.AddOnOptions.Add(newOption);
                    }
                    else
                    {
                        // 更新現有的選項
                        existingOption.Name = dtoOption.Name;
                        existingOption.Price = dtoOption.Price;
                        existingOption.DisplayOrder = dtoOption.DisplayOrder;
                    }
                }

                // 4. 保存所有更改
                db.SaveChanges();

                // 返回更新後的類別資料，包含更新後的選項
                return existingCategory;
            }
        }

    }
}