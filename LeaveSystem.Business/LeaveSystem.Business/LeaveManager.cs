using LeaveSystem.Business.Interfaces;
using LeaveSystem.Data.Model;
using LeaveSystem.Data.Uow;
using LeaveSystem.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LeaveSystem.Business
{
    public class LeaveManager : ILeaveManager
    {
        private readonly IUnitOfWork uow;
        public LeaveManager(IUnitOfWork unitOfWork)
        {
            uow = unitOfWork;
        }
        public IEnumerable<Leave> GetManagerEmployeesLeaves(string ManagerId)
        {
            List<Leave> employeeLeaves = new List<Leave>();
            //get managers employees id's
            var employees = uow.Employees.GetWhere(x => x.ManagerId == ManagerId)
                .Select(x => x.Id).ToList();
            employeeLeaves = uow.Leaves.GetAllIncluding(x => employees.Contains(x.EmployeeId), c => c.LeaveStatus).ToList();
            return employeeLeaves;
        }

        public IEnumerable<Leave> GetLeaveByEmployeeId(string EmployeeId)
        {
            return uow.Leaves
                .GetAllIncluding(x => x.EmployeeId == EmployeeId, c => c.LeaveStatus)
                .ToList();
        }
        public int AddLeave(Leave leave)
        {
            leave.StatusId = (int)LeaveStatusEnum.Pending;
            uow.Leaves.Add(leave);
            return uow.Save();
        }

        public int UpdateLeave(int leaveId, LeaveStatusEnum leaveStatusEnum)
        {
            var leaveToUpdate = GetLeaveById(leaveId);
            leaveToUpdate.StatusId = (int)leaveStatusEnum;
            uow.Leaves.Update(leaveToUpdate);
            return uow.Save();
        }
        public int UpdateLeave(Leave leave)
        {
            uow.Leaves.Update(leave);
            return uow.Save();
        }
        public Leave GetLeaveById(int leaveId)
        {
            return uow.Leaves.GetById(leaveId);

        }
    }
}
