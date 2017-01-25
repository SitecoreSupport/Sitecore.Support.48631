using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.WebControls;
using Sitecore.Xml;
using System;

namespace Sitecore.Support.Shell.Applications.Dialogs.InternalLink
{
    /// <summary>
    /// Represents a InternalLinkForm.
    /// </summary>
    public class InternalLinkForm : LinkForm
    {
        /// <summary>
        /// The anchor.
        /// </summary>
        protected Edit Anchor;

        /// <summary>
        /// The class.
        /// </summary>
        protected Edit Class;

        /// <summary>
        /// The custom label.
        /// </summary>
        protected Panel CustomLabel;

        /// <summary>
        /// The custom target.
        /// </summary>
        protected Edit CustomTarget;

        /// <summary>
        /// The internal link data context.
        /// </summary>
        protected DataContext InternalLinkDataContext;

        /// <summary>
        /// The querystring.
        /// </summary>
        protected Edit Querystring;

        /// <summary>
        /// The target.
        /// </summary>
        protected Combobox Target;

        /// <summary>
        /// The text.
        /// </summary>
        protected Edit Text;

        /// <summary>
        /// The title.
        /// </summary>
        protected Edit Title;

        /// <summary>
        /// The treeview.
        /// </summary>
        protected TreeviewEx Treeview;

        /// <summary>
        /// Called when the listbox has changed.
        /// </summary>
        protected void OnListboxChanged()
        {
            if (this.Target.Value == "Custom")
            {
                this.CustomTarget.Disabled = false;
                this.CustomLabel.Disabled = false;
                return;
            }
            this.CustomTarget.Value = string.Empty;
            this.CustomTarget.Disabled = true;
            this.CustomLabel.Disabled = true;
        }

        /// <summary>
        /// Raises the load event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs" /> instance containing the event data.
        /// </param>
        /// <remarks>
        /// This method notifies the server control that it should perform actions common to each HTTP
        /// request for the page it is associated with, such as setting up a database query. At this
        /// stage in the page lifecycle, server controls in the hierarchy are created and initialized,
        /// view state is restored, and form controls reflect client-side data. Use the IsPostBack
        /// property to determine whether the page is being loaded in response to a client postback,
        /// or if it is being loaded and accessed for the first time.
        /// </remarks>
        protected override void OnLoad(System.EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
            {
                return;
            }
            this.InternalLinkDataContext.GetFromQueryString();
            this.CustomTarget.Disabled = true;
            this.CustomLabel.Disabled = true;
            string queryString = WebUtil.GetQueryString("ro");
            string text = base.LinkAttributes["url"];
            string value = string.Empty;
            string text2 = base.LinkAttributes["target"];
            string linkTargetValue = LinkForm.GetLinkTargetValue(text2);
            if (linkTargetValue == "Custom")
            {
                value = text2;
                this.CustomTarget.Disabled = false;
                this.CustomLabel.Disabled = false;
                this.CustomTarget.Background = "window";
            }
            this.Text.Value = base.LinkAttributes["text"];
            this.Anchor.Value = base.LinkAttributes["anchor"];
            this.Target.Value = linkTargetValue;
            this.CustomTarget.Value = value;
            this.Class.Value = base.LinkAttributes["class"];
            this.Querystring.Value = base.LinkAttributes["querystring"];
            this.Title.Value = base.LinkAttributes["title"];
            string text3 = base.LinkAttributes["id"];
            if (string.IsNullOrEmpty(text3) || !ID.IsID(text3))
            {
                this.SetFolderFromUrl(text);
            }
            else
            {
                ID iD = new ID(text3);
                if (Client.ContentDatabase.GetItem(iD, this.InternalLinkDataContext.Language) == null && !string.IsNullOrWhiteSpace(text))
                {
                    this.SetFolderFromUrl(text);
                }
                else
                {
                    ItemUri folder = new ItemUri(iD, this.InternalLinkDataContext.Language, Client.ContentDatabase);
                    this.InternalLinkDataContext.SetFolder(folder);
                }
            }
            if (queryString.Length > 0)
            {
                this.InternalLinkDataContext.Root = queryString;
            }
        }

        /// <summary>
        /// Handles a click on the OK button.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="args">
        /// </param>
        /// <remarks>
        /// When the user clicks OK, the dialog is closed by calling
        /// the <see cref="M:Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow">CloseWindow</see> method.
        /// </remarks>
        protected override void OnOK(object sender, System.EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            Item selectionItem = this.Treeview.GetSelectionItem();
            if (selectionItem == null)
            {
                Context.ClientPage.ClientResponse.Alert("Select an item.");
                return;
            }
            string linkTargetAttributeFromValue = LinkForm.GetLinkTargetAttributeFromValue(this.Target.Value, this.CustomTarget.Value);
            string text = this.Querystring.Value;
            if (text.StartsWith("?", System.StringComparison.InvariantCulture))
            {
                text = text.Substring(1);
            }
            Packet packet = new Packet("link", new string[0]);
            LinkForm.SetAttribute(packet, "text", this.Text);
            LinkForm.SetAttribute(packet, "linktype", "internal");
            LinkForm.SetAttribute(packet, "anchor", this.Anchor);
            LinkForm.SetAttribute(packet, "querystring", this.Anchor);
            LinkForm.SetAttribute(packet, "title", this.Title);
            LinkForm.SetAttribute(packet, "class", this.Class);
            LinkForm.SetAttribute(packet, "querystring", text);
            LinkForm.SetAttribute(packet, "target", linkTargetAttributeFromValue);
            LinkForm.SetAttribute(packet, "id", selectionItem.ID.ToString());
            Assert.IsTrue(!string.IsNullOrEmpty(selectionItem.ID.ToString()) && ID.IsID(selectionItem.ID.ToString()), "ID doesn't exist.");
            Context.ClientPage.ClientResponse.SetDialogValue(packet.OuterXml);
            base.OnOK(sender, args);
        }

        /// <summary>
        /// The set folder from url.
        /// </summary>
        /// <param name="url">The url.</param>
        private void SetFolderFromUrl(string url)
        {
            Assert.ArgumentNotNull(url, "url");
            if (base.LinkType != "internal")
            {
                url = "/sitecore/content" + Settings.DefaultItem;
            }
            if (url.Length == 0)
            {
                url = "/sitecore/content";
            }
            if (!url.StartsWith("/sitecore", System.StringComparison.InvariantCulture))
            {
                url = "/sitecore/content" + url;
            }
            this.InternalLinkDataContext.Folder = url;
        }
    }
}
