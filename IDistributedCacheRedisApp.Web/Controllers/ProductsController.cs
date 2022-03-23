using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {

        private readonly IDistributedCache _distributedCache;
        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;

        }

        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new();

            // Cache Exp 1 minutes
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(30);

            _distributedCache.SetString("Name", "Apple Iphone 13", cacheEntryOptions);      

            // Using Async
            await _distributedCache.SetStringAsync("Price", "15000 TL");


            Product product = new Product { Id=1,Name="Samsung Galaxy S22",Price=18000};

            string jsonProduct = JsonConvert.SerializeObject(product);

            Byte[] byteArray = Encoding.UTF8.GetBytes(jsonProduct);

            // Complex Type Cache
            await _distributedCache.SetStringAsync("product:1", jsonProduct, cacheEntryOptions);

            // Byte cache - Not Prefer 
            _distributedCache.Set("product:3", byteArray);

            return View();
        }

        public async Task<IActionResult> ImageCache()
        {
            // Take Path 
            string path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img/haosama.png");

            byte[] imageByte = System.IO.File.ReadAllBytes(path);

            await _distributedCache.SetAsync("MyImage",imageByte);


            return View();
        }

        public IActionResult ImageUrl()
        {
            byte[] image = _distributedCache.Get("MyImage");

            return File(image,"image/png");
        }

        public IActionResult Show()
        {
            string name = _distributedCache.GetString("Name");

            string jsonProduct = _distributedCache.GetString("product:1");

            Product product = JsonConvert.DeserializeObject<Product>(jsonProduct);

            ViewBag.Name = name;
            ViewBag.Product = product;
            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("Name");
            return View();
        }





    }
}
