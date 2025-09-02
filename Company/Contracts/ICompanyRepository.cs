using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts
{
    public interface ICompanyRepository
    {
        Task<PagedList<Company>> GetCompanies(CompanyRequestParameters companyParameters, bool trackChanges);
        Task<Company> GetCompanyById(Guid id,bool trackChanges);
        void CreateCompany(Company company);
        void DeleteCompany(Company company);
    }
}
