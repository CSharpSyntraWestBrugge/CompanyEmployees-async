using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class EmployeeRepository:RepositoryBase<Employee>,IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }

        public IEnumerable<Employee> GetAllEmployees(bool trackChanges)
        {
            return FindAll(trackChanges).OrderBy(e => e.Age).ToList();
        }

        public Employee GetEmployee(Guid employeeId, bool trackChanges)
        {
            return FindByCondition(c => c.Id.Equals(employeeId), trackChanges)
            .SingleOrDefault();
        }
    }
}
