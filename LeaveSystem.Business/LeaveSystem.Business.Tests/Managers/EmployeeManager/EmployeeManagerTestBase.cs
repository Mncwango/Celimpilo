using LeaveSystem.Data.Model;
using LeaveSystem.Data.Uow;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSystem.Business.Tests.Managers.EmployeeManager
{
    public class EmployeeManagerTestBase
    {
        public Mock<IUnitOfWork> uowMock;
        public Mock<IEmployeeManager> employeeManagerMock;
        public UserManager<Employee> _employeeManager;
        public RoleManager<Role> _roleManager;
        public Mock<IUserStore<Employee>> _userStoreMock;
        public Mock<IUserEmailStore<Employee>> _emailStoreMock;
        public Mock<IUserRoleStore<Employee>> _userRoleStoreMock;


        [SetUp]
        public void SetUp()
        {
            
            uowMock = new Mock<IUnitOfWork>();
            employeeManagerMock = new Mock<IEmployeeManager>();
            _userStoreMock = new Mock<IUserStore<Employee>>();
            _employeeManager = GetMockUserManager();
            _roleManager = GetMockRoleManager();
            _emailStoreMock = new Mock<IUserEmailStore<Employee>>();
            _userRoleStoreMock = new Mock<IUserRoleStore<Employee>>();
            //var services = new ServiceCollection();
            //services.AddEntityFramework()
            //    .AddInMemoryDatabase()
            //    .AddDbContext<MyDbContext>(options => options.UseInMemoryDatabase());
        }

        private UserManager<Employee> GetMockUserManager()
        {
            
            return new UserManager<Employee>(
                _userStoreMock.Object, null, null, null, null, null, null, null, null);
        }
        private RoleManager<Role> GetMockRoleManager()
        {
            var userStoreMock = new Mock<IRoleStore<Role>>();
            return new RoleManager<Role>(
                userStoreMock.Object, null, null, null,null);
        }
    }
}
