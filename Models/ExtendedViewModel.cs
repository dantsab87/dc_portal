using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dc_portal.Models
{
    public class AcceptInvitationViewModel : ExtendedRegisterViewModel
    {
        public int Id { get; set; }
        public Guid Code { get; set; }
        public int HouseholdId { get; set; }
    }
}