using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Microsoft.AspNet.Identity;
using WebCA_LeahuLorena.Models;

namespace WebCA_LeahuLorena.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Models.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Model model = db.Models.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Users/Create

        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var userID = User.Identity.GetUserId();
            var existingModel = db.Models.FirstOrDefault(s => s.userID == userID);

            if (existingModel == null)
            {
                existingModel = new Model();
            }
            var user = db.Users.FirstOrDefault(s => s.Id == userID);
            if (user != null)
                existingModel.emailAddress = user.Email;
            return View(existingModel);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,userID,countryShort,country,locality,organization,departament,domain,privatePassword")] Model model)
        {
            if (ModelState.IsValid)
            {
                var userID = User.Identity.GetUserId();

                var existingModel = db.Models.FirstOrDefault(s => s.userID == userID);

                if (existingModel == null)
                {
                    model.userID = userID;
                    db.Models.Add(model);
                }
                else
                {
                    if (existingModel != null)
                    {
                        //update
                        existingModel.locality = model.locality;
                        existingModel.country = model.country;
                        existingModel.countryShort = model.countryShort;
                        existingModel.departament = model.departament;
                        existingModel.domain = model.domain;
                        existingModel.organization = model.organization;
                        existingModel.privatePassword = model.privatePassword;
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Create");
                
            }

            return View(model);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Model model = db.Models.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,userID,countryShort,country,locality,organization,departament,domain,privatePassword")] Model model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Model model = db.Models.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Model model = db.Models.Find(id);
            db.Models.Remove(model);
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
