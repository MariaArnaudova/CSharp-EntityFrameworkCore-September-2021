﻿using AutoMapper;
using ProductShop.Data;
using ProductShop.DataTransferObjects;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserInputModel, User>();

            this.CreateMap<ProductInputModel, Product>();

            this.CreateMap<CategoryInputModel, Category>();

            this.CreateMap<CategoryProductInputModel, CategoryProduct>();
        }
    }
}
