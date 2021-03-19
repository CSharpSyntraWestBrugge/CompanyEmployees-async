using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IRepositoryManager _repositoryManager;
        private IMapper _mapper;
        public EmployeesController(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetEmployees()
        {
            var employees = _repositoryManager.Employee.GetAllEmployees(false);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            //var employeesDto = employees.Select(c => new EmployeeDto
            //{
            //    Id = c.Id,
            //    Name = c.Name,
            //    Age = c.Age
            //});
            return Ok(employeesDto);
        }
        [HttpGet("{id}")]
        public IActionResult GetEmployee(Guid id)
        {
            var employee = _repositoryManager.Employee.GetEmployee(id, trackChanges: false);
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                var employeeDto = _mapper.Map<EmployeeDto>(employee);
                return Ok(employeeDto);
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(Guid id)
        {
            //eerst employee opzoeken aan de hand van id
            var employee = _repositoryManager.Employee.GetEmployee(id, false);
            if (employee == null)
            {
                NotFound(); // status 404 - not found zonder gegevens teruggeven
            }
            _repositoryManager.Employee.DeleteEmployee(employee);
            _repositoryManager.Save();
            return NoContent();
        }
        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId,
            [FromBody] EmployeeForCreationDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("employeeDto object is null");
            }
            var company = _repositoryManager.Company.GetCompany(companyId, false);
            if (company == null)
            {
                return NotFound();
            }
            var employee = _mapper.Map<Employee>(employeeDto);
            _repositoryManager.Employee.CreateEmployeeForCompany(companyId, employee);
            _repositoryManager.Save();
            return Ok(employee);
        }
        [HttpPut("{employeeId}")]
        public IActionResult UpdateEmployee(Guid employeeId, [FromBody] EmployeeForUpdateDto employeeDto)
        {
            if (employeeDto == null)
            {
                BadRequest("EmployeeForUpdateDto is empty");
            }
            var employee = _repositoryManager.Employee.GetEmployee(employeeId, true);
            if (employee == null)
            {
                NotFound(); //404
            }
            employee.Name = employeeDto.Name;
            employee.Age = employeeDto.Age;
            employee.Position = employeeDto.Position;
            //ofwel met automapper://eerst aan MappingProfile toevoegen: CreateMap<EmployeeForUpdateDto, Employee>();//101
            // _mapper.Map(employeeDto, employee);
            _repositoryManager.Save();
            return Ok(employee);
        }
    }
    
}
