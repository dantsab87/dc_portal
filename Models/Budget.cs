using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dc_portal.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public string OwnerId { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public float TargetAmount { get; set; }
        public float CurrentAmount { get; set; }

        public virtual Household Household { get; set; }
        public virtual ApplicationUser Owner { get; set; }


        public virtual ICollection<BudgetItem> BudgetItems { get; set; }


        public Budget()
        {
            BudgetItems = new HashSet<BudgetItem>();
        }
    }
}