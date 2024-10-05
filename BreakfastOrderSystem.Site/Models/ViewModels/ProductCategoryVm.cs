using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BreakfastOrderSystem.Site.Models.EFModels;

namespace BreakfastOrderSystem.Site.Models.ViewModels
{
    public class ProductCategoryVm
    {
        public int? Id { get; set; }



       
        [StringLength(255)]
        public string Image { get; set; }


        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        

        public int? DisplayOrder { get; set; }

        public bool DeleteImage { get; set; } // 标识是否要删除图片


    }
}