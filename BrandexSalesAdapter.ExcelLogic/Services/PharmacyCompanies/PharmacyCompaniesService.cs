﻿using Microsoft.Extensions.Configuration;

namespace BrandexSalesAdapter.ExcelLogic.Services.PharmacyCompanies
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Microsoft.EntityFrameworkCore;
    
    using Data;
    using Data.Models;
    
    using Models.PharmacyCompanies;
    using Microsoft.Data.SqlClient;
    
    using static Common.ExcelDataConstants.PharmacyCompaniesColumns;

    public class PharmacyCompaniesService : IPharmacyCompaniesService
    {
        SpravkiDbContext db;
        private readonly IConfiguration _configuration;

        public PharmacyCompaniesService(SpravkiDbContext db ,IConfiguration configuration)
        {
            this.db = db;
            _configuration = configuration;
        }
        
        public async Task UploadBulk(List<PharmacyCompanyInputModel> pharmacyCompanies)
        {
            var table = new DataTable();
            table.TableName = PharmacyCompanies;
            
            table.Columns.Add(Name, typeof(string));
            table.Columns.Add(Owner);
            table.Columns.Add(VAT);
            
            foreach (var pharmacyCompany in pharmacyCompanies)
            {
                var row = table.NewRow();
                row[Name] = pharmacyCompany.Name.ToUpper();
                row[Owner] = pharmacyCompany.Owner;
                row[VAT] = pharmacyCompany.VAT;
                table.Rows.Add(row);
            }

            string connection = _configuration.GetConnectionString("DefaultConnection");
            
            var con = new SqlConnection(connection);
            
            var objbulk = new SqlBulkCopy(con);  
            
            objbulk.DestinationTableName = PharmacyCompanies;
            
            objbulk.ColumnMappings.Add(Name, Name);
            objbulk.ColumnMappings.Add(Owner, Owner);
            objbulk.ColumnMappings.Add(VAT, VAT);

            con.Open();
            await objbulk.WriteToServerAsync(table);  
            con.Close();  
            
        }

        public async Task<string> UploadCompany(PharmacyCompanyInputModel company)
        {
            if (company.Name == null) return "";
            var companyModel = new Company
            {
                Name = company.Name,
                VAT = company.VAT,
                Owner = company.Owner
            };

            await db.Companies.AddAsync(companyModel);
            await db.SaveChangesAsync();
            return company.Name;
        }

        public async Task<bool> CheckCompanyByName(string companyName)
        {
            return await db.Companies.Where(x => x.Name.ToLower()
                                    .TrimEnd().Contains(companyName.ToLower().TrimEnd()))
                                    .Select(x => x.Id).AnyAsync();
        }

        public async Task<int> IdByName(string companyName)
        {
            int companyId = await db.Companies
                                   .Where(x => x.Name.ToLower()
                                   .TrimEnd().Contains(companyName.ToLower().TrimEnd()))
                                   .Select(x => x.Id).FirstOrDefaultAsync();
            return companyId;
        }

        public async Task<List<PharmacyCompanyCheckModel>> GetPharmacyCompaniesCheck()
        {
            return await db.Companies.Select(p => new PharmacyCompanyCheckModel()
            {
                Id = p.Id,
                Name = p.Name,
                VAT = p.VAT
            }).ToListAsync();
        }
    }
}
