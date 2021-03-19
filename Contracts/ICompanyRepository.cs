using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);
        Company GetCompany(Guid companyId, bool trackChanges);
        void DeleteCompany(Company company);
        void CreateCompany(Company company);

    }
}
