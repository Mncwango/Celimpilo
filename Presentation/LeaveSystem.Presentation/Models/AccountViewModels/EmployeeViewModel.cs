﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveSystem.Presentation.Models.AccountViewModels
{
    public class EmployeeViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellPhoneNumber { get; set; }
        public string Email { get; set; }
        public string ManagerId { get; set; }
    }
}
