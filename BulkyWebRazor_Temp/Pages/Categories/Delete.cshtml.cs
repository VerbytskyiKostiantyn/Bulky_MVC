using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
    {
		private readonly ApplicationDBContext _db;
		[BindProperty]
		public Category Category { get; set; }
		public DeleteModel(ApplicationDBContext db)
		{
			_db = db;
		}
		public void OnGet(int id)
		{
			Category = _db.Categories.Find(id);
		}

		public IActionResult OnPost()
		{
			_db.Categories.Remove(Category);
			_db.SaveChanges();
			TempData["success"] = "Category deleted successfully";
			return RedirectToPage("index");
		}
	}
}
