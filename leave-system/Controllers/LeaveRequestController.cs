using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_system.Data;
using leave_system.Interfaces;
using leave_system.Models;
using leave_system.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace leave_system.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepo _leaverequestrepo;
        private readonly ILeaveTypeRepo _leavetyperepo;
        private readonly ILeaveAllocationRepo _leaveallocationrepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;
        // GET: LeaveRequestController
        public LeaveRequestController(
            ILeaveRequestRepo leaverequestrepo,
            ILeaveTypeRepo leavetyperepo,
            ILeaveAllocationRepo leaveallocationrepo,
            UserManager<Employee> userManager,
            IMapper mapper)
        {
            _leaverequestrepo = leaverequestrepo;
            _mapper = mapper;
            _userManager = userManager;
            _leavetyperepo = leavetyperepo;
            _leaveallocationrepo = leaveallocationrepo;
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var requests = _leaverequestrepo.FindAll();
            var leaveRequests = _mapper.Map<List<LeaveRequestViewModel>>(requests);

            var model = new AdminLeaveRequestViewModel
            {
                TotalRequests = leaveRequests.Count(),
                ApprovedRequests = leaveRequests.Count(x => x.Approved == true),
                RejectedRequests = leaveRequests.Count(x => x.Approved == false), //This does the same thing as Where().Count() below
                PendingRequests = leaveRequests.Where(x => x.Approved == null).Count(),
                LeaveRequests = leaveRequests
            };

            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Details(int id)
        {
            var model = _mapper.Map<LeaveRequestViewModel>(_leaverequestrepo.FindById(id));

            return View(model);
        }

        public ActionResult MyLeave()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var userId = user.Id;
            var requests = _leaverequestrepo.GetLeaveRequestsByEmployeeId(userId).ToList();
            var allocations = _leaveallocationrepo.GetLeaveAllocationsByEmployee(userId).ToList();

            var leaveRequests = _mapper.Map<List<LeaveRequestViewModel>>(requests);
            var leaveAllocations = _mapper.Map<List<LeaveAllocationViewModel>>(allocations);

            var model = new MyLeaveViewModel
            {
                LeaveRequests = leaveRequests,
                LeaveAllocations = leaveAllocations
            };

            return View(model);

        }

        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var request = _leaverequestrepo.FindById(id);
                var employeeId = request.RequestingEmployeeId;
                var leaveTypeId = request.LeaveTypeId;
                var allocation = _leaveallocationrepo.GetLeaveAllocationsByEmployeeAndType(employeeId, leaveTypeId);
                int daysRequested = (int)(request.EndDate - request.StartDate).TotalDays;

                allocation.NumberOfDays -= daysRequested;

                request.Approved = true;
                request.ApprovedById = user.Id;
                request.DateActioned = DateTime.Now;

                _leaverequestrepo.Update(request);
                _leaveallocationrepo.Update(allocation);

                return RedirectToAction(nameof(Index), "LeaveRequest");

            }
            catch (Exception ex)
            {

                return RedirectToAction(nameof(Index), "LeaveRequest");
            }
        }

        public ActionResult RejectRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var request = _leaverequestrepo.FindById(id);

                request.Approved = false;
                request.ApprovedById = user.Id;
                request.DateActioned = DateTime.Now;

                var isSuccess = _leaverequestrepo.Update(request);

                return RedirectToAction(nameof(Index), "LeaveRequest");

            }
            catch (Exception ex)
            {

                return RedirectToAction(nameof(Index), "LeaveRequest");
            }
        }

        public ActionResult CancelRequest(int id)
        {
            var leaveRequest = _leaverequestrepo.FindById(id);
            leaveRequest.Cancelled = true;
            _leaverequestrepo.Update(leaveRequest);
            return RedirectToAction("MyLeave");
        }

        // GET: LeaveRequestController/Create
        public ActionResult Create()
        {
            var leaveTypes = _leavetyperepo.FindAll();
            var leaveTypeItems = leaveTypes.Select(x => new SelectListItem  //assigns each value (x) in leaveTypes to a SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            var model = new CreateLeaveRequestViewModel
            {
                LeaveTypes = leaveTypeItems
            };

            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestViewModel model)
        {
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate);
                var leaveTypes = _leavetyperepo.FindAll();
                var leaveTypeItems = leaveTypes.Select(x => new SelectListItem  //assigns each value (x) in leaveTypes to a SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                model.LeaveTypes = leaveTypeItems;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (DateTime.Compare(startDate, endDate) > 1)
                {
                    ModelState.AddModelError("", "Start date has to be before or the same date as the end date.");
                    return View(model);
                }

                var employee = _userManager.GetUserAsync(User).Result;
                var allocation = _leaveallocationrepo.GetLeaveAllocationsByEmployeeAndType(employee.Id, model.LeaveTypeId);
                int daysRequested = (int)(endDate - startDate).TotalDays;

                if(daysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "Days requested exceeds available leave days.");
                    return View(model);
                }

                var leaveRequestModel = new LeaveRequestViewModel
                {
                    RequestingEmployeeId = employee.Id,
                    LeaveTypeId = model.LeaveTypeId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now
                };

                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                var isSuccess = _leaverequestrepo.Create(leaveRequest);

                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return View(model);
                }

                return RedirectToAction(nameof(Index), "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
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
