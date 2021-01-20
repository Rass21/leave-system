using AutoMapper;
using leave_system.Data;
using leave_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_system.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<LeaveType, LeaveTypeViewModel>().ReverseMap();
            CreateMap<LeaveAllocation, LeaveAllocationViewModel>().ReverseMap();
            CreateMap<LeaveHistory, LeaveHistoryViewModel>().ReverseMap();
            CreateMap<Employee, EmployeeViewModel>().ReverseMap();
            CreateMap<LeaveAllocation, EditLeaveAllocationVM>().ReverseMap();
        }
    }
}
