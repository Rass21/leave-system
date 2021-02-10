using leave_system.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_system.Interfaces
{
    public interface ILeaveRequestRepo : IRepoBase<LeaveRequest>
    {
        Task<ICollection<LeaveRequest>> GetLeaveRequestsByEmployeeId(string employeeid);
    }
}
