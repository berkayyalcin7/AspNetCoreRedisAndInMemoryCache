using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InMemoryCache.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCache.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;

        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            //TryGetValue O cache'in Var olup olmadığını sorguluyoruz var ise Out keywordü ile zamancache içerisine atıyoruz.
            if (!_memoryCache.TryGetValue("Zaman", out string zamancache))
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();

                //Toplam ömrü 60 saniye
                options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);

                //3 dakika içerisinde dataya erişilmez ise Memory'den silinecek
                //options.SlidingExpiration = TimeSpan.FromSeconds(10);

                //Memory ' dolarsa ilk bu Key'i Cache'i sil diyoruz. Low - Normal - High - NeverRemove
                //NeverRemove Sadece çok önemli datalar için kullanılmalı 
                options.Priority = CacheItemPriority.Low;

                //Bu Delege 4 Tane Parametre alıyor 
                //Görevi bir data silindiği zaman bu hangi key value çiftinden olduğu ve Reason Sebebini belirtebiliyoruz.
                options.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _memoryCache.Set("callback", $"{key} -> {value} => Sebep : {reason}");

                });


                //Cache'de değerler Key Value şeklinde tutulur
                //options İle Ömrünü 3. parametreye veriyoruz.
                _memoryCache.Set<string>("Zaman", DateTime.Now.ToString(), options);

                ViewBag.Zaman = _memoryCache.Get<string>("Zaman");
            }
            return View();
        }

        public IActionResult Show()
        {
            //Cache'den siler
            //_memoryCache.Remove("Zaman");

            //Böyle Bir Cache' var ise almaya çalışır yok ise oluşturur.
            //_memoryCache.GetOrCreate<string>("Zaman", entry =>
            //{
            //    return DateTime.Now.ToString();
            //});
            _memoryCache.TryGetValue("Zaman", out string zaman);
            _memoryCache.TryGetValue("callback", out string callback);

            //Key Değeri ile Memory'deki Value'ye ulaşabiliriz
            //ViewBag.Zaman = _memoryCache.Get<string>("Zaman");


            ViewBag.Zaman = zaman;

            ViewBag.Callback = callback;

            return View();
        }

        //Complex  Type ' bir nesneyi memory'de tutma
        public IActionResult ProductInfo(Product product)
        {
            Product p = new Product { Id = 1, Name = "VS Licence", Price = 300 };

            //Burada Otomatik olarak Complex'typeları serialize işlemi yapıyor
            _memoryCache.Set<Product>("Product:1", p);

            ViewBag.Product = _memoryCache.Get<Product>("Product:1");

            return View();
        }
    }
}