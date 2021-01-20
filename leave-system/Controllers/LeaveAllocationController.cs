using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_system.Data;
using leave_system.Interfaces;
using leave_system.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace leave_system.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly ILeaveTypeRepo _leaverepo;
        private readonly ILeaveAllocationRepo _allocationrepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;


        public LeaveAllocationController(
            ILeaveTypeRepo leaverepo,
            ILeaveAllocationRepo allocationrepo,
            UserManager<Employee> userManager,
            IMapper mapper)
        {
            _leaverepo = leaverepo;
            _userManager = userManager;
            _allocationrepo = allocationrepo;
            _mapper = mapper;
        }
        public ActionResult Index()
        {
            var leavetypes = _leaverepo.FindAll().ToList();
            var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leavetypes);

            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };

            return View(model);
        }

        public ActionResult SetLeave(int id)
        {
            var leavetype = _leaverepo.FindById(id);
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            foreach (var emp in employees)
            {
                if(_allocationrepo.CheckAllocation(id, emp.Id))
                {
                    continue;
                }

                var allocation = new LeaveAllocationViewModel
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leavetype.DefaultDays,
                    Period = DateTime.Now.Year
                };
                var leaveallocation = _mapper.Map<LeaveAllocation>(allocation);
                _allocationrepo.Create(leaveallocation);            
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult ListEmployees()
        {
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var model = _mapper.Map<List<EmployeeViewModel>>(employees);
            return View(model);
        }

        // GET: LeaveAllocationController/Details/5
        public ActionResult Details(string id)
        {
            var employee = _mapper.Map<EmployeeViewModel>(_userManager.FindByIdAsync(id).Result);
            var allocations = _mapper.Map<List<LeaveAllocationViewModel>>(_allocationrepo.GetLeaveAllocationsByEmployee(id));
            var model = new ViewAllocationsViewModel
            {
                Employee = employee,
                LeaveAllocations = allocations
            };

            return View(model);
        }

        // GET: LeaveAllocationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocationController/Edit/5
        public ActionResult Edit(int id)
        {
            var allocation = _allocationrepo.FindById(id);
            var model = _mapper.Map<EditLeaveAllocationVM>(allocation);
            return View(model);
        }

        // POST: LeaveAllocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditLeaveAllocationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var record = _allocationrepo.FindById(model.Id);
                record.NumberOfDays = model.NumberOfDays;

                
                var isSuccess = _allocationrepo.Update(record);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return View(model);
                }
                return RedirectToAction(nameof(Details), new { id = model.EmployeeId});
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }
        }

        // GET: LeaveAllocationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
