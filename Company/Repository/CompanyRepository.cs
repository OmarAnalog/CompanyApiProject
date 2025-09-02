using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(Context context) : base(context)
        {
        }
        public async Task<PagedList<Company>> GetCompanies(CompanyRequestParameters companyParameters,bool trackChanges)
        {
            var companies=await FindAll(trackChanges)
           .OrderBy(c => c.Name)
           .Skip((companyParameters.PageNumber - 1) * companyParameters.PageSize)
           .Take(companyParameters.PageSize)
           .ToListAsync();
            var count= await FindAll(trackChanges)
           .CountAsync();
            return new PagedList<Company>(companies,count, companyParameters.PageNumber, companyParameters.PageSize);
        }

        public async Task<Company> GetCompanyById(Guid id, bool trackChanges)
        => await FindByCondition(c => c.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();
        public void CreateCompany(Company company) =>  Create(company);
        public void DeleteCompany(Company company) => Delete(company);
    }
}
