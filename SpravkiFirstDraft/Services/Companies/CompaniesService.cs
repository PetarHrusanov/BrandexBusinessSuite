namespace SpravkiFirstDraft.Services.Companies
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SpravkiFirstDraft.Data;
    using SpravkiFirstDraft.Data.Models;
    using SpravkiFirstDraft.Models.Companies;

    public class CompaniesService : ICompaniesService
    {
        SpravkiDbContext db;

        public CompaniesService(SpravkiDbContext db)
        {
            this.db = db;
        }

        public async Task<string> UploadCompany(CompanyInputModel company)
        {
            if(company.Name!= null)
            {
                var companyModel = new Company
                {
                    Name = company.Name,
                    VAT = company.VAT,
                    Owner = company.Owner
                };

                await this.db.Companies.AddAsync(companyModel);
                await this.db.SaveChangesAsync();
                return company.Name;
            }
            else
            {
                return "";
            }
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
    }
}
