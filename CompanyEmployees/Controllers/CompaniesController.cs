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
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private IRepositoryManager _repositoryManager;
        private IMapper _mapper;
        public CompaniesController(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            var companies = _repositoryManager.Company.GetAllCompanies(false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            //var companiesDto = companies.Select(c => new CompanyDto
            //{ 
            //    Id = c.Id,
            //    Name = c.Name,
            //    FullAddress = c.Address + " " + c.Country               
            //});
            return Ok(companiesDto);
        }
        [HttpGet("{id}")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _repositoryManager.Company.GetCompany(id, false);
            if (company == null)
            {
                return NotFound();//404 not found status boodschap (geen gegevens)
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto); //200 OK status boodschap met de gegevens
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteCompany(Guid id)
        {
            var company = _repositoryManager.Company.GetCompany(id, trackChanges: false);
            if (company == null)
            {
                //_logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repositoryManager.Company.DeleteCompany(company);
            _repositoryManager.Save();
            return NoContent();
        }
        [HttpPost]
        public IActionResult CreateCompany([FromBody] CompanyForCreationDto companyDto)
        {
            if (companyDto == null)
            {
                BadRequest("CompanyDto object is empty");
            }
            //To do: het companyDto object omzetten naar een company object
            //2 manieren mogelijk
            //1) Ofwel manueel code schrijven:
            //Company company = new Company()
            //{
            //    Name = companyDto.Name,
            //    Address = companyDto.Address,
            //    Country = companyDto.Country
            //};
            //2) Ofwel Automapper
            Company company = _mapper.Map<Company>(companyDto);
            _repositoryManager.Company.CreateCompany(company);
            _repositoryManager.Save();
            return Ok(company);    //200 met data van nieuw gecreëerde company
        }
        [HttpPut("{companyId}")]
        public IActionResult UpdateCompany(Guid companyId, [FromBody] CompanyForUpdateDto companyDto)
        {
            if (companyDto == null)
            {
                 return BadRequest("CompanyForUpdateDto object is null");
            }
            var companyEntity = _repositoryManager.Company.GetCompany(companyId,
                trackChanges: true);
            if (companyEntity == null)
            {
                return NotFound();
            }
            companyEntity.Name = companyDto.Name;
            companyEntity.Address = companyDto.Address;
            companyEntity.Country = companyDto.Country;
            //_mapper.Map(companyDto, companyEntity);
            _repositoryManager.Save();
            return Ok(companyEntity);
        }
    }
}