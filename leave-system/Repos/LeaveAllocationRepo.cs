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

        public bool CheckAllocation(int leavetypeId, string employeeId)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                .Where(
                x => x.EmployeeId == employeeId &&
                x.LeaveTypeId == leavetypeId &&
                x.Period == period)
                .Any();
        }

        public bool Create(LeaveAllocation entity)
        {
            _db.Add(entity);
            return Save();
        }

        public bool Delete(LeaveAllocation entity)
        {
            _db.Remove(entity);
            return Save();
        }

        public ICollection<LeaveAllocation> FindAll()
        {
            var leaveAllocations = _db.LeaveAllocations.Include(x => x.LeaveType).ToList();
            return leaveAllocations;
        }

        public LeaveAllocation FindById(int id)
        {
            var leaveAllocation = _db.LeaveAllocations
                .Include(i => i.LeaveType) //first including objects within the LeaveAllocation class
                .Include(i => i.Employee)
                .FirstOrDefault(x => x.Id == id); //and only after do we specify to get the LeaveAllocation by id
            return leaveAllocation;
        }

        public ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string id)
        {
            var period = DateTime.Now.Year;
            return FindAll().
                Where(x => x.EmployeeId == id && x.Period == period).
                ToList();
        }

        public bool isExists(int id)
        {
            var exists = _db.LeaveAllocations.Any(x => x.Id == id);

            return exists;
        }

        public bool Save()
        {
            var changes = _db.SaveChanges();
            return changes > 0;
        }

        public bool Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return Save();
        }
    }
}
