using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        public IUnitOfWork _unitOfWork { get; set; }

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
            return View(objProductList);
        }

		public IActionResult Create()
		{
			IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString(),
			});
			//here we can use both of these methods
			ViewBag.CategoryList = CategoryList;
			//ViewData["CategoryList"] = CategoryList;
			return View();
		}
        [HttpPost]
		public IActionResult Create(Product obj)
		{
            _unitOfWork.Product.Add(obj);
            _unitOfWork.Save();
			TempData["success"] = "Product created successfully";
			return RedirectToAction("Index");
		}
		public IActionResult Delete(int id)
		{
			Product obj = _unitOfWork.Product.Get(u=>u.Id == id);
			return View(obj);
		}
		[HttpPost]
		public IActionResult Delete(Product obj)
		{
			_unitOfWork.Product.Remove(obj);
			_unitOfWork.Save();
			TempData["success"] = "Product deleted successfully";
			return RedirectToAction("Index");
		}
		public IActionResult Edit(int id)
		{
			Product obj = _unitOfWork.Product.Get(u => u.Id == id);
			return View(obj);
		}
		[HttpPost]
		public IActionResult Edit(Product obj)
		{
			_unitOfWork.Product.Update(obj);
			_unitOfWork.Save();
			TempData["success"] = "Product updated successfully";
			return RedirectToAction("Index");
		}
	}
}
