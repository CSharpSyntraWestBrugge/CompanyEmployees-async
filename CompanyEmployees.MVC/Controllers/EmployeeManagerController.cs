﻿using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.MVC.Controllers
{
    [Authorize(Roles="Manager")]
    public class EmployeeManagerController : Controller
    {
        private IRepositoryManager _repositoryManager;

        public EmployeeManagerController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var employees = await _repositoryManager.Employee.GetAllEmployeesAsync(false);
            return View(employees);
        }
        [AllowAnonymous]
        public IActionResult Details(Guid id)
        {
            var employee = _repositoryManager.Employee.GetEmployee(id,false);
            return View(employee);
        }
        private async Task FillCompaniesAsync()
        {
            List<SelectListItem> companies = (from c in await _repositoryManager.Company.GetAllCompaniesAsync(false)
                                              orderby c.Name
                                              select new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToList();
            ViewBag.Companies = companies;
        }
        public IActionResult Insert()
        {
            FillCompaniesAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insert(Employee model)
        {
            if (ModelState.IsValid)
            {
                _repositoryManager.Employee.CreateEmployeeForCompany(model.CompanyId, model);
                _repositoryManager.Save();
                ViewBag.Message = "Werknemer toegevoegd";
            }
            FillCompaniesAsync();
            return View(model);
        }

        public IActionResult Update(Guid id)
        {
            FillCompaniesAsync();

            Employee model = _repositoryManager.Employee.GetEmployee(id, false);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Employee model)
        {
            FillCompaniesAsync();

            if (ModelState.IsValid)
            {
                Employee employeeToUpdate = _repositoryManager.Employee.GetEmployee(model.Id, true);
                employeeToUpdate.Name = model.Name;
                employeeToUpdate.Position = model.Position;
                employeeToUpdate.Age = model.Age;
                employeeToUpdate.CompanyId = model.CompanyId;
                employeeToUpdate.Description = model.Description;
                employeeToUpdate.Gender = model.Gender;
                _repositoryManager.Save();
                ViewBag.Message = "Gegevens werknemer aangepast";
            }
            return View(model);
        }

        [ActionName("Delete")]
        public IActionResult ConfirmDelete(Guid id)
        {
            ViewBag.Message = "Opgelet: U gaat deze werknemer verwijderen!";
            Employee model = _repositoryManager.Employee.GetEmployee(id, false);
            Company company = _repositoryManager.Company.GetCompany(model.CompanyId, false);
            ViewBag.Company = company;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            Employee employeeToDelete = _repositoryManager.Employee.GetEmployee(id, false);
            _repositoryManager.Employee.DeleteEmployee(employeeToDelete);
            _repositoryManager.Save();
            TempData["Message"] = "Werknemer verwijderd";
            return RedirectToAction("Index");//nameof(Index) = "Index"
        }

    }
}
