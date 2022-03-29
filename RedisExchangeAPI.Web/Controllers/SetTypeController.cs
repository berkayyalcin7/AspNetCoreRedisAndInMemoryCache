using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedisExchangeAPI.Web.Controllers
{
    public class SetTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private string listKey = "hashName";

        public SetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(1);
        }

        public IActionResult Index()
        {
            HashSet<string> namesList = new();
            // içindeki değerler Unique ve sırasız şekilde tutar.
            if (_db.KeyExists(listKey))
            {
                _db.SetMembers(listKey).ToList().ForEach(x =>
                {
                    namesList.Add(x.ToString());
                });
            }
            return View(namesList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            // Anahtar yok ise Expire 1 dakika ekle
            if (!_db.KeyExists(listKey))
            {
               
            }

            _db.KeyExpire(listKey, TimeSpan.FromMinutes(1));
            _db.SetAdd(listKey, name);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string name)
        {
            _db.SetRemoveAsync(listKey, name).Wait();

            return RedirectToAction("Index");
        }
    }
}
