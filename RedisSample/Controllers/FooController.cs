﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RedisSample;
using RedisSample.Models;
using RedisSample.Redis;
using StackExchange.Redis;

namespace RedisSample.Controllers
{
    public class FooController : Controller
    {
        private MyDbContext db = new MyDbContext();

        public IEnumerable<RedisValue> ReadData()
        {
            var result = new List<RedisValue>();
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            var devicesCount = 10000;
            for (int i = 0; i < devicesCount; i++)
            {
                var value = cache.StringGet($"Device_Status:{i}");
                result.Add(value);
            }

            return result;
        }

        public void SaveBigData()
        {
            var devicesCount = 10000;
            var rnd = new Random();
            var cache = RedisConnectorHelper.Connection.GetDatabase();

            for (int i = 0; i < devicesCount; i++)
            {
                var value = rnd.Next(0, 10000);
                cache.StringSet($"Device_Status:{i}", value);
            }
        }

        // GET: Foo
        public ActionResult Index()
        {
            SaveBigData();
            ReadData();
            return View(db.Foos.ToList());
        }

        // GET: Foo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Foo foo = db.Foos.Find(id);
            if (foo == null)
            {
                return HttpNotFound();
            }
            return View(foo);
        }

        // GET: Foo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Foo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description")] Foo foo)
        {
            if (ModelState.IsValid)
            {
                db.Foos.Add(foo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(foo);
        }

        // GET: Foo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Foo foo = db.Foos.Find(id);
            if (foo == null)
            {
                return HttpNotFound();
            }
            return View(foo);
        }

        // POST: Foo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description")] Foo foo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(foo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(foo);
        }

        // GET: Foo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Foo foo = db.Foos.Find(id);
            if (foo == null)
            {
                return HttpNotFound();
            }
            return View(foo);
        }

        // POST: Foo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Foo foo = db.Foos.Find(id);
            db.Foos.Remove(foo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
