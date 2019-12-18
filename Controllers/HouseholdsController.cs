using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using dc_portal.Extensions;
using dc_portal.Helpers;
using dc_portal.Models;
using Microsoft.AspNet.Identity;

namespace dc_portal.Controllers
{
    [Authorize]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper roleHelper = new RoleHelper();

        // GET: Households
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var userHousehold = db.Households.Where(h=>h.Id == user.HouseholdId).ToList();

            return View(userHousehold);
            //return View(db.Households.ToList());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }


        //get Households/config/5

        public ActionResult Config(int? Id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (Id != null && Id != 0 && user.Household.IsConfigured == false) 
            {
                var configVm = new ConfigVM();

                configVm.HouseHoldId = (int)Id;
                return View(configVm);
            }
            return RedirectToAction("Dashboard", "Home");
        }

        //post Households/config/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Config(ConfigVM configVM)
        {       
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            //if (ModelState.IsValid)
            //{
                var bank = new BankAccount
                {
                    HouseholdId = configVM.HouseHoldId,
                    Name = configVM.Name,
                    StartingBalance = configVM.StartingBalance,
                    LowBalanceLevel = configVM.LowBalance,
                    AccountType = configVM.AccountType,

                    Created = DateTime.Now,
                    CurrentBalance = configVM.StartingBalance,
                    OwnerId = userId
                };

                db.BankAccounts.Add(bank);
                db.SaveChanges();

                var budget = new Budget
                {
                    Name = configVM.BudgetName,
                    Created = DateTime.Now,
                    OwnerId = userId,
                    HouseholdId = configVM.HouseHoldId,
                    TargetAmount = 0
                };

                db.Budgets.Add(budget);
                db.SaveChanges();

                var bItems = new BudgetItem
                {
                    Name = configVM.BIName,
                    TargetAmount = configVM.TargetAmount,
                    BudgetId = budget.Id,
                    Created = DateTime.Now,
                    CurrentAmount = 0
                };


                db.BudgetItems.Add(bItems);
                user.Household.IsConfigured = true;
                TempData["Complete_HH_config"] = "You have just configured your Household!";
                db.SaveChanges();

                return RedirectToAction("Dashboard", "Home");
            //}

            //return View(configVM);
        }




        // GET: Households/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Guest"))
            {
                return View();
            }
            if (User.IsInRole("Head of Household"))
            {
                TempData["MustLeaveHH"] = "If you want to create a new HouseHold or join a new one, please leave your current Household.";
                return RedirectToAction("Dashboard", "Home");
            }

            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Guest")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Greeting")] Household household)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Guest"))
                {
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Find(userId);

                    roleHelper.RemoveUserFromRole(user.Id, "Guest");


                    household.Created = DateTime.Now;
                    db.Households.Add(household);
                    user.HouseholdId = household.Id;

                    roleHelper.AddUserToRole(user.Id, "Head of Household");

                    db.SaveChanges();

                    await ControllerContext.HttpContext.RefreshAuthentication(user);

                    return RedirectToAction("Details", "Households", new { id = household.Id });
                }
            }

            return View(household);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Greeting,Created")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }



        // GET: Households/Delete/5
        [Authorize(Roles = "Head of Household")]
        public ActionResult Delete(int? id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var totalusers = db.Users.Where(t => t.HouseholdId == user.HouseholdId).Count();

            if (totalusers > 1)
            {
                TempData["CantDelete"] = "Unable to delete Household because there are others still occupying the Household!";
                return RedirectToAction("Details", "Households", new { id = user.HouseholdId });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);

        }

        // POST: Households/Delete/5
        [Authorize(Roles = "Head of Household")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (User.IsInRole("Head of Household"))
            {
                Household household = db.Households.Find(id);
                db.Households.Remove(household);
                user.HouseholdId = null;
                roleHelper.RemoveUserFromRole(user.Id, "Head of Household");
                roleHelper.AddUserToRole(user.Id, "Guest");
                TempData["Deleted-HH"] = "You have successfully deleted your Household!";
                db.SaveChanges();
                await ControllerContext.HttpContext.RefreshAuthentication(user);
            }
            return RedirectToAction("Dashboard", "Home");
        }



        // GET: Households/Leave/5
        public async Task<ActionResult> Leave(int? id)
        {
            Household household = db.Households.Find(id);
            if (User.IsInRole("Head of Household"))
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var totalusers = db.Users.Where(t => t.HouseholdId == user.HouseholdId).Count();
                if (totalusers > 1)
                {
                    var myhousehold = user.HouseholdId;
                    //var members = db.Users.Where(u => u.HouseholdId == myhousehold && u.Id != userId);
                    var members = roleHelper.UsersInRole("Member").Where(m=>m.HouseholdId == myhousehold).ToList();

                    ViewBag.HouseMembers = new SelectList(members, "Id", "FullName");
                    return View();
                    //TempData["CantDelete"] = "Unable to leave Household because there are others still occupying the Household!";
                    //return RedirectToAction("Details", "Households", new { id = user.HouseholdId });

                    //add successor option here//

                }
                //var myhousehold = user.HouseholdId;
                //RoleHelper roleHelper = new RoleHelper();
                //var members = roleHelper.UsersInRole("Member").ToList();
                //var members = db.Users.Where(u => u.HouseholdId == myhousehold && u.Id != userId);


                //ViewBag.HouseMembers = new SelectList(members, "Id", "FirstName");

                //TempData["Congrats-Leave"] = $"You have successfully left the Household, {myhousehold.Name} ! ";
                //db.Households.Remove(myhousehold);
                //user.HouseholdId = null;
                //roleHelper.RemoveUserFromRole(user.Id, "Head of Household");
                //roleHelper.AddUserToRole(user.Id, "Guest");
                //db.SaveChanges();
                //await ControllerContext.HttpContext.RefreshAuthentication(user);
                //return RedirectToAction("Dashboard", "Home");
            }

            if (User.IsInRole("Member"))
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);

                var myhousehold = db.Households.Find(id);
                TempData["Congrats-Leave"] = $"You have successfully left the Household, {myhousehold.Name} ! ";
                db.Households.Remove(myhousehold);
                user.HouseholdId = null;
                roleHelper.RemoveUserFromRole(user.Id, "Member");
                roleHelper.AddUserToRole(user.Id, "Guest");
                db.SaveChanges();
                await ControllerContext.HttpContext.RefreshAuthentication(user);
                return RedirectToAction("Dashboard", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Leave/5
        [HttpPost, ActionName("Leave")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Leave(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            RoleHelper roleHelper = new RoleHelper();


            if (User.IsInRole("Head of Household")) 
            {
                Household household = db.Households.Find(id);
                db.Households.Remove(household);
                roleHelper.RemoveUserFromRole(user.Id, "Head of Household");
                roleHelper.AddUserToRole(user.Id, "Guest");
                db.SaveChanges();
                await ControllerContext.HttpContext.RefreshAuthentication(user);
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Dashboard", "Home");
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
