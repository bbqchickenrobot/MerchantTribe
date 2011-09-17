using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using BVSoftware.Commerce;
using BVSoftware.Commerce.Catalog;
using BVSoftware.Commerce.Content;
using BVSoftware.Search;

namespace BVCommerce
{

    partial class Search : BaseSearchPage
    {

        protected override void OnPreInit(System.EventArgs e)
        {
            base.OnPreInit(e);
            this.Title = SiteTerms.GetTerm(SiteTermIds.Search);
        }

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
            this.AddBodyClass("store-search-page");
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            int itemsPerPage = 9;

            Pager1.ItemsPerPage = itemsPerPage;
            Pager2.ItemsPerPage = itemsPerPage;

            string query = Request.QueryString["q"];
            if ((query == null))
            {
                query = string.Empty;
            }
            if (!Page.IsPostBack)
            {
                this.btnGo.ImageUrl = BVApp.ThemeManager().ButtonUrl("Go", Request.IsSecureConnection);
                this.q.Text = query;
            }

            Pager1.BaseUrl = Page.ResolveUrl("~/search.aspx?q=" + Server.UrlEncode(query) + "&p={page}");
            Pager2.BaseUrl = Pager1.BaseUrl; // Page.ResolveUrl("~/search.aspx?q=" + Server.UrlEncode(query) + "&p={page}");

            int pageNumber = 1;
            if ((Request.QueryString["p"] != null))
            {
                int tryPage = 0;
                if (int.TryParse(Request.QueryString["p"], out tryPage))
                {
                    pageNumber = tryPage;
                }
            }
            Pager1.CurrentPage = pageNumber;
            Pager2.CurrentPage = pageNumber;

            int totalResults = 0;

            SearchManager manager = new SearchManager();
            List<SearchObject> objects = manager.DoSearch(BVApp.CurrentStore.Id, query, pageNumber, itemsPerPage, ref totalResults);

            List<string> ids = new List<string>();            
            foreach (SearchObject o in objects)
            {
                switch (o.ObjectType)
                {
                    case (int)SearchManagerObjectType.Product:
                        ids.Add(o.ObjectId);                        
                        break;
                }
            }
            List<Product> products = BVApp.CatalogServices.Products.FindMany(ids);

            if ((products != null))
            {
                RenderProducts(products);
                this.Pager1.RowCount = totalResults;
                this.Pager2.RowCount = totalResults;
            }

            this.q.Focus();
        }

        private void RenderProducts(List<Product> displayProducts)
        {

            StringBuilder sb = new StringBuilder();

            int columnCount = 1;

            foreach (Product p in displayProducts)
            {

                bool isLastInRow = false;
                bool isFirstInRow = false;
                if ((columnCount == 1))
                {
                    isFirstInRow = true;
                }

                if ((columnCount == 3))
                {
                    isLastInRow = true;
                    columnCount = 1;
                }
                else
                {
                    columnCount += 1;
                }
                UserSpecificPrice price = BVApp.PriceProduct(p, BVApp.CurrentCustomer, null);
                BVSoftware.Commerce.Utilities.HtmlRendering.RenderSingleProduct(ref sb, p, isLastInRow, isFirstInRow, this.Page, price);
            }

            this.litSearchResults.Text = sb.ToString();
        }

        protected void btnGo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string destination = "~/search?q=" + Server.UrlEncode(q.Text.Trim()) + "&p=1";
            Response.Redirect(destination);
        }

    }
}