﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MerchantTribe.Commerce;
using System.Text;

namespace MerchantTribeStore.code.TemplateEngine.TagHandlers
{
    public class AdminPanel : ITagHandler
    {
        public string TagName
        {
            get { return "sys:adminpanel"; }
        }

        public void Process(StringBuilder output, 
                            MerchantTribeApplication app, 
                            dynamic viewBag, 
                            ITagProvider tagProvider, 
                            ParsedTag tag, string contents)
        {
            Render(output, app, viewBag);
        }

        private string AddSecureRoot(dynamic viewBag, string baseUrl)
        {
            return (string)viewBag.RootUrlSecure + baseUrl;
        }

        private string StoreClosedLink(dynamic viewBag)
        {
            string result = string.Empty;
            if (viewBag.StoreClosed == true)
            {
               result = "<a href=\"" + AddSecureRoot(viewBag, "bvadmin/configuration/general.aspx") + "\" class=\"red\">";
               result += "*** STORE IS CLOSED, SHOPPERS CAN'T SEE THIS PAGE ***</a>";
            }
            return result;
        }

        public void Render(StringBuilder sb, MerchantTribeApplication app, dynamic viewBag)
        {            
            if (viewBag.IsAdmin == true)
            {
                sb.Append("<div id=\"adminpanel\">");
                sb.Append("   <a id=\"adminpanellogo\" href=\"" + AddSecureRoot(viewBag, "bvadmin") + "\"><img src=\"" + app.StoreUrl(false, false) + "images/system/AdminPanelLogo.png\" alt=\"MerchantTribeStore\" /></a>");
                sb.Append(StoreClosedLink(viewBag));
                sb.Append("   <a href=\"" + AddSecureRoot(viewBag, "bvadmin") + "\" class=\"right\">Go To Admin Dashboard</a>");
                sb.Append("</div>");
            }         
        }
    }
}
