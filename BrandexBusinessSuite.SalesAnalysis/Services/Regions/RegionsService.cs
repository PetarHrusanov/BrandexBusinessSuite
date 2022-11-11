using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BrandexBusinessSuite.SalesAnalysis.Services.Regions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.Models.DataModels;
using Data;
using SalesAnalysis.Data.Models;

using static Methods.DataMethods;

public class RegionsService : IRegionsService
{
    private readonly SalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;

    public RegionsService(SalesAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<string> UploadRegion(string regionName)
    {
        if (regionName == null) return "";
        var regionDbModel = new Region
        {
            Name = regionName
        };

        await _db.Regions.AddAsync(regionDbModel);
        await _db.SaveChangesAsync();
        return regionName;

    }

    public async Task<List<BasicCheckErpModel>> AllRegions() 
        =>await _db.Regions.Select(a => new BasicCheckErpModel
        {
            Id = a.Id,
            ErpId = a.ErpId,
            Name = a.Name
        }).ToListAsync();

    public async Task BulkUpdateData(List<BasicCheckErpModel> list)
    {
        var dt = ConvertToDataTable(list);
        
        var connection = _configuration.GetConnectionString("DefaultConnection");

        await using var conn = new SqlConnection(connection);
        await using var command = new SqlCommand("CREATE TABLE #TmpTable(Id smallint NOT NULL,ErpId nvarchar(50) NOT NULL, Name nvarchar(50) NOT NULL)", conn);
        try
        {
            conn.Open();
            command.ExecuteNonQuery();

            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.BulkCopyTimeout = 6600;
                bulkCopy.DestinationTableName = "#TmpTable";
                await bulkCopy.WriteToServerAsync(dt);
                bulkCopy.Close();
            }

            command.CommandTimeout = 3000;
            command.CommandText = "UPDATE P SET P.[ErpId]= T.[ErpId] FROM [Regions] AS P INNER JOIN #TmpTable AS T ON P.[Id] = T.[Id] ;DROP TABLE #TmpTable;";
            command.ExecuteNonQuery();
        }
        catch (Exception)
        {
            // Handle exception properly
        }
        finally
        {
            conn.Close();
        }
    }
}