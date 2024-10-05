using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BreakfastOrderSystem.Site.Models.ViewModels
{
    public class ProductVm
    {
        public int Id { get; set; } // 商品 ID
        public string Name { get; set; } // 商品名稱
        public int Price { get; set; } // 商品價格
        public bool IsAvailable { get; set; } // 商品是否上架
        public string Image { get; set; } // 商品圖片
        public string ProductCategoryName { get; set; } // 商品類別名稱
        public string AddOnCategoryName { get; set; } // 加選類別名稱

        public int DisplayOrder { get; set; }
        public int ProductCategoryId { get; set; } // 加選類別名稱

        // 加選類別的詳細資訊 (以JSON形式傳遞)
        public string AddOnDetailsJson { get; set; }
        public List<string> AddOnCategoryNames { get; set; }

        public List<ProductAddOnDetailVm> AddOnDetails { get; set; }

        public bool DeleteImage { get; set; } // 标识是否要删除图片
    }
}