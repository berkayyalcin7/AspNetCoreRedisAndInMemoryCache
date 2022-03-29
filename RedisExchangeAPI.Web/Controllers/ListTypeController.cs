using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace RedisExchangeAPI.Web.Controllers
{
    public class ListTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;

        private string listKey = "names";

        public ListTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(0);
        }

        public IActionResult Index()
        {
            List<string> namesList = new();

            if (_db.KeyExists(listKey))
            {
                _db.ListRange(listKey).ToList().ForEach(x =>
                {
                    namesList.Add(x.ToString());
                });
            }

            return View(namesList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            _db.ListRightPush(listKey, name);
            
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string name)
        {
            _db.ListRemoveAsync(listKey, name).Wait();

            return RedirectToAction("Index");
        }
    }
}
