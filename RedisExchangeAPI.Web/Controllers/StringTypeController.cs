using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class StringTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        public StringTypeController(RedisService redisService, IDatabase db)
        {
            _redisService = redisService;
            _db = db;
        }

        public IActionResult Index()
        {
            var db = _redisService.GetDb(0);

            db.StringSet("Name", "Berkay Yalçın");
            db.StringSet("Counter", 1000);


            return View();
        }

        public IActionResult Show()
        {
            // Result ile değerini alabiliriz .. Await async kullanmadan
            var value = _db.StringGetRangeAsync("Name", 0, 3).Result;
            // Her çalıştığında 100 azalt Result ile farklı olarak sadece metodu çalıştırır . Sonuç Dönmez.
            _db.StringDecrementAsync("Counter",100).Wait();

            return View();
        }

    }
}
