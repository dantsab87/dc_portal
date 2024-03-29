﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using dc_portal.Extensions;
using dc_portal.Models;
using Microsoft.AspNet.Identity;

namespace dc_portal.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            var myTransactions = db.Transactions.Where(mt => mt.OwnerId == user.Id);
            //var transactions = db.Transactions.Include(t => t.BankAccount).Include(t => t.BudgetItem).Include(t => t.Owner);
            return View(myTransactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            var houseId = db.Users.Find(User.Identity.GetUserId()).HouseholdId ?? 0;
            var myhouse = db.Households.Find(houseId);

            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(ba => ba.HouseholdId == houseId), "Id", "Name");
            ViewBag.BudgetId = new SelectList(db.Budgets.Where(b => b.HouseholdId == houseId), "Id", "Name");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems.Where(bi=>bi.Budget.HouseholdId == houseId), "Id", "Name");
            //ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerId");
            //ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "Name");
            //ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BankAccountId,BudgetItemId,TransactionType,Amount,Memo")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.OwnerId = User.Identity.GetUserId();
                transaction.Created = DateTime.Now;
                db.Transactions.Add(transaction);
                db.SaveChanges();

                //transaction extensions to keep associated bank and budget balances up to date
                transaction.UpdateBalances();
                transaction.ManageNotifications();

                return RedirectToAction("Dashboard", "Home");
                //db.Transactions.Add(transaction);
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "Name", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", transaction.OwnerId);
            return View(transaction);
            //ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.BankAccountId);
            //ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "Name", transaction.BudgetItemId);
            //ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", transaction.OwnerId);
            //return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.BankAccountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "Name", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", transaction.OwnerId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankAccountId,BudgetItemId,OwnerId,TransactionType,Created,Amount,Memo")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.BankAccountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "Name", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", transaction.OwnerId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
