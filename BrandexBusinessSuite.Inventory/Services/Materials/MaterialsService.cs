using Microsoft.Data.SqlClient;

namespace BrandexBusinessSuite.Inventory.Services.Materials;

using System.Data;

using Data.Enums;
using BrandexBusinessSuite.Inventory.Models.Materials;
using BrandexBusinessSuite.Models.ErpDocuments;
using AutoMapper;
using Data;
using Microsoft.EntityFrameworkCore;

using static  Common.Constants;
using static  Common.ExcelDataConstants.Generic;

public class MaterialsService : IMaterialsService
{
    private readonly InventoryDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private const string Materials = "Materials";
    private const string Type = "Type";
    private const string Measurement = "Measurement";

    public MaterialsService(InventoryDbContext db, IConfiguration configuration, IMapper mapper)
    {
        _db = db;
        _configuration = configuration;
        _mapper = mapper;
    }
    
    public async Task<List<MaterialOutputModel>> GetAll()
        => await _mapper.ProjectTo<MaterialOutputModel>(_db.Materials).ToListAsync();

    public async Task UploadBulk(List<ErpProduct> products, MaterialType materialType, MaterialMeasurement materialMeasurement)
    {
        var table = new DataTable();
        table.TableName = Materials;
        
        var dataColumns = new DataColumn[]
        {
            new (Name),
            new (ErpId),
            new (PartNumber),
            new (Type, typeof(int)),
            new (Measurement, typeof(int)),
            new (CreatedOn),
            new (IsDeleted, typeof(bool)),
        };
        
        table.Columns.AddRange(dataColumns);

        foreach (var values in products.Select(product => new object[] { product.Name.BG.TrimEnd(), product.Id, product.PartNumber, 
                     materialType,materialMeasurement, DateTime.Now, false }))
        {
            table.LoadDataRow(values, true);
        }

        await using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var objbulk = new SqlBulkCopy(con);
        objbulk.DestinationTableName = Materials;

        objbulk.ColumnMappings.Add(Name, Name);
        objbulk.ColumnMappings.Add(ErpId, ErpId);
        objbulk.ColumnMappings.Add(PartNumber, PartNumber);
        objbulk.ColumnMappings.Add(Type, Type);
        objbulk.ColumnMappings.Add(Measurement, Measurement);
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);
        con.Close(); 
    }
}