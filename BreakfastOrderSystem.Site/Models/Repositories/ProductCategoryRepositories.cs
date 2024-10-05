using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BreakfastOrderSystem.Site.Models.Dtos;
using BreakfastOrderSystem.Site.Models.EFModels;
using BreakfastOrderSystem.Site.Models.ViewModels;

namespace BreakfastOrderSystem.Site.Models.Repositories
{
    public class ProductCategoryRepositories
    {


        public void CreateProductCategory(ProductCategory productCategory)
        {
            using (var db = new AppDbContext())
            {
                db.ProductCategories.Add(productCategory);
                db.SaveChanges();
            }
        }

        public bool IsExist(string name)
        {
            return GetByName(name) != null;
        }

        public EFModels.ProductCategory GetByName(string name)
        {
            using (var db = new AppDbContext())
            {
                return db.ProductCategories.FirstOrDefault(c => c.Name == name);
            }
        }

        //public void UpdateProductCategory(ProductCategoryDto dto)
        //{
        //    using (var db = new AppDbContext())
        //    {
        //        // 根據 ID 查找分類
        //        var category = db.ProductCategories.SingleOrDefault(c => c.Id == dto.Id);

        //        if (category == null)
        //        {
        //            throw new ArgumentException("找不到要更新的分類");
        //        }

        //        // 保存原有圖片的值
        //        var originalImage = category.Image;

        //        // 更新屬性，使用 AutoMapper 映射
        //        MvcApplication._mapper.Map(dto, category);

        //        // 處理圖片邏輯：
        //        // 1. 如果用戶選擇刪除圖片，則刪除圖片
        //        // 2. 如果 DTO 中有新圖片，則更新為新圖片
        //        // 3. 如果沒有新圖片且用戶未選擇刪除，則保留原有圖片

        //        if (dto.DeleteImage)  // 从 DTO 中判断是否需要删除图片
        //        {
        //            // 刪除當前圖片
        //            category.Image = null;
        //        }
        //        else if (!string.IsNullOrEmpty(dto.Image))
        //        {
        //            // 使用新圖片路徑
        //            category.Image = dto.Image;
        //        }
        //        else
        //        {
        //            // 保留原有圖片
        //            category.Image = originalImage;
        //        }

        //        // 保存更改
        //        db.SaveChanges();
        //    }
        //}

        public ProductCategory UpdateProductCategory(ProductCategoryDto dto)
        {
            using (var db = new AppDbContext())
            {
                // 根據 ID 查找分類
                var category = db.ProductCategories.SingleOrDefault(c => c.Id == dto.Id);

                if (category == null)
                {
                    throw new ArgumentException("找不到要更新的分類");
                }

                // 更新屬性
                category.Name = dto.Name;
                category.DisplayOrder = dto.DisplayOrder;

                // 處理圖片
                if (dto.DeleteImage)
                {
                    category.Image = null;
                }
                else if (!string.IsNullOrEmpty(dto.Image))
                {
                    category.Image = dto.Image;
                }

                db.SaveChanges(); // 保存變更
                return category;  // 返回更新後的分類
            }
        }

    }
}