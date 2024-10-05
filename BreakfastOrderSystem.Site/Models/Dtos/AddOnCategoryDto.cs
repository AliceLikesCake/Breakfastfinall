using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BreakfastOrderSystem.Site.Models.ViewModels;

namespace BreakfastOrderSystem.Site.Models.Dtos
{
    public class AddOnCategoryDto
    {
        public int Id { get; set; } // 加選類別ID
        public string Name { get; set; } // 加選類別名稱
        public bool IsSingleChoice { get; set; } // 單/複選
        public int DisplayOrder { get; set; } // 加選類別順序
        public List<AddOnOptionVm> Options { get; set; }
    }
}