using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveSystem.Business;
using LeaveSystem.Business.Interfaces;
using LeaveSystem.Data.Model;
using LeaveSystem.Data.Uow;
using LeaveSystem.Infrastructure;
using LeaveSystem.Presentation.Controllers.Base;
using LeaveSystem.Presentation.Models.ManageLeaveViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveSystem.Presentation.Controllers
{
    [Authorize(Roles = "administrator,manager")]
    public class ManageLeaveController : BaseController
    {
        private readonly ILeaveManager leaveManager;
        private readonly IEmployeeManager _employeeManager;
        public ManageLeaveController(ILeaveManager leaveManager, IEmployeeManager employeeManager)
            : base(leaveManager,employeeManager)
        {
            this.leaveManager = leaveManager;
            _employeeManager = employeeManager;
        }

        public IActionResult Index()
        {
            var leaves = leaveManager.GetLeaveByEmployeeId(currentEmployee.Id).ToList();
            var results = Mapper.Map<List<Leave>, List<LeaveViewModel>>(leaves);
            return View(results);
        }
        public IActionResult MyLeaves()
        {
            return View();
        }
        public IActionResult EmployeesLeave()
        {
            var leaves = leaveManager.GetManagerEmployeesLeaves(currentEmployee.Id).ToList();
            var results = Mapper.Map<List<Leave>, List<LeaveViewModel>>(leaves);
            return View(results);
        }
        public IActionResult LeaveDetails(int LeaveId)
        {
            return View();
        }
        [HttpGet]
        public IActionResult UpdateLeave(int leaveId,LeaveStatusEnum leaveStatusEnum)
        {
            leaveManager.UpdateLeaveStatus(leaveId, leaveStatusEnum);
            return RedirectToAction("EmployeesLeave");
        }
        public IActionResult RejectLeave(LeaveViewModel leave)
        {
            return View();
        }
    }
}