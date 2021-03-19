using CompanyEmployees.MVC.Controllers;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.MVC.Test
{
    public class EmployeeManagerControllerTests
    {
        private Mock<IRepositoryManager> mockRepo;
        [SetUp]
        public void Initialize()
        {
            mockRepo = new Mock<IRepositoryManager>();
            mockRepo.Setup(repo => repo.Company.GetAllCompanies(false))
                .Returns(SeedTestData.GetTestCompanies());
        }
        [Test]
        public void Index_ReturnsAViewResult_WithListOfEmployees()
        {
            //Arrange
            mockRepo.Setup(repo => repo.Employee.GetAllEmployees(false))
                .Returns(SeedTestData.GetTestEmployees());
            var controller = new EmployeeManagerController(mockRepo.Object);

            //Act
            var result = controller.Index();

            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
            ViewResult viewResult = result as ViewResult; //casting, kan ook via (ViewResult)result
            Assert.IsAssignableFrom<List<Employee>>(viewResult.ViewData.Model);
            List<Employee> model = viewResult.ViewData.Model as List<Employee>;
            Assert.AreEqual(2, model.Count());
            //bv eerste  van de testEmployees testen op Id:
            Assert.IsTrue(model.Where(emp => emp.Id == SeedTestData.GetTestEmployee().Id).Any());
        }

        [Test]
        public void Details_ForEmployeeId_ReturnsEmployee()
        {
            //Arrange
            Employee testEmployee = SeedTestData.GetTestEmployee();
            Guid testEmployeeId = testEmployee.Id;
            mockRepo.Setup(e => e.Employee.GetEmployee(testEmployeeId,It.IsAny<bool>())).Returns(testEmployee);
            EmployeeManagerController controller = new EmployeeManagerController(mockRepo.Object);
            //Act
            var result = controller.Details(testEmployeeId);
            //Assert
            Assert.IsInstanceOf<ViewResult>(result); //Klassiek assertions model van Nunit: testen op type
            ViewResult viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.TypeOf<Employee>());//Constraint assertions model van Nunit: testen op type
            //of: Assert.IsInstanceOf<Employee>(viewResult.Model);     //Klassiek assertions model van Nunit: testen op type
            Employee employee = (Employee)viewResult.Model; //of casting via 'as' operator: viewResult.Model as Employee
            Assert.AreEqual(testEmployeeId, employee.Id);
            //Nog wat extra testjes voor de andere Employee Properties:
            Assert.AreEqual(testEmployee.Name, employee.Name);
            Assert.AreEqual(testEmployee.Description, employee.Description);
            Assert.AreEqual(testEmployee.Age, employee.Age);
            Assert.AreEqual(testEmployee.CompanyId, employee.CompanyId);
            Assert.AreEqual(testEmployee.Position, employee.Position);
        }
        [Test]
        public void Insert_InsertsEmployeeAndReturnsAViewResult_WithAnEmployee()
        {
            //Arrange
            mockRepo.Setup(repo => repo.Employee.CreateEmployeeForCompany(It.IsAny<Guid>(), It.IsAny<Employee>()))
                .Verifiable();//om te kunnen testen of de methode CreateEmployeeForCompany wordt aangeroepen
            var controller = new EmployeeManagerController(mockRepo.Object);
            var newEmployee = SeedTestData.GetTestEmployee();

            //Act
            var result = controller.Insert(newEmployee);
            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.AreEqual(viewResult.Model, newEmployee);
            mockRepo.Verify();//Hier wordt geverifieerd om de CreateEmployeeForCompany wordt aangeroepen, indien ja, is de test ok
        }
        [Test]
        public void Delete_SetsMessageAndReturnsRedirectToActionResult()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var testDeleteEmployee = SeedTestData.GetTestEmployee();
            mockRepo.Setup(repo => repo.Employee.GetEmployee(testDeleteEmployee.Id, false))
                .Returns(testDeleteEmployee);
            var controller = new EmployeeManagerController(mockRepo.Object) { TempData = tempData};
            //Act
            var result = controller.Delete(testDeleteEmployee.Id);
            //Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual(tempData["Message"], "Werknemer verwijderd");
        }
        [Test]
        public void Update_Employee_ReturnViewResult_WithEmployee()
        {
            //Arrange
            Employee employeeToUpdate = SeedTestData.GetTestEmployee();
            Guid employeeToUpdateId = employeeToUpdate.Id;
            mockRepo.Setup(repo => repo.Employee.GetEmployee(employeeToUpdateId, true))
                .Returns(employeeToUpdate);
            mockRepo.Setup(repo => repo.Save()).Verifiable();
            var controller = new EmployeeManagerController(mockRepo.Object);
            employeeToUpdate.Name = "gewijzigde naam";
            //Act
            var result = controller.Update(employeeToUpdate);
            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
            ViewResult viewResult = result as ViewResult;
            Assert.AreEqual(employeeToUpdate, viewResult.Model);
            mockRepo.Verify();
        }
    }
}
