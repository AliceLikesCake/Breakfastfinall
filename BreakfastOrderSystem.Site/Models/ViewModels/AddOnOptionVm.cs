using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BreakfastOrderSystem.Site.Models.ViewModels
{
    public class AddOnOptionVm
    {
        public int Id { get; set; } // 加選資訊ID
        public string Name { get; set; } // 加選資訊名稱
        public int Price { get; set; } // 加選資訊價格
        public int AddOnCategoryId { get; set; } // 關聯的加選類別ID
        public int DisplayOrder { get; set; } // 加選資訊順序
    }
}