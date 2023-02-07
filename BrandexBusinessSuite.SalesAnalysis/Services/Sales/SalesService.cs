namespace BrandexBusinessSuite.SalesAnalysis.Services.Sales;

using AutoMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

using Data;
using Models.Sales;
using BrandexBusinessSuite.SalesAnalysis.Data.Models;


public class SalesService :ISalesService
{
    private readonly SalesAnalysisDbContext _db;
    private readonly IMapper _mapper;

    public SalesService(SalesAnalysisDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task UploadBulk(IEnumerable<SaleInputModel> sales)
    {
        var entities = sales.Select(o => new Sale
        {
            PharmacyId = o.PharmacyId,
            ProductId = o.ProductId,
            DistributorId = o.DistributorId,
            Date = o.Date,
            Count = o.Count,
            CreatedOn = DateTime.Now,
            IsDeleted = false
        }).ToList();

        await _db.BulkInsertAsync(entities);
    }

    public async Task<List<SalesCheckModel>> GetAll() 
        => await _mapper.ProjectTo<SalesCheckModel>(_db.Sales).ToListAsync();
    
}