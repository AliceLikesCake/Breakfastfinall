using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BreakfastOrderSystem.Site.Models.Dtos
{
    public class ProductCategoryDto
    {
        public int Id { get; set; }


        public string Name { get; set; }

        public int? DisplayOrder { get; set; }

        public string Image { get; set; }

        public bool DeleteImage { get; set; } // 标识是否要删除图片

    }
}