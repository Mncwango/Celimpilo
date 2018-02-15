using LeaveSystem.Data.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LeaveSystem.Business
{
    public interface IEmployeeManager
    {
        //Task<IdentityResult> CreateEmployee(Employee employee, string password);
        //Task<Employee> GetEmployeeById(Guid employeeId);
        //Task<Employee> GetEmployeeByEmail(string email);
        //Task<IdentityResult> ResetPasswordAsync(Employee employee,string token, string password);
        //Task<Employee> GetEmployeeAsync(ClaimsPrincipal employee);
        Task<bool> HasPasswordAsync(Employee employee);
        //Task<IdentityResult> SetEmployeePassword(Employee employee, string password);
        //Task<IdentityResult> ChangeEmployeePassword(Employee employee, string oldPassword, string newPassword);
        //Task<IdentityResult> DeleteEmployee(Employee employee);
        //Task<IdentityResult> AddToRole(Employee employee, string role);

        #region New Functions
        Task<bool> CheckPasswordAsync(Employee user, string password);
        Task<Tuple<bool, string[]>> CreateRoleAsync(Role role, IEnumerable<string> claims);
        Task<Tuple<bool, string[]>> CreateUserAsync(Employee user, IEnumerable<string> roles, string password);
        Task<Tuple<bool, string[]>> DeleteRoleAsync(Role role);
        Task<Tuple<bool, string[]>> DeleteRoleAsync(string roleName);
        Task<Tuple<bool, string[]>> DeleteUserAsync(Employee user);
        Task<Tuple<bool, string[]>> DeleteUserAsync(string userId);
        Task<Role> GetRoleByIdAsync(string roleId);
        Task<Role> GetRoleByNameAsync(string roleName);
        Role GetRoleLoadRelatedAsync(string roleName);
        List<Role> GetRolesLoadRelatedAsync(int page, int pageSize);
        Tuple<Employee, string[]> GetUserAndRolesAsync(string userId);
        Employee GetEmployeeByEmail(string email);
        Task<Employee> GetUserByIdAsync(string userId);
        Task<Employee> GetUserByUserNameAsync(string userName);
        Task<IList<string>> GetUserRolesAsync(Employee user);
        List<Tuple<Employee, string[]>> GetUsersAndRolesAsync(int page, int pageSize);
        Task<Tuple<bool, string[]>> ResetPasswordAsync(Employee user, string newPassword);
        Task<Tuple<bool, string[]>> UpdatePasswordAsync(Employee user, string currentPassword, string newPassword);
        Task<Tuple<bool, string[]>> UpdateRoleAsync(Role role, IEnumerable<string> claims);
        Task<Tuple<bool, string[]>> UpdateUserAsync(Employee user);
        Task<Tuple<bool, string[]>> UpdateUserAsync(Employee user, IEnumerable<string> roles);

        #endregion

    }
}
