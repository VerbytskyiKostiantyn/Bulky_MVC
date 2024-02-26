using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
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
		public UserController(ApplicationDBContext db)
		{
			_db = db;
		}
		public IActionResult Index()
		{
			return View();
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

				var roleId = userRoles.FirstOrDefault(u=>u.UserId == user.Id).RoleId;
				user.Role = roles.FirstOrDefault(u=>u.Id == roleId).Name;

				if (user.Company == null)
				{
					user.Company = new Company() { Name = "" };
				}
			}

			return Json(new { data = objUserList });

		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{


			return Json(new { success = true, message = "Delete Successfull!" });

		}
		#endregion
	}
}
