using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppInvoiceSystem.Models;

namespace WebAppInvoiceSystem.Controllers
{
    public class CashiersController : Controller
    {
        private ArmyDb db = new ArmyDb();

        // GET: Cashiers
        public ActionResult Index()
        {
            var cashiers = db.Cashiers.Include(c => c.Branch);
            return View(cashiers.ToList());
        }

        // GET: Cashiers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cashier cashier = db.Cashiers.Find(id);
            if (cashier == null)
            {
                return HttpNotFound();
            }
            return View(cashier);
        }

        // GET: Cashiers/Create
        public ActionResult Create()
        {
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "BranchName");
            return View();
        }

        // POST: Cashiers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CashierName,BranchID")] Cashier cashier)
        {
            if (ModelState.IsValid)
            {
                db.Cashiers.Add(cashier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchID = new SelectList(db.Branches, "ID", "BranchName", cashier.BranchID);
            return View(cashier);
        }

        // GET: Cashiers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cashier cashier = db.Cashiers.Find(id);
            if (cashier == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "BranchName", cashier.BranchID);
            return View(cashier);
        }

        // POST: Cashiers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CashierName,BranchID")] Cashier cashier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "BranchName", cashier.BranchID);
            return View(cashier);
        }

        // GET: Cashiers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cashier cashier = db.Cashiers.Find(id);
            if (cashier == null)
            {
                return HttpNotFound();
            }
            return View(cashier);
        }

        // POST: Cashiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cashier cashier = db.Cashiers.Find(id);
            db.Cashiers.Remove(cashier);
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
