using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BreakfastOrderSystem.Site.Models.ViewModels
{
    public class ProductIndexVm
    {
        public List<ProductCategoryVm> ProductCategories { get; set; }
        public List<ProductVm> Products { get; set; }
        public List<AddOnCategoryVm> AddOnCategories { get; set; }
        public List<ProductAddOnDetailVm> ProductAddOnDetails { get; set; }
    }
}