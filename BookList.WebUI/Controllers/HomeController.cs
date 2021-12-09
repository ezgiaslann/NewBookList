using BookList.Business.Abstract;
using BookList.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookList.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private IProductService _productService;
        //private ICategoryService _categoryService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
            //_categoryService = categoryService; 
        }

        public IActionResult Index()
        {
            return View(new ProductListModel()
            {
                Products = _productService.GetAll(),
                //Categories = _categoryService.GetAll()
            });
        }
    }
}
