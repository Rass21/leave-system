using leave_system.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_system.Interfaces
{
    public interface ILeaveAllocationRepo : IRepoBase<LeaveAllocation>
    {
        bool CheckAllocation(int leavetypeId, string employeeId);
        ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string employeeid);
        LeaveAllocation GetLeaveAllocationsByEmployeeAndType(string employeeid, int leavetypeid);
    }
}
