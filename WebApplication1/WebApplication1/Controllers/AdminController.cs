using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApp.Dtos;
using UserApp.Models;

namespace UserApp.Controllers
{
    [ApiController]
    [Route("/api/[Controller]")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpGet]
        public async Task<IActionResult> GetUserWithRole()
        {
            var userList = await _userManager.Users
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    Id = x.Id,
                    userName = x.UserName,
                    roles = (from userRole in x.UserRoles
                        join role in _roleManager.Roles on userRole.RoleId equals role.Id
                        select role.Name).ToList()
                }).ToListAsync();

            return Ok(userList);
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost("editroles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, EditForRolesDto editForRolesDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = editForRolesDto.RoleNames;
            selectedRoles = selectedRoles ?? new string[] { };

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
                return BadRequest(("Failed to add roles"));

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to remove roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var isDelete = _userManager.DeleteAsync(user).Result;

            if (!isDelete.Succeeded)
            {
                return BadRequest("User can not be deleted.");
            }

            return Ok();
        }
    }
}