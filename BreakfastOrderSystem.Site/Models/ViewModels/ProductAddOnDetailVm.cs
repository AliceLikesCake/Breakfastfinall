using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BreakfastOrderSystem.Site.Models.ViewModels
{
    public class ProductAddOnDetailVm
    {
        public int Id { get; set; }  // 商品加選ID
        public int ProductId { get; set; }  // 商品ID
        public string ProductName { get; set; }  // 商品名稱
        public int AddOnCategoryId { get; set; }  // 加選類別ID
        public string AddOnCategoryName { get; set; }  // 加選類別名稱
        public List<OptionVm> Options { get; set; }
        public string AddOnOptionName { get; set; }

        public int AddOnOptionId { get; set; }


    }

}