using LeaveSystem.Data.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LeaveSystem.Business;
using System.Threading.Tasks;

namespace LeaveSystem.Business.Tests.Managers.EmployeeManager
{
    [TestFixture]
    public class EmployeeManagerTests : EmployeeManagerTestBase
    {
        private string id
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
        private Employee GetMockEmployee()
        {
            return new Employee
            {
                Id = id,
                UserName = "admin",
                FirstName = "Inbuilt",
                LastName = "Administrator",
                Email = "admin@company1.com",
                PhoneNumber = "+1 (123) 000-0000",
                CellPhoneNumber = "+1 (123) 000-0000",
                EmailConfirmed = true,
                IsEnabled = true,
                CreatedDate = DateTime.Now.Date
            };
        }
        private List<Employee> GetListOfMockedEmployees()
        {
            return new List<Employee>()
            {
                     new Employee
                     {
                       UserName = "admin",
                       FirstName = "Inbuilt",
                       LastName = "Administrator",
                       Email = "admin@company1.com",
                       PhoneNumber = "+1 (123) 000-0000",
                       CellPhoneNumber = "+1 (123) 000-0000",
                       EmailConfirmed = true,
                       IsEnabled = true,
                       CreatedDate = DateTime.Now.Date
                    },
                  new Employee
                  {
                   UserName = "employee",
                   FirstName = "Inbuilt",
                   LastName = "Administrator",
                   Email = "employee@company1.com",
                   PhoneNumber = "+1 (123) 000-0000",
                   CellPhoneNumber = "+1 (123) 000-0000",
                   EmailConfirmed = true,
                   IsEnabled = true,
                   CreatedDate = DateTime.Now.Date
                  }
            };
        }

        [Test()]
        public void ShouldGetEmployeeByGivenEmail()
        {

            List<Employee> employeesList = new List<Employee>()
            {
                     new Employee
                     {
                       UserName = "admin",
                       FirstName = "Inbuilt",
                       LastName = "Administrator",
                       Email = "admin@company1.com",
                       PhoneNumber = "+1 (123) 000-0000",
                       CellPhoneNumber = "+1 (123) 000-0000",
                       EmailConfirmed = true,
                       IsEnabled = true,
                       CreatedDate = DateTime.Now.Date
                    },
                  new Employee
                  {
                   UserName = "employee",
                   FirstName = "Inbuilt",
                   LastName = "Administrator",
                   Email = "employee@company1.com",
                   PhoneNumber = "+1 (123) 000-0000",
                   CellPhoneNumber = "+1 (123) 000-0000",
                   EmailConfirmed = true,
                   IsEnabled = true,
                   CreatedDate = DateTime.Now.Date
                  }
            };
            var expectedEmployee = new Employee
            {
                UserName = "admin",
                FirstName = "Inbuilt",
                LastName = "Administrator",
                Email = "admin@company1.com",
                PhoneNumber = "+1 (123) 000-0000",
                CellPhoneNumber = "+1 (123) 000-0000",
                EmailConfirmed = true,
                IsEnabled = true,
                CreatedDate = DateTime.Now.Date
            };

            //_emailStoreMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.FromResult(employee));
            uowMock.Setup(x => x.Employees.GetWhere(It.IsAny<Expression<Func<Employee, bool>>>())).Returns(employeesList.AsQueryable());

            var manager = new LeaveSystem.Business.EmployeeManager(_employeeManager, _roleManager, uowMock.Object);
            var results = manager.GetEmployeeByEmail("admin@company1.com");
            Assert.That(expectedEmployee.Email, Is.EqualTo(results.Email));
        }
        [Test]
        public void ShouldReturnNullEmployeeWhenGivenEmailDoesNotExist()
        {
            uowMock.Setup(x => x.Employees.GetWhere(It.IsAny<Expression<Func<Employee, bool>>>())).Returns(new List<Employee>().AsQueryable());
            var manager = new LeaveSystem.Business.EmployeeManager(_employeeManager, _roleManager, uowMock.Object);
            var results = manager.GetEmployeeByEmail("admin@company1.com");
            Assert.IsNull(results);
        }
        [Test]
        public async Task ShouldFindFindEmployeeByGivenIdAsync()
        {
            var employee = new Employee
            {
                Id = id,
                UserName = "admin",
                FirstName = "Inbuilt",
                LastName = "Administrator",
                Email = "admin@company1.com",
                PhoneNumber = "+1 (123) 000-0000",
                CellPhoneNumber = "+1 (123) 000-0000",
                EmailConfirmed = true,
                IsEnabled = true,
                CreatedDate = DateTime.Now.Date
            };

            _userStoreMock.Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.FromResult<Employee>(employee));
            var manager = new LeaveSystem.Business.EmployeeManager(_employeeManager, _roleManager, uowMock.Object);
            var results = await manager.GetUserByIdAsync(id);
            Assert.AreEqual(employee, results);

        }

        [Test]
        public async Task ShouldGetEmployeeByGivenUserNameAsync()
        {
            var expectedEmployee = GetMockEmployee();
            _userStoreMock.Setup(x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
                .Returns(Task.FromResult(expectedEmployee));
            var manager = new LeaveSystem.Business.EmployeeManager(_employeeManager, _roleManager, uowMock.Object);
            var results =await manager.GetUserByUserNameAsync("admin@company1.com");
            Assert.AreEqual(expectedEmployee, results);
            
        }
        [Test]
        public async Task ShouldGetRolesForGivenEmployee()
        {
            //IList<string> roles = new List<string> {"administrator" };
            //_userRoleStoreMock.Setup(x => x.GetRolesAsync(It.IsAny<Employee>(), It.IsAny<System.Threading.CancellationToken>()))
            //    .Returns(Task.FromResult(roles));
            //var manager = new LeaveSystem.Business.EmployeeManager(_employeeManager, _roleManager, uowMock.Object);
            //var results = await manager.GetUserRolesAsync(GetMockEmployee());
            //Assert.AreEqual(roles, results);
        }
    }
}
