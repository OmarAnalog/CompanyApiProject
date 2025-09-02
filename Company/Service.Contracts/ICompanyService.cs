using Entities.Models;
using Shared.DTO;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        Task<(IEnumerable<CompanyDto>,MetaData)>GetAllCompanies(CompanyRequestParameters companyParameters,bool trackChanges);
        Task<CompanyDto> GetCompany(Guid id,bool trackChanges);
        Task<CompanyDto> CreateCompany(CompanyForCreationDto companyDto);
        Task DeleteCompany(Guid id,bool trackChanges);
        Task UpdateCompany(Guid companyid, CompanyForUpdateDto companyForUpdate, bool
        trackChanges);
    }
}
