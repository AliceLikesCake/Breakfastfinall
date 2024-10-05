using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BreakfastOrderSystem.Site.Models.Dtos;
using BreakfastOrderSystem.Site.Models.EFModels;
using BreakfastOrderSystem.Site.Models.Repositories;

namespace BreakfastOrderSystem.Site.Models.Services
{
    public class ProductCategoryService
    {

        private ProductCategoryRepositories _repo;

        public ProductCategoryService()
        {
            _repo = new ProductCategoryRepositories();
        }
        public ProductCategory CreateProductCategory(ProductCategoryDto dto)
        {
            // 檢查參數是否有效
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (_repo.IsExist(dto.Name)) throw new ArgumentException("類別名字已存在");

            // 使用 AutoMapper 或手動映射將 DTO 轉換為實體類
            ProductCategory productCategory = MvcApplication._mapper.Map<ProductCategory>(dto);

            // 調用儲存邏輯
            _repo.CreateProductCategory(productCategory);

            // 返回創建的 ProductCategory
            return productCategory;
        }

        public ProductCategory UpdateProductCategory(ProductCategoryDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // 檢查類別名稱是否存在於資料庫
            var categoryInDb = _repo.GetByName(dto.Name);
            if (categoryInDb != null && categoryInDb.Id != dto.Id)
                throw new ArgumentException("類別名字已存在");

            // 調用儲存庫的更新方法
            var updatedCategory = _repo.UpdateProductCategory(dto);

            // 返回更新後的類別
            return updatedCategory;
        }

    }
}