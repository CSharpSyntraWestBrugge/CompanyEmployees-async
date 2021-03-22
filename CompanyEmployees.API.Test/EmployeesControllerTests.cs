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
    public class EmployeesControllerTests
    {
        private Mock<IRepositoryManager> mockRepo;
        private static IMapper _mapper;

        public EmployeesControllerTests()
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
            mockRepo.Setup(repo => repo.Employee.GetAllEmployeesAsync(false).Result)
                .Returns(SeedTestData.GetTestEmployees());
        }
        [Test]
        public void GetEmployees_ReturnsOkResult_WithListOfEmployees()
        { 
            // Arrange

            var controller = new EmployeesController( mockRepo.Object,_mapper);

            // Act
            var result = controller.GetEmployees();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<IEnumerable<EmployeeDto>>(okResult.Value);
            var items = okResult.Value as IEnumerable<EmployeeDto>;
            Assert.AreEqual(2, items.Count());
            //bv eerste  van de testEmployees testen op Id:
            Assert.IsTrue(items.Where(emp => emp.Id == SeedTestData.GetTestEmployee().Id).Any());
        }
        [Test]
        public void GetEmployee_ForEmployeeId_ReturnsEmployeeDto()
        {
            //Arrange
            Employee testEmployee = SeedTestData.GetTestEmployee();
            Guid testEmployeeId = testEmployee.Id;
            mockRepo.Setup(r => r.Employee.GetEmployee(testEmployeeId, It.IsAny<bool>())).Returns(testEmployee);
            EmployeesController controller = new EmployeesController(mockRepo.Object,_mapper); //EmployeesController is sut(System Under Test)

            //Act
            var result = controller.GetEmployee(testEmployeeId);
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            OkObjectResult okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.TypeOf<EmployeeDto>());
            EmployeeDto employeeDto = okResult.Value as EmployeeDto; //of EmployeeDto employeeDto = (EmployeeDto)viewResult.Value;
            Assert.AreEqual(testEmployeeId, employeeDto.Id);
            Assert.AreEqual(testEmployee.Name, employeeDto.Name);
            Assert.AreEqual(testEmployee.Age, employeeDto.Age);
        }

        [Test]
        public void CreateEmployeeForCompany_ValidInputCreatesEmployeeyAndReturnsAnOkResult_WithAnEmployee()
        {
            // Arrange
            var existingCompany = SeedTestData.GetTestCompany();
            Guid companyId = existingCompany.Id;
            var testEmployee = SeedTestData.GetTestEmployee();
            mockRepo.Setup(repo => repo.Company.GetCompany(companyId, false)).Returns(existingCompany).Verifiable();
            mockRepo.Setup(repo => repo.Employee.CreateEmployeeForCompany(companyId,It.IsAny<Employee>()))
                .Verifiable();
            var controller = new EmployeesController(mockRepo.Object,_mapper);
            EmployeeForCreationDto newEmployeeDto = new EmployeeForCreationDto()
            {
                Name = testEmployee.Name,
                Age = testEmployee.Age,
                Position = testEmployee.Position
            };
            // Act
            var result = controller.CreateEmployeeForCompany(companyId, newEmployeeDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.TypeOf<Employee>());
            Employee resultEmp = okResult.Value as Employee;
            Assert.AreEqual(resultEmp.Name, newEmployeeDto.Name);
            Assert.AreEqual(resultEmp.Age, newEmployeeDto.Age);
            Assert.AreEqual(resultEmp.Position, newEmployeeDto.Position);
            mockRepo.Verify(repo => repo.Company.GetCompany(companyId, false), Times.Once);
            mockRepo.Verify();
        }
        [Test]
        public void CreateEmployeeForCompany__InValidInputNull_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var existingCompany = SeedTestData.GetTestCompany();
            Guid companyId = existingCompany.Id;
            var controller = new EmployeesController(mockRepo.Object, _mapper);
            EmployeeForCreationDto newEmployeeDto = null;
            // Act
            var result = controller.CreateEmployeeForCompany(companyId,newEmployeeDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            mockRepo.Verify(repo => repo.Employee.CreateEmployeeForCompany(companyId,It.IsAny<Employee>()), Times.Never);
        }
        [Test]
        public void DeleteEmployee_ForExistingId_RemovesEmployeeAndReturnsNoContentResult()
        {
            // Arrange
            var testDeleteEmployee = SeedTestData.GetTestEmployee();
            mockRepo.Setup(repo => repo.Employee.GetEmployee(testDeleteEmployee.Id, false))
                .Returns(testDeleteEmployee).Verifiable();

            var controller = new EmployeesController(mockRepo.Object,_mapper);

            // Act
            var result = controller.DeleteEmployee(testDeleteEmployee.Id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            mockRepo.Verify(repo => repo.Employee.GetEmployee(testDeleteEmployee.Id, false), Times.Once);
        }
        [Test]
        public void DeleteEmployee_ForUnExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var unExistingEmployeeId = Guid.NewGuid();
            Employee testInvalidEmployee = null;
            mockRepo.Setup(repo => repo.Employee.GetEmployee(unExistingEmployeeId, false))
                .Returns(testInvalidEmployee).Verifiable();

            var controller = new EmployeesController(mockRepo.Object, _mapper);

            // Act
            var result = controller.DeleteEmployee(unExistingEmployeeId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            mockRepo.Verify(repo => repo.Employee.GetEmployee(unExistingEmployeeId, false), Times.Once);
        }
        [Test]
        public void UpdateEmployee_ValidInput_ReturnsOkObjectResult_WithUpdatedEmployee()
        {
            // Arrange 
            Employee employeeToUpdate = SeedTestData.GetTestEmployee();
            employeeToUpdate.Name = "gewijzigde naam";
            employeeToUpdate.Age = 25;
            employeeToUpdate.Position = "gewijzigde positie";

            EmployeeForUpdateDto employeeForUpdateDto = new EmployeeForUpdateDto
            {
                 Name = employeeToUpdate.Name,
                 Age = employeeToUpdate.Age,
                 Position = employeeToUpdate.Position,
                
            };

            mockRepo.Setup(repo => repo.Employee.GetEmployee(employeeToUpdate.Id, true))
                .Returns(employeeToUpdate).Verifiable();
            mockRepo.Setup(repo => repo.Save()).Verifiable();

            var controller = new EmployeesController(mockRepo.Object, _mapper);
            
            //Act
            var result = controller.UpdateEmployee(employeeToUpdate.Id, employeeForUpdateDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsInstanceOf<Employee>(okResult.Value);
            var updatedEmployee = okResult.Value as Employee;
            Assert.AreEqual(employeeToUpdate.Name, updatedEmployee.Name);
            Assert.AreEqual(employeeToUpdate.Age, updatedEmployee.Age);
            Assert.AreEqual(employeeToUpdate.Position, updatedEmployee.Position);
            mockRepo.Verify(repo => repo.Employee.GetEmployee(updatedEmployee.Id, true),Times.Once);
            mockRepo.Verify(repo => repo.Save(), Times.Once);
        }
        [Test]
        public void UpdateEmployee_InValidInputNull_ReturnsBadRequestObjectResult()
        {
            // Arrange 
            Employee employeeToUpdate = SeedTestData.GetTestEmployee();

            EmployeeForUpdateDto employeeForUpdateDto = null;

            var controller = new EmployeesController(mockRepo.Object, _mapper);
            // Act
            var result = controller.UpdateEmployee(employeeToUpdate.Id, employeeForUpdateDto) ;

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            mockRepo.Verify(repo => repo.Employee.GetEmployee(employeeToUpdate.Id, true), Times.Never);
            mockRepo.Verify(repo => repo.Save(), Times.Never);
        }
        [Test]
        public void UpdateEmployee_UnExistingEmployeeId_ReturnsNotFoundResult()
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
