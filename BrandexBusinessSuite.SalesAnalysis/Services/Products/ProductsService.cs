﻿namespace BrandexBusinessSuite.SalesAnalysis.Services.Products;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;
    
using Data;
using SalesAnalysis.Data.Models;
using SalesAnalysis.Models.Products;

public class ProductsService : IProductsService
{
    SalesAnalysisDbContext db;

    public ProductsService(SalesAnalysisDbContext db)
    {
        this.db = db;
    }

    public async Task<string> CreateProduct(ProductInputModel productInputModel)
    {
        if (productInputModel.BrandexId == 0 || productInputModel.Name == null || productInputModel.ShortName == null ||
            productInputModel.Price == 0) return "";
        var productDBModel = new Product
        {
            Name = productInputModel.Name,
            Price = productInputModel.Price,
            ShortName = productInputModel.ShortName,
            BrandexId = productInputModel.BrandexId,
            PharmnetId = productInputModel.PharmnetId,
            PhoenixId = productInputModel.PhoenixId,
            SopharmaId = productInputModel.SopharmaId,
            StingId = productInputModel.StingId
        };

        await db.Products.AddAsync(productDBModel);
        await db.SaveChangesAsync();
        return productDBModel.Name;

    }

    public async Task<List<ProductCheckModel>> GetProductsCheck()
    {
        return await db.Products.Select(p => new ProductCheckModel
        {
            Id = p.Id,
            BrandexId = p.BrandexId,
            PhoenixId = p.PhoenixId,
            PharmnetId = p.PharmnetId,
            StingId = p.StingId,
            SopharmaId = p.SopharmaId
        }).ToListAsync();
    }

    public async Task<List<string>> GetProductsNames()
    {
        return await db.Products.Select(p => p.Name).ToListAsync();
    }

    public async Task<List<ProductShortOutputModel>> GetProductsIdPrices()
    {
        return await db.Products.Select(p => new ProductShortOutputModel
        {
            Name = p.Name,
            Id = p.Id,
            Price = p.Price
        }).ToListAsync();
    }
}