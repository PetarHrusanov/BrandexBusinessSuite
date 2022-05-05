namespace BrandexSalesAdapter.ExcelLogic.Services.PharmacyChains
{
    
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Data;
    
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Data;

    using Models.PharmacyChains;
    using Microsoft.Data.SqlClient;
    
    using Data.Models;
    
    using static Common.ExcelDataConstants.PharmacyChainsColumns;


    public class PharmacyChainsService : IPharmacyChainsService
    {
        SpravkiDbContext db;
        private readonly IConfiguration _configuration;

        public PharmacyChainsService(SpravkiDbContext db , IConfiguration configuration)
        {
            this.db = db;
            _configuration = configuration;
        }
        
        public async Task UploadBulk(List<string> pharmacyChains)
        {
            var table = new DataTable();
            table.TableName = PharmacyChains;
            
            table.Columns.Add(Name, typeof(string));
            
            foreach (var pharmacyChain in pharmacyChains)
            {
                var row = table.NewRow();
                row[Name] = pharmacyChain;
                table.Rows.Add(row);
            }

            string connection = _configuration.GetConnectionString("DefaultConnection");
            
            var con = new SqlConnection(connection);
            
            var objbulk = new SqlBulkCopy(con);  
            
            objbulk.DestinationTableName = PharmacyChains;
            
            objbulk.ColumnMappings.Add(Name, Name);

            con.Open();
            await objbulk.WriteToServerAsync(table);  
            con.Close();  
            
        }

        public async Task<string> UploadPharmacyChain(string chainName)
        {
            // if (chainName == null) return "";
            var chainInput = new PharmacyChain
            {
                Name = chainName
            };

            await db.PharmacyChains.AddAsync(chainInput);
            await db.SaveChangesAsync();
            return chainName;

        }

        public async Task<bool> CheckPharmacyChainByName(string pharmacyChainName)
        {
            return await db.PharmacyChains.Where(x => x.Name.ToLower().TrimEnd() == pharmacyChainName.ToLower().TrimEnd())
                                    .Select(x => x.Id).AnyAsync();
        }

        public async Task<int> IdByName(string pharmacyChainName)
        {
            return await db.PharmacyChains.Where(x => x.Name.ToLower().TrimEnd() == pharmacyChainName.ToLower().TrimEnd())
                                    .Select(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<List<PharmacyChainCheckModel>> GetPharmacyChainsCheck()
        {
            return await db.PharmacyChains.Select(p => new PharmacyChainCheckModel()
            {
                Id = p.Id,
                Name = p.Name
            }).ToListAsync();
        }
    }
}
