using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dc_portal.Enums;

namespace dc_portal.Models
{

    public class ConfigVM
    {
        public int HouseHoldId { get; set; }

        //for bank accounts
        public string Name { get; set; }
        public float StartingBalance {get;set; }
        public float LowBalance { get;set;}
        public AccountType AccountType { get;set;}

        //budget
        public string BudgetName {get; set;}


        //Budget items
        public string BIName { get; set; }
        public float TargetAmount { get; set; }
    }


}