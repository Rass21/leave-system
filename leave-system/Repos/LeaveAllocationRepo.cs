using leave_system.Data;
using leave_system.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_system.Repos
{
    public class LeaveAllocationRepo : ILeaveAllocationRepo
    {
        private readonly ApplicationDbContext _db;
        public LeaveAllocationRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CheckAllocation(int leavetypeId, string employeeId)
        {
            var period = DateTime.Now.Year;
            var leaveAllocation = await FindAll();
            return leaveAllocation
                .Where(
                x => x.EmployeeId == employeeId &&
                x.LeaveTypeId == leavetypeId &&
                x.Period == period)
                .Any();
        }

        public async Task<bool> Create(LeaveAllocation entity)
        {
            await _db.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            _db.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveAllocation>> FindAll()
        {
            var leaveAllocations = await _db.LeaveAllocations
                .Include(x => x.LeaveType)
                .ToListAsync();
            return leaveAllocations;
        }

        public async Task<LeaveAllocation> FindById(int id)
        {
            var leaveAllocation = await _db.LeaveAllocations
                .Include(i => i.LeaveType) //first including objects within the LeaveAllocation class
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(x => x.Id == id); //and only after do we specify to get the LeaveAllocation by id
            return leaveAllocation;
        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(string employeeid)
        {
            var period = DateTime.Now.Year;
            var leaveAllocations = await FindAll();

            return leaveAllocations.Where(x => x.EmployeeId == employeeid && x.Period == period).ToList();
        }

        public async Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string employeeid, int leavetypeid)
        {
            var period = DateTime.Now.Year;
            var leaveAllocations = await FindAll();
            return  leaveAllocations.FirstOrDefault(x => x.EmployeeId == employeeid && x.Period == period && x.LeaveTypeId == leavetypeid);           
        }

        public async Task<bool> isExists(int id)
        {
            var exists = await _db.LeaveAllocations.AnyAsync(x => x.Id == id);

            return exists;
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return await Save();
        }
    }
}
