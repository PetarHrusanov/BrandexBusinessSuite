namespace SpravkiFirstDraft.Services.Companies
{
    using System;
    using System.Threading.Tasks;
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

        public async Task<bool> UploadCompany(CompanyInputModel company)
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
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
