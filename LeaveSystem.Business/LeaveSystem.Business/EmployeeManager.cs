using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LeaveSystem.Data;
using LeaveSystem.Data.Model;
using LeaveSystem.Data.Uow;
using LeaveSystem.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeaveSystem.Business
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly UserManager<Employee> _employeeManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeManager(UserManager<Employee> employeeManager, RoleManager<Role> roleManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _employeeManager = employeeManager;
            _roleManager = roleManager;

        }


        //public async Task<IdentityResult> ChangeEmployeePassword(Employee employee, string oldPassword, string newPassword)
        //{
        //    return await _employeeManager.ChangePasswordAsync(employee, oldPassword, newPassword);
        //}

        //public async Task<IdentityResult> CreateEmployee(Employee employee, string password)
        //{
        //    employee.CreatedDate = DateTime.Now.Date;
        //    employee.PhoneNumber = employee.CellPhoneNumber;
        //    employee.PhoneNumberConfirmed = true;
        //    return await _employeeManager.CreateAsync(employee, password);
        //}

        //public async Task<IdentityResult> DeleteEmployee(Employee employee)
        //{
        //    return await _employeeManager.DeleteAsync(employee);

        //}

        //public async Task<Employee> GetEmployeeAsync(ClaimsPrincipal employee)
        //{
        //    return await _employeeManager.GetUserAsync(employee);
        //}

        //public async Task<Employee> GetEmployeeByEmail(string email)
        //{
        //    return await _employeeManager.FindByEmailAsync(email);
        //}

        //public async Task<Employee> GetEmployeeById(Guid employeeId)
        //{
        //    return await _employeeManager.FindByIdAsync(employeeId.ToString());
        //}

        public async Task<bool> HasPasswordAsync(Employee employee)
        {
            return await _employeeManager.HasPasswordAsync(employee);
        }

        //public async Task<IdentityResult> ResetPasswordAsync(Employee employee, string token, string password)
        //{
        //    return await _employeeManager.ResetPasswordAsync(employee, token, password);
        //}

        //public async Task<IdentityResult> SetEmployeePassword(Employee employee, string password)
        //{
        //    return await _employeeManager.AddPasswordAsync(employee, password);
        //}

        //public async Task<IdentityResult> AddToRole(Employee employee,string role)
        //{
        //    return await _employeeManager.AddToRoleAsync(employee, role);
        //}



        public async Task<Employee> GetUserByIdAsync(string userId)
        {
            return await _employeeManager.FindByIdAsync(userId);
        }

        public async Task<Employee> GetUserByUserNameAsync(string userName)
        {
            return await _employeeManager.FindByNameAsync(userName);
        }

        public Employee GetEmployeeByEmail(string email)
        {
            return _unitOfWork.Employees.GetWhere(x => x.Email == email).FirstOrDefault();

        }

        public async Task<IList<string>> GetUserRolesAsync(Employee user)
        {
            return await _employeeManager.GetRolesAsync(user);
        }


        public Tuple<Employee, string[]> GetUserAndRolesAsync(string userId)
        {
            var user = _unitOfWork.Employees
                .GetAllIncluding(e => e.Id == userId, r => r.Roles)
                .FirstOrDefault();

            if (user == null)
                return null;

            var userRoleIds = user.Roles.Select(r => r.RoleId).ToList();

            var roles = _unitOfWork.Roles
                .GetWhere(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToArray();

            return Tuple.Create(user, roles);
        }


        public List<Tuple<Employee, string[]>> GetUsersAndRolesAsync(int page, int pageSize)
        {
            var employees = _unitOfWork.Employees
                .GetAllIncluding(page, pageSize, r => r.Roles)
                .OrderBy(c => c.UserName);

            var userRoleIds = employees.SelectMany(u => u.Roles.Select(r => r.RoleId)).ToList();

            var roles = _unitOfWork.Roles.GetWhere(r => userRoleIds.Contains(r.Id)).ToList();


            return employees.Select(u => Tuple.Create(u,
                roles.Where(r => u.Roles.Select(ur => ur.RoleId).Contains(r.Id)).Select(r => r.Name).ToArray()))
                .ToList();
        }


        public async Task<Tuple<bool, string[]>> CreateUserAsync(Employee user, IEnumerable<string> roles, string password)
        {
            var result = await _employeeManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            user = await _employeeManager.FindByNameAsync(user.UserName);

            try
            {
                result = await _employeeManager.AddToRoleAsync(user, roles.FirstOrDefault());
            }
            catch (Exception ex)
            {
                await DeleteUserAsync(user);
                throw;
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
            }

            return Tuple.Create(true, new string[] { });
        }


        public async Task<Tuple<bool, string[]>> UpdateUserAsync(Employee user)
        {
            return await UpdateUserAsync(user, null);
        }


        public async Task<Tuple<bool, string[]>> UpdateUserAsync(Employee user, IEnumerable<string> roles)
        {
            var result = await _employeeManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            if (roles != null)
            {
                var userRoles = await _employeeManager.GetRolesAsync(user);

                var rolesToRemove = userRoles.Except(roles).ToArray();
                var rolesToAdd = roles.Except(userRoles).Distinct().ToArray();

                if (rolesToRemove.Any())
                {
                    result = await _employeeManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!result.Succeeded)
                        return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                }

                if (rolesToAdd.Any())
                {
                    result = await _employeeManager.AddToRolesAsync(user, rolesToAdd);
                    if (!result.Succeeded)
                        return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return Tuple.Create(true, new string[] { });
        }


        public async Task<Tuple<bool, string[]>> ResetPasswordAsync(Employee user, string newPassword)
        {
            string resetToken = await _employeeManager.GeneratePasswordResetTokenAsync(user);

            var result = await _employeeManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> UpdatePasswordAsync(Employee user, string currentPassword, string newPassword)
        {
            var result = await _employeeManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());

            return Tuple.Create(true, new string[] { });
        }

        public async Task<bool> CheckPasswordAsync(Employee user, string password)
        {
            if (!await _employeeManager.CheckPasswordAsync(user, password))
            {
                if (!_employeeManager.SupportsUserLockout)
                    await _employeeManager.AccessFailedAsync(user);

                return false;
            }

            return true;
        }


        public async Task<Tuple<bool, string[]>> DeleteUserAsync(string userId)
        {
            var user = await _employeeManager.FindByIdAsync(userId);

            if (user != null)
                return await DeleteUserAsync(user);

            return Tuple.Create(true, new string[] { });
        }


        public async Task<Tuple<bool, string[]>> DeleteUserAsync(Employee user)
        {
            var result = await _employeeManager.DeleteAsync(user);
            return Tuple.Create(result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }






        public async Task<Role> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }


        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }


        public Role GetRoleLoadRelatedAsync(string roleName)
        {
            var role = _unitOfWork.Roles.GetAllIncluding(q => q.Name == roleName, x => x.Claims, c => c.Users).FirstOrDefault();
            return role;
        }


        public List<Role> GetRolesLoadRelatedAsync(int page, int pageSize)
        {
            var roles = _unitOfWork.Roles.GetAllIncluding(page, pageSize, x => x.Claims, c => c.Users)
                  .OrderBy(o => o.Name);

            return roles.ToList();
        }


        public async Task<Tuple<bool, string[]>> CreateRoleAsync(Role role, IEnumerable<string> claims)
        {
            if (claims == null)
                claims = new string[] { };

            string[] invalidClaims = claims.Where(c => ApplicationPermissions.GetPermissionByValue(c) == null).ToArray();
            if (invalidClaims.Any())
                return Tuple.Create(false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });


            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            role = await _roleManager.FindByNameAsync(role.Name);

            foreach (string claim in claims.Distinct())
            {
                result = await this._roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, ApplicationPermissions.GetPermissionByValue(claim)));

                if (!result.Succeeded)
                {
                    await DeleteRoleAsync(role);
                    return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> UpdateRoleAsync(Role role, IEnumerable<string> claims)
        {
            if (claims != null)
            {
                string[] invalidClaims = claims.Where(c => ApplicationPermissions.GetPermissionByValue(c) == null).ToArray();
                if (invalidClaims.Any())
                    return Tuple.Create(false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });
            }


            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            if (claims != null)
            {
                var roleClaims = (await _roleManager.GetClaimsAsync(role)).Where(c => c.Type == CustomClaimTypes.Permission);
                var roleClaimValues = roleClaims.Select(c => c.Value).ToArray();

                var claimsToRemove = roleClaimValues.Except(claims).ToArray();
                var claimsToAdd = claims.Except(roleClaimValues).Distinct().ToArray();

                if (claimsToRemove.Any())
                {
                    foreach (string claim in claimsToRemove)
                    {
                        result = await _roleManager.RemoveClaimAsync(role, roleClaims.Where(c => c.Value == claim).FirstOrDefault());
                        if (!result.Succeeded)
                            return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }

                if (claimsToAdd.Any())
                {
                    foreach (string claim in claimsToAdd)
                    {
                        result = await _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, ApplicationPermissions.GetPermissionByValue(claim)));
                        if (!result.Succeeded)
                            return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }
            }

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role != null)
                return await DeleteRoleAsync(role);

            return Tuple.Create(true, new string[] { });
        }


        public async Task<Tuple<bool, string[]>> DeleteRoleAsync(Role role)
        {
            var result = await _roleManager.DeleteAsync(role);
            return Tuple.Create(result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }



    }
}
