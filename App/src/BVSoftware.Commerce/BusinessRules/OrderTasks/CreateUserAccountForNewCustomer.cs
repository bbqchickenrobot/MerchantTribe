﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BVSoftware.Commerce.Membership;
using BVSoftware.Commerce.Content;
using BVSoftware.Commerce.Utilities;

namespace BVSoftware.Commerce.BusinessRules.OrderTasks
{
    public class CreateUserAccountForNewCustomer : BusinessRules.OrderTask
    {

        public override bool Execute(OrderTaskContext context)
        {
            CustomerAccount u = context.BVApp.MembershipServices.Customers.FindByEmail(context.Order.UserEmail);
            if (u != null)
            {
                if (u.Bvin != string.Empty)
                {
                    return true;
                }
            }

            CustomerAccount n = new CustomerAccount();
            n.Email = context.Order.UserEmail;
            int length = WebAppSettings.PasswordMinimumLength;
            if (length < 8) length = 8;
            string newPassword = MerchantTribe.Web.PasswordGenerator.GeneratePassword(length);
            n.Password = newPassword;
            n.FirstName = context.Order.ShippingAddress.FirstName;
            n.LastName = context.Order.ShippingAddress.LastName;
                        
            if (context.BVApp.MembershipServices.CreateCustomer(n, n.Password))
            {
                // Update Addresses for Customer
                context.BVApp.MembershipServices.CustomerSetBillingAddress(n, context.Order.BillingAddress);
                context.BVApp.MembershipServices.CustomerSetShippingAddress(n, context.Order.ShippingAddress);
                context.BVApp.MembershipServices.UpdateCustomer(n);

                // Email Password to Customer
                HtmlTemplate t = context.BVApp.ContentServices.GetHtmlTemplateOrDefault(HtmlTemplateType.ForgotPassword);                
                if (t != null)
                {
                    System.Net.Mail.MailMessage m;

                    List<IReplaceable> replacers = new List<IReplaceable>();
                    replacers.Add(n);
                    replacers.Add(new Replaceable("[[NewPassword]]", newPassword));
                    t = t.ReplaceTagsInTemplate(context.BVApp, replacers);

                    m = t.ConvertToMailMessage(n.Email);
                    
                    if (MailServices.SendMail(m) == false)
                    {
                        EventLog.LogEvent("Create Account During Checkout", "Failed to send email to new customer " + n.Email, Metrics.EventLogSeverity.Warning);
                    }
                }
            }
            context.UserId = n.Bvin;
                                    
            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {            
            return true;
        }

        public override string TaskId()
        {
            return "1755C649-4C16-41A6-B5AE-5259067FFF0E";
        }

        public override string TaskName()
        {
            return "Create User Account for New Customer";
        }

        public override string StepName()
        {
            string result = string.Empty;
            result = "Create User Account for New Customer";
            if (result == string.Empty)
            {
                result = this.TaskName();
            }
            return result;
        }

        public override Task Clone()
        {
            return new CreateUserAccountForNewCustomer();
        }

    }
}
