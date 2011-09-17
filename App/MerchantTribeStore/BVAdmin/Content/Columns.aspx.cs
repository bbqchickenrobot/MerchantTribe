using System;
using System.Web;
using System.Web.UI.WebControls;
using BVSoftware.Commerce.Membership;
using BVSoftware.Commerce.Content;
using BVSoftware.Commerce;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace BVCommerce
{

    partial class BVAdmin_Content_Columns : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            this.PageTitle = "Content Columns";
            this.CurrentTab = AdminTabType.Content;
            ValidateCurrentUserHasPermission(SystemPermissions.ContentView);
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadColumns();
            }
        }

        private void LoadColumns()
        {
            List<ContentColumn> cols;
            cols = BVApp.ContentServices.Columns.FindAll();
            this.GridView1.DataSource = cols;
            this.GridView1.DataBind();
            this.lblResults.Text = cols.Count + " columns found";
        }

        protected void GridView1_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            msg.ClearMessage();

            string bvin = (string)GridView1.DataKeys[e.RowIndex].Value;
            if (BVApp.ContentServices.Columns.Delete(bvin) == false)
            {
                this.msg.ShowWarning("Unable to delete this column. System columns can not be deleted.");
            }

            LoadColumns();
        }

        protected void btnNew_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            msg.ClearMessage();

            if (this.NewNameField.Text.Trim().Length < 1)
            {
                msg.ShowWarning("Please enter a name for the new column.");
            }
            else
            {
                ContentColumn c = new ContentColumn();
                c.DisplayName = this.NewNameField.Text.Trim();
                c.SystemColumn = false;
                if (BVApp.ContentServices.Columns.Create(c) == true)
                {
                    Response.Redirect("Columns_Edit.aspx?id=" + c.Bvin);
                }
                else
                {
                    msg.ShowError("Unable to create column. Please see event log for details");
                    EventLog.LogEvent("Create Content Column Button", "Unable to create column", BVSoftware.Commerce.Metrics.EventLogSeverity.Error);
                }
            }

        }

        protected void GridView1_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            string bvin = (string)GridView1.DataKeys[e.NewEditIndex].Value;
            Response.Redirect("Columns_edit.aspx?id=" + bvin);
        }

    }
}