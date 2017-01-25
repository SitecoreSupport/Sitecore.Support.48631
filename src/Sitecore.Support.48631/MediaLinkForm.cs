using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Shell.Framework;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Xml;
using System;

namespace Sitecore.Support.Shell.Applications.Dialogs.MediaLink
{
    /// <summary>
    /// Link of type media.
    /// </summary>
    public class MediaLinkForm : LinkForm
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
        /// The media link data context.
        /// </summary>
        protected DataContext MediaLinkDataContext;

        /// <summary>
        /// The media link treeview.
        /// </summary>
        protected TreeviewEx MediaLinkTreeview;

        /// <summary>
        /// The preview.
        /// </summary>
        protected Border Preview;

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
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            Item item = null;
            if (message.Arguments.Count > 0 && ID.IsID(message.Arguments["id"]))
            {
                IDataView dataView = this.MediaLinkTreeview.GetDataView();
                if (dataView != null)
                {
                    item = dataView.GetItem(message.Arguments["id"]);
                }
            }
            if (item == null)
            {
                item = this.MediaLinkTreeview.GetSelectionItem();
            }
            Dispatcher.Dispatch(message, item);
            base.HandleMessage(message);
        }

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
            this.MediaLinkDataContext.GetFromQueryString();
            string text = base.LinkAttributes["target"];
            string value = string.Empty;
            string linkTargetValue = LinkForm.GetLinkTargetValue(text);
            if (linkTargetValue == "Custom")
            {
                value = text;
                this.CustomTarget.Disabled = false;
                this.CustomLabel.Disabled = false;
            }
            this.Text.Value = base.LinkAttributes["text"];
            this.Target.Value = linkTargetValue;
            this.CustomTarget.Value = value;
            this.Class.Value = base.LinkAttributes["class"];
            this.Title.Value = base.LinkAttributes["title"];
            string text3;
            if (base.LinkType == "media")
            {
                string text2 = base.LinkAttributes["id"];
                if (!string.IsNullOrEmpty(text2) && ID.IsID(text2))
                {
                    Item item = Client.ContentDatabase.GetItem(new ID(text2));
                    if (item != null && item.ID != ItemIDs.MediaLibraryRoot)
                    {
                        text3 = item.Paths.Path;
                        if (text3.StartsWith("/sitecore/media library", System.StringComparison.InvariantCulture))
                        {
                            text3 = text3.Substring("/sitecore/media library".Length);
                        }
                        if (this.MediaLinkTreeview.GetDataView() == null)
                        {
                            return;
                        }
                        if (item.Parent != null)
                        {
                            this.MediaLinkDataContext.SetFolder(item.Uri);
                        }
                    }
                    else
                    {
                        text3 = "/sitecore/media library";
                    }
                }
                else
                {
                    text3 = "/sitecore/media library";
                }
            }
            else
            {
                text3 = "/sitecore/media library";
            }
            this.MediaLinkDataContext.AddSelected(new DataUri(text3));
            this.MediaLinkDataContext.Root = "/sitecore/media library";
            this.UpdatePreview(this.MediaLinkDataContext.GetFolder());
        }

        /// <summary>
        /// Called when the new has folder.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [HandleMessage("medialink:newfolder")]
        protected void OnNewFolder(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            Item folder = this.MediaLinkDataContext.GetFolder();
            if (folder != null)
            {
                Items.NewFolder(folder);
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
            Item selectionItem = this.MediaLinkTreeview.GetSelectionItem();
            if (selectionItem == null)
            {
                Context.ClientPage.ClientResponse.Alert("Select a media item.");
                return;
            }
            string arg_46_0 = selectionItem.Paths.MediaPath;
            string linkTargetAttributeFromValue = LinkForm.GetLinkTargetAttributeFromValue(this.Target.Value, this.CustomTarget.Value);
            Packet packet = new Packet("link", new string[0]);
            LinkForm.SetAttribute(packet, "text", this.Text);
            LinkForm.SetAttribute(packet, "linktype", "media");
            LinkForm.SetAttribute(packet, "title", this.Title);
            LinkForm.SetAttribute(packet, "class", this.Class);
            LinkForm.SetAttribute(packet, "target", linkTargetAttributeFromValue);
            LinkForm.SetAttribute(packet, "id", selectionItem.ID.ToString());
            SheerResponse.SetDialogValue(packet.OuterXml);
            base.OnOK(sender, args);
        }

        /// <summary>
        /// Called when this instance has open.
        /// </summary>
        protected void OnOpen()
        {
            Item selectionItem = this.MediaLinkTreeview.GetSelectionItem();
            if (selectionItem != null && selectionItem.HasChildren)
            {
                this.MediaLinkDataContext.SetFolder(selectionItem.Uri);
            }
        }

        /// <summary>
        /// Selects the tree node.
        /// </summary>
        protected void SelectTreeNode()
        {
            Item selectionItem = this.MediaLinkTreeview.GetSelectionItem();
            if (selectionItem == null)
            {
                return;
            }
            this.UpdatePreview(selectionItem);
        }

        /// <summary>
        /// Uploads the image.
        /// </summary>
        protected void UploadImage()
        {
            Item selectionItem = this.MediaLinkTreeview.GetSelectionItem();
            if (selectionItem != null)
            {
                if (!selectionItem.Access.CanCreate())
                {
                    SheerResponse.Alert("You do not have permission to create a new item here.", new string[0]);
                    return;
                }
                Context.ClientPage.SendMessage(this, "media:upload(edit=1,load=1," + State.Client.UsesBrowserWindowsQueryParameterName + "=1)");
            }
        }

        /// <summary>
        /// Updates the preview.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        private void UpdatePreview(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            MediaUrlOptions thumbnailOptions = MediaUrlOptions.GetThumbnailOptions(item);
            thumbnailOptions.UseDefaultIcon = true;
            thumbnailOptions.Width = 96;
            thumbnailOptions.Height = 96;
            thumbnailOptions.Language = item.Language;
            thumbnailOptions.AllowStretch = false;
            string mediaUrl = MediaManager.GetMediaUrl(item, thumbnailOptions);
            this.Preview.InnerHtml = "<img src=\"" + mediaUrl + "\" width=\"96\" height=\"96\" border=\"0\" alt=\"\" />";
        }
    }
}
