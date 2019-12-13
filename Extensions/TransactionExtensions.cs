using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dc_portal.Enums;
using dc_portal.Models;

namespace dc_portal.Extensions
{
    public static class TransactionExtensions
    {
        public static ApplicationDbContext db = new ApplicationDbContext();

        public static void UpdateBalances(this Transaction transaction)
        {
            UpdateBalances(transaction);
            UpdateBudgetBalances(transaction);
            UpdateBudgetItemBalance(transaction);
        }

        public static void ManageNotifications(this Transaction transaction)
        {
            var bankAccount = db.BankAccounts.AsNoTracking().FirstOrDefault(b=>b.Id == transaction.BankAccountId);
            var currentBal = bankAccount.CurrentBalance;
            if (currentBal < 0)
            {
                var body = $"Your most recent transaction in the amount of {transaction.Amount} has resulted in an overdraft on your account: {bankAccount.CurrentBalance}.";
                CreateNotification(transaction, "Overdrafted!!", body);
            }

            else if (currentBal > 0 && currentBal <= bankAccount.LowBalanceLevel)
            {
                var body = $"Your most recent transaction in the amount of {transaction.Amount} has triggered a low balance warning on your account: {bankAccount.CurrentBalance}.";
                CreateNotification(transaction, "Warning! Low Balance!!", body);
            }
        }

        private static void CreateNotification(Transaction transaction, string subject, string body)
        {
            var houseId = db.Users.Find(transaction.OwnerId).HouseholdId ?? 0;
            var notification = new Notification
            {
                Body = body,
                Created = DateTime.Now,
                HouseholdId = houseId,
                IsRead = false,
                RecipientId = transaction.OwnerId,
                Subject = subject
            };
            db.Notifications.Add(notification);
            db.SaveChanges();
        }

        private static void UpdateBankBalance(Transaction transaction)
        {
            var bank = db.BankAccounts.Find(transaction.BankAccountId);
            if (transaction.TransactionType == TransactionType.Deposit)
                bank.CurrentBalance += transaction.Amount;
            else
                bank.CurrentBalance -= transaction.Amount;

            db.SaveChanges();
        }

        private static void UpdateBudgetBalances(Transaction transaction)
        {
            if (transaction.TransactionType == TransactionType.Deposit || transaction.BudgetItemId == null)
                return;

            var budgetItem = db.BudgetItems.Find(transaction.BudgetItemId);
            budgetItem.CurrentAmount += transaction.Amount;
            db.SaveChanges();
        }

        private static void UpdateBudgetItemBalance(Transaction transaction)
        {
            if (transaction.TransactionType == TransactionType.Deposit || transaction.BudgetItemId == null)
                return;

            var budgetItem = db.BudgetItems.Find(transaction.BudgetItemId);
            budgetItem.CurrentAmount += transaction.Amount;
            db.SaveChanges();
        }
    }
}