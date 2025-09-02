using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DTO;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompanyService(IRepositoryManager repositoryManager, ILoggerManager logger,IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<CompanyDto>,MetaData)> GetAllCompanies(CompanyRequestParameters companyParameters,bool trackChanges)
        {
            var companies = await _repositoryManager.Company.GetCompanies(companyParameters,trackChanges);
            var companyDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            _logger.LogInfo($"Returned all companies from database.");
            return (companyDto,companies.MetaData);
        }

        public async Task<CompanyDto> GetCompany(Guid id, bool trackChanges)
        {
            var company = await GetCompanyAndCheckExistence(id, trackChanges);
            if (company is null)
            {
                _logger.LogInfo($"Invalid Id for a company");
                throw new CompanyNotFoundException(id);
            }
            var companyDto = _mapper.Map<CompanyDto>(company);
            _logger.LogInfo($"Returned company with id: {id} from database.");
            return companyDto;
        }
        public async Task<CompanyDto> CreateCompany(CompanyForCreationDto companyForCreation)
        {
            var company = _mapper.Map<Company>(companyForCreation);
            _repositoryManager.Company.CreateCompany(company);
            await _repositoryManager.SaveAsync();
            _logger.LogInfo($"Created company with id: {company.Id}.");
            return _mapper.Map<CompanyDto>(company);
        }
        public async Task DeleteCompany(Guid id, bool trackChanges)
        {
            var company=await GetCompanyAndCheckExistence(id,trackChanges);
            if (company is null) {
                _logger.LogInfo($"Invalid Id for a company");
                throw new CompanyNotFoundException(id);
            }
            _repositoryManager.Company.DeleteCompany(company);
            await _repositoryManager.SaveAsync();
            _logger.LogInfo($"Company with id {company.Id} has been deleted successfully.");
        }

        public async Task UpdateCompany(Guid companyid, CompanyForUpdateDto companyForUpdate, bool trackChanges)
        {
            var company = await GetCompanyAndCheckExistence(companyid, trackChanges);
            _mapper.Map(companyForUpdate, company);
            await _repositoryManager.SaveAsync();
            _logger.LogInfo($"Company with id {company.Id} has been updated successfully.");
        }
        private async Task<Company>GetCompanyAndCheckExistence(Guid companyId, bool trackChanges)
        {
            var company= await _repositoryManager.Company.GetCompanyById(companyId, trackChanges);
            if (company is null)
            {
                _logger.LogInfo($"Invalid Id for a company");
                throw new CompanyNotFoundException(companyId);
            }
            return company;
        }
    }
}
