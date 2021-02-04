using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_system.Models
{
    public class LeaveRequestViewModel
    {
        public int Id { get; set; }

        public EmployeeViewModel RequestingEmployee { get; set; }
        [Display(Name ="Employee Name")]
        public string RequestingEmployeeId { get; set; }
        [Required]
        [Display(Name ="Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public LeaveTypeViewModel LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
        [Display(Name ="Date of Request")]
        public DateTime DateRequested { get; set; }
        [Display(Name ="Date Actioned")]
        public DateTime DateActioned { get; set; }
        [Display(Name ="Approval Status")]
        public bool? Approved { get; set; }

        public EmployeeViewModel ApprovedBy { get; set; }
        public string ApprovedById { get; set; }
        public bool Cancelled { get; set; }
    }

    public class AdminLeaveRequestViewModel
    {
        [Display(Name = "Total Requests")]
        public int TotalRequests { get; set; }
        [Display(Name = "Approved Requests")]
        public int ApprovedRequests { get; set; }
        [Display(Name = "Pending Requests")]
        public int PendingRequests { get; set; }
        [Display(Name = "Rejected Requests")]
        public int RejectedRequests { get; set; }
        public List<LeaveRequestViewModel> LeaveRequests { get; set; }

    }

    public class CreateLeaveRequestViewModel
    {
        [Required]
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public string EndDate { get; set; }
        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
        [Display(Name ="Leave Type")]
        public int LeaveTypeId { get; set; }
        public string Comment { get; set; }
    }

    public class MyLeaveViewModel
    {
        public List<LeaveRequestViewModel> LeaveRequests { get; set; }
        public List<LeaveAllocationViewModel> LeaveAllocations { get; set; }
    }
}
