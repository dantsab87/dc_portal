using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dc_portal.Models;

namespace dc_portal.Helpers
{
    public class InvitationHelper
    {

        public static void MarkAsInvalid(int Id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var invitation = db.Invitations.Find(Id);
            invitation.IsValid = false;
            db.SaveChanges();
        }
    }
}