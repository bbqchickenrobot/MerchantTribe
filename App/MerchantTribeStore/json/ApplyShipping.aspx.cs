
using System;
using BVSoftware.Commerce;
using BVSoftware.Commerce.Orders;

namespace BVCommerce
{
    partial class json_ApplyShipping : BaseStoreJsonPage
    {

        public override bool RequiresSSL
        {
            get { return true; }
        }

        public class JsonResponse
        {
            public string totalsastable = string.Empty;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            JsonResponse result = new JsonResponse();

            string rateKey = Request.Form["MethodId"];
            string orderid = Request.Form["OrderId"];
            if (rateKey == null)
            {
                rateKey = "";
            }
            if (orderid == null)
            {
                orderid = "";
            }

            
            Order o = BVApp.OrderServices.Orders.FindForCurrentStore(orderid);
            BVApp.OrderServices.OrdersRequestShippingMethodByUniqueKey(rateKey, o);
            BVApp.CalculateOrderAndSave(o);
            SessionManager.SaveOrderCookies(o);

            result.totalsastable = o.TotalsAsTable();

            //System.Threading.Thread.Sleep(500)
            this.litOutput.Text = MerchantTribe.Web.Json.ObjectToJson(result);
        }

    }
}