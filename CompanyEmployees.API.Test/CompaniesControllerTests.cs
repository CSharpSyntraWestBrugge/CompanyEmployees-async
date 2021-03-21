using AutoMapper;
using CompanyEmployees.Controllers;
using CompanyEmployees.Mapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanyEmployees.API.Test
{
    public class CompanyControllerTests
    {
        private Mock<IRepositoryManager> mockRepo;
        private static IMapper _mapper;

        public CompanyControllerTests()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }
        }
        [SetUp]
        public void Initialize()
        {
            mockRepo = new Mock<IRepositoryManager>();
            mockRepo.Setup(repo => repo.Company.GetAllCompaniesAsync(false).Result)
                .Returns(SeedTestData.GetTestCompanies());
        }
        [Test]
        public void GetCompanies_ReturnsOkResult_WithListOfCompanies()
        { 
            // Arrange

            var controller = new CompaniesController( mockRepo.Object,_mapper);

            // Act
            var result = controller.GetCompanies();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<IEnumerable<CompanyDto>>(okResult.Value);
            var items = okResult.Value as IEnumerable<CompanyDto>;
            Assert.AreEqual(2, items.Count());
            //bv eerste  van de testCompanies testen op Id:
            Assert.IsTrue(items.Where(comp => comp.Id == SeedTestData.GetTestCompany().Id).Any());
        }
        [Test]
        public void GetCompany_ForCompanyId_ReturnsCompanyDto()
        {
            //Arrange
            Company testCompany = SeedTestData.GetTestCompany();
            Guid testCompanyId = testCompany.Id;
            mockRepo.Setup(r => r.Company.GetCompany(testCompanyId, It.IsAny<bool>())).Returns(testCompany);
            CompaniesController controller = new CompaniesController(mockRepo.Object,_mapper); //CompaniesController is sut(System Under Test)

            //Act
            var result = controller.GetCompany(testCompanyId);
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            OkObjectResult okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.TypeOf<CompanyDto>());
            CompanyDto companyDto = okResult.Value as CompanyDto; //of CompanyDto company = (CompanyDto)viewResult.Value;
            Assert.AreEqual(testCompanyId, companyDto.Id);
            Assert.AreEqual(testCompany.Name, companyDto.Name);
            Assert.AreEqual(testCompany.Address + " " + testCompany.Country, companyDto.FullAddress);
        }

        [Test]
        public void CreateCompany_ValidInputCreatesCompanyAndReturnsAnOkResult_WithAnCompany()
        {
            // Arrange
            var newCompany = SeedTestData.GetTestCompany();
            mockRepo.Setup(repo => repo.Company.CreateCompany(It.IsAny<Company>()))
                .Verifiable();
            var controller = new CompaniesController(mockRepo.Object,_mapper);
            CompanyForCreationDto newCompanyDto = new CompanyForCreationDto()
            {
                Name = newCompany.Name,
                Address = newCompany.Address,
                Country = newCompany.Country
            };
            // Act
            var result = controller.CreateCompany(newCompanyDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.TypeOf<Company>());
            Company resultComp = okResult.Value as Company;
            Assert.AreEqual(resultComp.Name, newCompany.Name);
            Assert.AreEqual(resultComp.Address, newCompany.Address);
            Assert.AreEqual(resultComp.Country, newCompany.Country);
            mockRepo.Verify();
        }
        [Test]
        public void CreateCompany_InValidInputNull_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var controller = new CompaniesController(mockRepo.Object, _mapper);
            CompanyForCreationDto newCompanyDto = null;
            // Act
            var result = controller.CreateCompany(newCompanyDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            mockRepo.Verify(repo => repo.Company.CreateCompany(It.IsAny<Company>()), Times.Never);
        }
        [Test]
        public void DeleteCompany_ForExistingId_RemovesCompanyAndReturnsNoContentResult()
        {
            // Arrange
            var testDeleteCompany = SeedTestData.GetTestCompany();
            mockRepo.Setup(repo => repo.Company.GetCompany(testDeleteCompany.Id, false))
                .Returns(testDeleteCompany).Verifiable();

            var controller = new CompaniesController(mockRepo.Object,_mapper);

            // Act
            var result = controller.DeleteCompany(testDeleteCompany.Id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            mockRepo.Verify(repo => repo.Company.GetCompany(testDeleteCompany.Id, false), Times.Once);
        }
        [Test]
        public void DeleteCompany_ForUnExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var unExistingCompanyId = Guid.NewGuid();
            Company testInvalidComp = null;
            mockRepo.Setup(repo => repo.Company.GetCompany(unExistingCompanyId, false))
                .Returns(testInvalidComp).Verifiable();

            var controller = new CompaniesController(mockRepo.Object, _mapper);

            // Act
            var result = controller.DeleteCompany(unExistingCompanyId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            mockRepo.Verify(repo => repo.Company.GetCompany(unExistingCompanyId, false), Times.Once);
        }
        [Test]
        public void UpdateCompany_ValidInput_ReturnsOkObjectResult_WithUpdatedCompany()
        {
            // Arrange 
            Company companyToUpdate = SeedTestData.GetTestCompany();
            companyToUpdate.Name = "gewijzigde naam";
            companyToUpdate.Address = "gewijzigd adres";
            companyToUpdate.Country = "gewijzigd land";
            CompanyForUpdateDto companyForUpdateDto = new CompanyForUpdateDto
            {
                 Name = companyToUpdate.Name,
                 Address = companyToUpdate.Address,
                 Country = companyToUpdate.Country
            };

            mockRepo.Setup(repo => repo.Company.GetCompany(companyToUpdate.Id, true))
                .Returns(companyToUpdate).Verifiable();
            mockRepo.Setup(repo => repo.Save()).Verifiable();

            var controller = new CompaniesController(mockRepo.Object, _mapper);
            
            //Act
            var result = controller.UpdateCompany(companyToUpdate.Id, companyForUpdateDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsInstanceOf<Company>(okResult.Value);
            var updatedCompany = okResult.Value as Company;
            Assert.AreEqual(companyToUpdate.Name, updatedCompany.Name);
            Assert.AreEqual(companyToUpdate.Address, updatedCompany.Address);
            Assert.AreEqual(companyToUpdate.Country, updatedCompany.Country);
            mockRepo.Verify(repo => repo.Company.GetCompany(companyToUpdate.Id, true),Times.Once);
            mockRepo.Verify(repo => repo.Save(), Times.Once);
        }
        [Test]
        public void UpdateCompany_InValidInputNull_ReturnsBadRequestObjectResult()
        {
            // Arrange 
            Company companyToUpdate = SeedTestData.GetTestCompany();

            CompanyForUpdateDto companyForUpdateDto = null;

            var controller = new CompaniesController(mockRepo.Object, _mapper);
            // Act
            var result = controller.UpdateCompany(companyToUpdate.Id, companyForUpdateDto) ;

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            mockRepo.Verify(repo => repo.Company.GetCompany(companyToUpdate.Id, true), Times.Never);
            mockRepo.Verify(repo => repo.Save(), Times.Never);
        }
        [Test]
        public void UpdateCompany_UnExistingCompanyId_ReturnsNotFoundResult()
        {
            // Arrange 
            Company companyToUpdate = SeedTestData.GetTestCompany();

            CompanyForUpdateDto companyForUpdateDto = null;

            var controller = new CompaniesController(mockRepo.Object, _mapper);
            // Act
            var result = controller.UpdateCompany(companyToUpdate.Id, companyForUpdateDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            mockRepo.Verify(repo => repo.Company.GetCompany(companyToUpdate.Id, true), Times.Never);
            mockRepo.Verify(repo => repo.Save(), Times.Never);
        }
    }
}
