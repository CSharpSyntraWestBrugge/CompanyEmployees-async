using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.MVC.Test
{
    public class RepositoryManagerTests
    {

        #region EmployeesRepoTests//EMPLOYEES TESTS 

        [Test]
        public void GetAllEmployees_ShouldReturnAllEmployeesFromContext()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var countEmployeesInDb = context.Employees.Count();

                    var repository = new RepositoryManager(context);
                    var emp = repository.Employee.GetAllEmployees(false);
                    Assert.IsNotNull(emp);
                    Assert.AreEqual(countEmployeesInDb, emp.Count());
                }
            }
        }

        [Test]
        public void GetEmployee_ShouldReturnEmployee()
        {
            //Arrange
            Guid testEmployeeId;
            using (var factory = new TestRepositoryContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var testEmployee = context.Employees.FirstOrDefault();
                    testEmployeeId = testEmployee.Id;
                    var repository = new RepositoryManager(context);
                    //Act
                    var empl = repository.Employee.GetEmployee(testEmployeeId, false);
                    Assert.IsNotNull(empl);
                    Assert.AreEqual(testEmployeeId, empl.Id);
                    Assert.AreEqual(testEmployee.Name, empl.Name);
                    Assert.AreEqual(testEmployee.Age, empl.Age);
                    Assert.AreEqual(testEmployee.Description, empl.Description);
                    Assert.AreEqual(testEmployee.Gender, empl.Gender);
                    Assert.AreEqual(testEmployee.Position, empl.Position);
                    Assert.AreEqual(testEmployee.CompanyId, empl.CompanyId);
                }
            }
        }
        [Test]
        public void CreateEmployeeForExistingCompany_ShouldAddNewEmployeeToContextForCompany()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                //Arrange
                int count = 0;

                Company testCompany = null;
                Guid testEmployeeId = Guid.NewGuid();
                Employee testEmployee = new Employee()
                {
                    Id = testEmployeeId,
                    Name = "Jos",
                    Description = "Test employee",
                    Age = 45,
                    Gender = GeslachtType.Man,
                    Position = "Developer"
                };
                using (var context = factory.CreateContext())
                {
                    count = context.Employees.Count();
                    testCompany = context.Companies.FirstOrDefault();
                    testEmployee.CompanyId = testCompany.Id;
                    var repository = new RepositoryManager(context);
                    //Act
                    repository.Employee.CreateEmployeeForCompany(testCompany.Id, testEmployee);
                    repository.Save();
                }
                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.AreEqual(count + 1, context.Employees.Count());
                    var addedEmployee = context.Employees.Find(testEmployeeId);
                    Assert.IsNotNull(addedEmployee);
                    Assert.AreEqual(testEmployeeId, addedEmployee.Id);
                    Assert.AreEqual(testEmployee.Name, addedEmployee.Name);
                    Assert.AreEqual(testEmployee.Age, addedEmployee.Age);
                    Assert.AreEqual(testEmployee.Description, addedEmployee.Description);
                    Assert.AreEqual(testEmployee.Gender, addedEmployee.Gender);
                    Assert.AreEqual(testEmployee.Position, addedEmployee.Position);
                    Assert.AreEqual(testEmployee.CompanyId, addedEmployee.CompanyId);
                }
            }
        }

        [Test]
        public void SaveChangesGetEmployeeTrackChangesTrue_ShouldChangeEmployeeInContext()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                //Arrange          
                Guid testCompanyId;
                Guid testEmployeeId;
                Employee testEmployee;
               
                using (var context = factory.CreateContext())
                {
                    var repository = new RepositoryManager(context);
                    var firstCompany = context.Companies.FirstOrDefault();
                    testCompanyId = firstCompany.Id;
                    var firstEmployee = context.Employees.FirstOrDefault();
                    testEmployeeId = firstEmployee.Id;
                    //Act
                    testEmployee = repository.Employee.GetEmployee(testEmployeeId, true);

                    testEmployee.Name = "gewijzigde naam Joke";
                    testEmployee.Age = 18;
                    testEmployee.CompanyId =testCompanyId;
                    testEmployee.Description = "gewijzigde beschrijving";
                    testEmployee.Gender = GeslachtType.Vrouw;
                    testEmployee.Position = "gewijzigde positie";

                    repository.Save();
                }
                //Assert
                using (var context = factory.CreateContext())
                {
                    var changedEmployee = context.Employees.FirstOrDefault(e => e.Id == testEmployeeId);
                    Assert.IsNotNull(changedEmployee);
                    Assert.AreEqual(testEmployee.Id, changedEmployee.Id);
                    Assert.AreEqual(testEmployee.Name, changedEmployee.Name);
                    Assert.AreEqual(testEmployee.Age, changedEmployee.Age);
                    Assert.AreEqual(testEmployee.CompanyId, changedEmployee.CompanyId);
                    Assert.AreEqual(testEmployee.Description, changedEmployee.Description);
                    Assert.AreEqual(testEmployee.Gender, changedEmployee.Gender);
                    Assert.AreEqual(testEmployee.Position, changedEmployee.Position);
                }
            }
        }
        [Test]
        public void DeleteEmployee_ShouldRemoveEmployeeFromContext()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                //Arrange          
                Guid testEmployeeId;
                int count;
                using (var context = factory.CreateContext())
                {
                    count = context.Employees.Count();
                    var repository = new RepositoryManager(context);
                    var firstEmployee = context.Employees.FirstOrDefault();
                    testEmployeeId = firstEmployee.Id;
                    //Act
                    repository.Employee.DeleteEmployee(firstEmployee);

                    repository.Save();
                }
                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.AreEqual(count - 1, context.Employees.Count());
                    Assert.IsFalse(context.Employees.Where(c => c.Id == testEmployeeId).Any());
                }
            }
        }
        #endregion //EMPLOYEES TESTS
        #region CompaniesRepoTests//COMPANIES TESTS
        [Test]
        public void GetAllCompanies_ShouldReturnAllCompaniesFromContext()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var countCompaniesInDb = context.Companies.Count();

                    var repository = new RepositoryManager(context);
                    var comp = repository.Company.GetAllCompanies(false);
                    Assert.IsNotNull(comp);
                    Assert.AreEqual(countCompaniesInDb, comp.Count());
                }
            }
        }
        [Test]
        public void GetCompany_ShouldReturnCompany()
        {
            //Arrange
            Guid testCompanyId;
            using (var factory = new TestRepositoryContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var testCompany = context.Companies.FirstOrDefault();
                    testCompanyId = testCompany.Id;
                    var repository = new RepositoryManager(context);
                    //Act
                    var comp = repository.Company.GetCompany(testCompanyId, false);
                    //Assert
                    Assert.IsNotNull(comp);
                    Assert.AreEqual(testCompanyId, comp.Id);
                    //Aanvullen: andere properties ook testen op gelijkheid
                    Assert.AreEqual(testCompany.Name, comp.Name);
                    Assert.AreEqual(testCompany.LaunchDate, comp.LaunchDate);
                    Assert.AreEqual(testCompany.Size, comp.Size);
                    Assert.AreEqual(testCompany.Address, comp.Address);
                    Assert.AreEqual(testCompany.Country, comp.Country);
                    Assert.AreEqual(testCompany.Description, comp.Description);
                }
            }
        }
        [Test]
        public void CreateCompany_ShouldAddNewCompanyToContext()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                //Arrange
                int count = 0;
                Guid testCompanyId;
                Company testCompany;//Hier de declaratie van testCompany plaatsen
                using (var context = factory.CreateContext())
                {
                    testCompanyId = Guid.NewGuid();
                    testCompany = new Company() //Verwijder Company declaratie, hier initialiseren
                    {
                        Id = testCompanyId,
                        Name = "Test bedrijf",
                        Country = "Test land",
                        Description = "Test beschrijving",
                        Size = CompanySize.Small,
                        LaunchDate = DateTime.Today,
                        Address = "Test adres"
                    };
                    count = context.Companies.Count();
                    var repository = new RepositoryManager(context);
                    //Act
                    repository.Company.CreateCompany(testCompany);
                    repository.Save();
                }
                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.AreEqual(count + 1, context.Companies.Count());
                    var addedCompany = context.Companies.FirstOrDefault(e => e.Id == testCompanyId);
                    Assert.IsNotNull(addedCompany);
                    Assert.AreEqual(testCompanyId, addedCompany.Id);
                    //Aanvullen: andere properties ook testen op gelijkheid
                    Assert.AreEqual(testCompany.Name, addedCompany.Name);
                    Assert.AreEqual(testCompany.LaunchDate, addedCompany.LaunchDate);
                    Assert.AreEqual(testCompany.Size, addedCompany.Size);
                    Assert.AreEqual(testCompany.Address, addedCompany.Address);
                    Assert.AreEqual(testCompany.Country, addedCompany.Country);
                    Assert.AreEqual(testCompany.Description, addedCompany.Description);
                }
            }
        }
        [Test]
        public void SaveChangesGetCompanyTrackChangesTrue_ShouldChangeCompanyInContext()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                //Arrange          
                Guid testCompanyId;
                Company testCompany;
                using (var context = factory.CreateContext())
                {
                    var repository = new RepositoryManager(context);
                    var firstCompany = context.Companies.FirstOrDefault();
                    testCompanyId = firstCompany.Id;
                    //Act
                    testCompany = repository.Company.GetCompany(testCompanyId, true);
                    testCompany.Name = "gewijzigde naam";
                    testCompany.Size = CompanySize.Big;
                    testCompany.LaunchDate = new DateTime(2021, 3, 15);
                    testCompany.Description = "gewijzigde beschrijving";
                    testCompany.Country = "gewijsigd land";
                    testCompany.Address = "gewijzigd adres";

                    repository.Save();
                }
                //Assert
                using (var context = factory.CreateContext())
                {
                    var changedCompany = context.Companies.Include(c => c.Employees).FirstOrDefault(e => e.Id == testCompanyId);
                    Assert.IsNotNull(changedCompany);
                    Assert.AreEqual(testCompany.Id, changedCompany.Id);
                    Assert.AreEqual(testCompany.Name, changedCompany.Name);
                    Assert.AreEqual(testCompany.Size, changedCompany.Size);
                    Assert.AreEqual(testCompany.LaunchDate, changedCompany.LaunchDate);
                    Assert.AreEqual(testCompany.Description, changedCompany.Description);
                    Assert.AreEqual(testCompany.Country, changedCompany.Country);
                    Assert.AreEqual(testCompany.Address, changedCompany.Address);
                }
            }

        }
        [Test]
        public void DeleteCompany_ShouldRemoveCompanyFromContext()
        { 
            using (var factory = new TestRepositoryContextFactory())
            {
                //Arrange          
                Guid testCompanyId;
                int count;
                using (var context = factory.CreateContext())
                {
                    count = context.Companies.Count();
                    var repository = new RepositoryManager(context);
                    var firstCompany = context.Companies.FirstOrDefault();
                    testCompanyId = firstCompany.Id;
                    //Act
                    repository.Company.DeleteCompany(firstCompany);

                    repository.Save();
                }
                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.AreEqual(count-1, context.Companies.Count());
                    Assert.IsFalse(context.Companies.Where(c => c.Id == testCompanyId).Any());
                }
            }
        }
        #endregion //COMPANIES TESTS
    }
}

