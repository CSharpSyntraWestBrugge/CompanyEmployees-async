using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(bool trackChanges);
        Employee GetEmployee(Guid employeeId, bool trackChanges);
        void DeleteEmployee(Employee employee);
        void CreateEmployeeForCompany(Guid companyId, Employee employee);

    }
}
