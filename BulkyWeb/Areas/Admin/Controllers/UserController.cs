using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class UserController : Controller
	{
		private readonly ApplicationDBContext _db;
		private readonly UserManager<IdentityUser> _userManager;
		public UserController(ApplicationDBContext db, UserManager<IdentityUser> userManager)
		{
			_db = db;
			_userManager = userManager;
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult RoleManagment(string? userId)
		{
			RoleManagmentVM roleManagmentVM = new RoleManagmentVM()
			{
				ApplicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(i => i.Id == userId),
				Companys = _db.Companys.Select(x => x.Name).Select(i => new SelectListItem
				{
					Text = i,
					Value = i
				}),
				Roles = _db.Roles.Select(x => x.Name).Select(i => new SelectListItem
				{
					Text = i,
					Value = i
				})
			};
			var userRoles = _db.UserRoles.ToList();
			var roles = _db.Roles.ToList();
			var roleId = userRoles.FirstOrDefault(u => u.UserId == roleManagmentVM.ApplicationUser.Id).RoleId;
			roleManagmentVM.ApplicationUser.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;


			return View(roleManagmentVM);
		}

		[HttpPost]
		public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
		{
			var user = _userManager.FindByIdAsync(roleManagmentVM.ApplicationUser.Id).Result;
			if (user != null)
			{
				var currentRoles = _userManager.GetRolesAsync(user).Result;
				foreach (var role in currentRoles)
				{
					_userManager.RemoveFromRoleAsync(user, role).Wait();
				}
				_userManager.AddToRoleAsync(user, roleManagmentVM.ApplicationUser.Role).Wait();
			}
			var roleid = _db.Companys.FirstOrDefault(u => u.Name == roleManagmentVM.ApplicationUser.Company.Name);
			var applicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(i => i.Id == roleManagmentVM.ApplicationUser.Id);

			if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
			{
				applicationUser.CompanyId = roleid.Id;
			}
			else
			{
				applicationUser.CompanyId = null;
			}

			_db.ApplicationUsers.Update(applicationUser);
			_db.SaveChanges();

			TempData["Success"] = "User Updated Successfully!";

			return RedirectToAction("Index");
		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();

			var userRoles = _db.UserRoles.ToList();
			var roles = _db.Roles.ToList();

			foreach (var user in objUserList)
			{

				var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
				user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

				if (user.Company == null)
				{
					user.Company = new Company() { Name = "" };
				}
			}

			return Json(new { data = objUserList });

		}

		[HttpPost]
		public IActionResult LockUnlock([FromBody] string id)
		{
			var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
			if (objFromDb == null)
			{
				return Json(new { success = false, message = "Error while Locking/Unlocking" });
			}

			if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
			{
				objFromDb.LockoutEnd = DateTime.Now;
			}
			else
			{
				objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
			}
			_db.SaveChanges();

			return Json(new { success = true, message = "Operation Successfull!" });

		}
		#endregion
	}
}
