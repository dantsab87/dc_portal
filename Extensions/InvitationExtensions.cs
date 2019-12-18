using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using dc_portal.Models;

namespace dc_portal.Extensions
{
    public static class InvitationExtensions
    {
        public static async Task EmailInvitation(this Invitation invitation)
        {
            var Url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var callbackUrl = Url.Action("AcceptInvitation", "Account", new { recipientEmail = invitation.RecipientEmail, code = invitation.Code, Id = invitation.Id }, protocol: HttpContext.Current.Request.Url.Scheme);
                var from = $"The Golden Egg < {WebConfigurationManager.AppSettings["emailfrom"]}>";

            var emailMessage = new MailMessage(from, invitation.RecipientEmail)
            {
                Subject = $"You have been invited to join The Golden Egg Application!",
                Body = $"Please accept this invitation and register as a new household member <a href=\"{callbackUrl}\">here</a>.",
                IsBodyHtml = true
            };

            var svc = new PersonalEmail();
            await svc.SendAsync(emailMessage);
        }

    }
}