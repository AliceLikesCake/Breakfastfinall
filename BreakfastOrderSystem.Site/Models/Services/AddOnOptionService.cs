using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BreakfastOrderSystem.Site.Models.Dtos;
using BreakfastOrderSystem.Site.Models.EFModels;
using BreakfastOrderSystem.Site.Models.Repositories;

namespace BreakfastOrderSystem.Site.Models.Services
{
    public class AddOnOptionService
    {
        private AddOnOptionRepositories _repo;

        public AddOnOptionService()
        {
            _repo = new AddOnOptionRepositories();
        }
        public AddOnCategory CreateAddOnCategory(AddOnCategoryDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // 檢查是否已存在相同名稱的類別
            if (_repo.IsExist(dto.Name)) throw new ArgumentException("類別名字已存在");

            // 調用 Repository 進行保存操作並返回創建的實體
            return _repo.CreateAddOnCategory(dto);
        }


        //public void UpdateAddOnCategory(AddOnCategoryDto addOnCategoryDto)
        //{
        //    if (addOnCategoryDto == null) throw new ArgumentNullException(nameof(addOnCategoryDto));

        //    // 檢查分類名稱是否已存在
        //    var addOnCategoryInDb = _repo.GetByName(addOnCategoryDto.Name);
        //    if (addOnCategoryInDb != null && addOnCategoryInDb.Id != addOnCategoryDto.Id)
        //        throw new ArgumentException("類別名字已存在");

        //    // 調用 Repository 更新分類
        //    _repo.UpdateProductCategory(addOnCategoryDto);
        //}

        public AddOnCategory UpdateAddOnCategory(AddOnCategoryDto addOnCategoryDto)
        {
            if (addOnCategoryDto == null) throw new ArgumentNullException(nameof(addOnCategoryDto));

            // 檢查分類名稱是否已存在
            var addOnCategoryInDb = _repo.GetByName(addOnCategoryDto.Name);
            if (addOnCategoryInDb != null && addOnCategoryInDb.Id != addOnCategoryDto.Id)
                throw new ArgumentException("類別名字已存在");

            // 調用 Repository 更新分類，並返回更新後的 AddOnCategory
            return _repo.UpdateProductCategory(addOnCategoryDto);
        }

    }
}