using leave_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_system.Data;
using Microsoft.EntityFrameworkCore;

namespace leave_system.Repos
{
    public class LeaveRequestRepo : ILeaveRequestRepo
    {
        private readonly ApplicationDbContext _db;
        public LeaveRequestRepo(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool Create(LeaveRequest entity)
        {
            _db.Add(entity);
            return Save();

        }

        public bool Delete(LeaveRequest entity)
        {
            _db.Remove(entity);
            return Save();
        }

        public ICollection<LeaveRequest> FindAll()
        {
            var leaveRequests = 
                _db.LeaveRequests
                .Include(x => x.RequestingEmployee)
                .Include(x => x.ApprovedBy)
                .Include(x => x.LeaveType)
                .ToList();
            return leaveRequests;
        }

        public LeaveRequest FindById(int id)
        {
            var leaveRequest = _db.LeaveRequests
                .Include(x => x.RequestingEmployee)
                .Include(x => x.ApprovedBy)
                .Include(x => x.LeaveType)
                .FirstOrDefault(x => x.Id == id);
            return leaveRequest;
        }

        public ICollection<LeaveRequest> GetLeaveRequestsByEmployeeId(string employeeid)
        {
            var leaveRequests =
                FindAll()
                .Where(x => x.RequestingEmployeeId == employeeid)
                .ToList();
            return leaveRequests;
        }

        public bool isExists(int id)
        {
            var exists = _db.LeaveRequests.Any(x => x.Id == id);

            return exists;
        }

        public bool Save()
        {
            var changes = _db.SaveChanges();
            return changes > 0;
        }

        public bool Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return Save();
        }
    }
}
