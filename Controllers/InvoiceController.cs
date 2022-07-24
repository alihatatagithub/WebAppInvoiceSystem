using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppInvoiceSystem.Models;
using WebAppInvoiceSystem.Repository;

namespace WebAppInvoiceSystem.Controllers
{
    public class InvoiceController : Controller
    {
        private ArmyDb db = new ArmyDb();

        // GET: Invoice
        public ActionResult Index()
        {
            var invoiceHeaders = db.InvoiceHeaders.Include(i => i.Branch).Include(i => i.Cashier)
                .Include(a => a.InvoiceDetails);
            return View(invoiceHeaders.ToList());
        }

        // GET: Invoice/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceHeader invoiceHeader = db.InvoiceHeaders.Find(id);
            if (invoiceHeader == null)
            {
                return HttpNotFound();
            }
            return View(invoiceHeader);
        }

        // GET: Invoice/Create
        public ActionResult Create()
        {
            ViewBag.ErrorResult = null;
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "BranchName");
            ViewBag.CashierID = new SelectList(db.Cashiers, "ID", "CashierName");
            
            return View();
        }

        public ActionResult CreateInvoiceDetails()
        {
            
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInvoiceDetails(InvoiceDetail invoiceDetail)
        {

            InvoiceRepo.InvoiceDetailsRepo.Add(invoiceDetail);


            return RedirectToAction("Create");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CustomerName,Invoicedate,CashierID,BranchID")] InvoiceHeader invoiceHeader)
        {
            if (ModelState.IsValid && InvoiceRepo.InvoiceDetailsRepo.Any())
            {
                db.InvoiceHeaders.Add(invoiceHeader);
                
                foreach (var item in InvoiceRepo.InvoiceDetailsRepo)
                {
                    item.InvoiceHeaderID = invoiceHeader.ID;
                    db.InvoiceDetails.Add(item);
                }
                InvoiceRepo.InvoiceDetailsRepo.Clear();
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ErrorResult = "Please Add At Least One Invoice Details";
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "BranchName", invoiceHeader.BranchID);
            ViewBag.CashierID = new SelectList(db.Cashiers, "ID", "CashierName", invoiceHeader.CashierID);
            return View(invoiceHeader);
        }

        // GET: Invoice/Edit/5
        public ActionResult Edit(long? id,bool HasData = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceHeader invoiceHeader = db.InvoiceHeaders.Include(a => a.InvoiceDetails).FirstOrDefault(a => a.ID == id);
            if (invoiceHeader == null)
            {
                return HttpNotFound();
            }
            if (HasData)
            {
                InvoiceRepo.InvoiceDetailsRepo.Clear();
                foreach (var item in invoiceHeader.InvoiceDetails)
                {
                    InvoiceRepo.InvoiceDetailsRepo.Add(item);
                }

            }
            else if(!InvoiceRepo.InvoiceDetailsRepo.Any())
            {
                foreach (var item in invoiceHeader.InvoiceDetails)
                {
                    InvoiceRepo.InvoiceDetailsRepo.Add(item);
                }
            }
           
            ViewBag.InvoiceRepo = InvoiceRepo.InvoiceDetailsRepo;
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "BranchName", invoiceHeader.BranchID);
            ViewBag.CashierID = new SelectList(db.Cashiers, "ID", "CashierName", invoiceHeader.CashierID);
            return View(invoiceHeader);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InvoiceHeader invoiceHeader)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoiceHeader).State = EntityState.Modified;
                if (InvoiceRepo.InvoiceDetailsRepo.Any())
                {
                    foreach (var item in InvoiceRepo.InvoiceDetailsRepo)
                    {
                        InvoiceDetail oldInvoiceDetail = db.InvoiceDetails.FirstOrDefault(a => a.ID == item.ID);
                        oldInvoiceDetail.ItemName = item.ItemName;
                        oldInvoiceDetail.ItemPrice = item.ItemPrice;
                        oldInvoiceDetail.ItemCount = item.ItemCount;
                        
                    }

                }
                db.SaveChanges();
                InvoiceRepo.InvoiceDetailsRepo.Clear();
                return RedirectToAction("Index");
            }
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "BranchName", invoiceHeader.BranchID);
            ViewBag.CashierID = new SelectList(db.Cashiers, "ID", "CashierName", invoiceHeader.CashierID);
            return View(invoiceHeader);
        }

        public ActionResult EditInvoiceDetails(long? id,long? InvoiceHeaderId)
        {
            if (id == null || InvoiceHeaderId == null)
            {
                return RedirectToAction("Edit", new { id = InvoiceHeaderId });
            }
            InvoiceDetail invoiceDetail = db.InvoiceDetails.FirstOrDefault(a => a.ID == id);
            return PartialView(invoiceDetail);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInvoiceDetails(InvoiceDetail invoiceDetail)
        {

            InvoiceDetail oldInvoiceDetail = InvoiceRepo.InvoiceDetailsRepo
                .FirstOrDefault(a => a.ID == invoiceDetail.ID);

            oldInvoiceDetail.ItemName = invoiceDetail.ItemName;
            oldInvoiceDetail.ItemPrice = invoiceDetail.ItemPrice;
            oldInvoiceDetail.ItemCount = invoiceDetail.ItemCount;

            return RedirectToAction("Edit", new { id = invoiceDetail.InvoiceHeaderID });

        }
        // GET: Invoice/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceHeader invoiceHeader = db.InvoiceHeaders.Find(id);
            if (invoiceHeader == null)
            {
                return HttpNotFound();
            }
            return View(invoiceHeader);
        }

        // POST: Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            InvoiceHeader invoiceHeader = db.InvoiceHeaders.Find(id);
            var invoicedetails = db.InvoiceDetails.Where(a => a.InvoiceHeaderID == id);
            if (InvoiceRepo.InvoiceDetailsRepo.Any())
            {
                InvoiceRepo.InvoiceDetailsRepo.Clear();
            }
            db.InvoiceDetails.RemoveRange(invoicedetails);
            db.InvoiceHeaders.Remove(invoiceHeader);
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
