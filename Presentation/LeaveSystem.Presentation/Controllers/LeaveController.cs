using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveSystem.Business;
using LeaveSystem.Business.Interfaces;
using LeaveSystem.Data.Model;
using LeaveSystem.Presentation.Controllers.Base;
using LeaveSystem.Presentation.Models.ManageLeaveViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeaveSystem.Presentation.Controllers
{
    [Authorize(Roles ="manager,employee")]
    public class LeaveController : BaseController
    {
        private readonly ILeaveManager leaveManager;
        private readonly IEmployeeManager employeeManager;
        public LeaveController(ILeaveManager leaveManager, IEmployeeManager employeeManager)
            : base(leaveManager, employeeManager)
        {
            this.leaveManager = leaveManager;
            this.employeeManager = employeeManager;
        }
        public IActionResult Index()
        {
            var leaves = leaveManager.GetLeaveByEmployeeId(currentEmployee.Id).ToList();
            var results = Mapper.Map<List<Leave>, List<LeaveViewModel>>(leaves);
            return View(results);
        }

        [HttpGet]
        public IActionResult AddLeave()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddLeave(CreateLeaveViewModel leaveViewModel)
        {
            if (ModelState.IsValid)
            {
                var leave = Mapper.Map<CreateLeaveViewModel, Leave>(leaveViewModel);
                leave.EmployeeId = currentEmployee.Id;
                var results = leaveManager.AddLeave(leave);
                if (results > 0)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("error", "Error when creating the leave");


            }
            return View(leaveViewModel);
        }
        [HttpPost]
        public IActionResult EditLeave(UpdateLeaveViewModel model)
        {
            if (ModelState.IsValid)
            {
                var leave = Mapper.Map<UpdateLeaveViewModel, Leave>(model);
                var results = leaveManager.UpdateLeave(leave);
                if (results > 0)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("error", "Error when Updating the leave");

            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditLeave(int leaveId)
        {
            //get the leave
            var leave = leaveManager.GetLeaveById(leaveId);
            //map leave
            var leaveModel = Mapper.Map<Leave, UpdateLeaveViewModel>(leave);
            return View(leaveModel);
        }

        [HttpGet]
        public IActionResult LeaveDetails(int leaveId)
        {
            var leave = leaveManager.GetLeaveById(leaveId);
            var leaveModel = Mapper.Map<Leave, LeaveViewModel>(leave);
            return View(leaveModel);
        }
    }
}