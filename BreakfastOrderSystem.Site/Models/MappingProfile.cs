using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using BreakfastOrderSystem.Site.Models.Dtos;
using BreakfastOrderSystem.Site.Models.ViewModels;
using BreakfastOrderSystem.Site.Models.EFModels;
using System.IO;
using System.Security.Cryptography;


namespace BreakfastOrderSystem.Site.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //單向映射
            CreateMap<ProductCategoryVm, ProductCategoryDto>();
            CreateMap<ProductCategoryDto, ProductCategory>();

            CreateMap<AddOnCategoryVm, AddOnCategoryDto>();
            CreateMap<AddOnCategoryDto, AddOnCategory>();
            CreateMap<AddOnCategory, AddOnCategoryVm>();

            CreateMap<AddOnOptionDto, AddOnOption>();

        }

    }
}