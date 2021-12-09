using BookList.Business.Abstract;
using BookList.Entities;
using BookList.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookList.WebUI.Controllers
{
    public class ShopController : Controller
    {
        private IProductService _productService;
        //private ICategoryService _categoryService;

        public ShopController(IProductService productService)
        {
            _productService = productService;
            //_categoryService = categoryService; 
        }

        public IActionResult Details(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetails((int)id);
            if(product == null)
            {
                return NotFound();
            }
            return View(new ProductDetailsModel()
            {
                Product = product,
                Categories = product.ProductCategories.Select(i => i.Category).ToList()
            });
        }

        public IActionResult List(string category, int page=1)
        {
            const int pageSize = 3;
            return View(new ProductListModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory = category
                },
                Products = _productService.GetProductsByCategory(category, page, pageSize)
                //Categories = _categoryService.GetAll()
            });
        }
    }
}
