using Microsoft.Data.SqlClient;

namespace BrandexBusinessSuite.Inventory.Services.Materials;

using System.Data;

using BrandexBusinessSuite.Inventory.Data.Enums;
using BrandexBusinessSuite.Inventory.Models.Materials;
using BrandexBusinessSuite.Models.ErpDocuments;
using AutoMapper;
using BrandexBusinessSuite.Inventory.Data;
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
            
        table.Columns.Add(Name);
        table.Columns.Add(ErpId);
        table.Columns.Add(PartNumber);
        table.Columns.Add(Type, typeof(int));
        table.Columns.Add(Measurement, typeof(int));
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));

        foreach (var product in products)
        {
            var row = table.NewRow();
            row[Name] = product.Name.BG;
            row[ErpId] = product.Id;
            row[PartNumber] = product.PartNumber;
            row[Type] = materialType;
            row[Measurement] = materialMeasurement;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
        
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
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