using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Xml;
using System;

namespace Sitecore.Support.Shell.Applications.Dialogs.ExternalLink
{
    /// <summary>
    /// Represents a ExternalLinkForm.
    /// </summary>
    public class ExternalLinkForm : LinkForm
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
        /// The url.
        /// </summary>
        protected Edit Url;

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
            string value = base.LinkAttributes["url"];
            if (base.LinkType != "external")
            {
                value = string.Empty;
            }
            string value2 = string.Empty;
            string text = base.LinkAttributes["target"];
            string linkTargetValue = LinkForm.GetLinkTargetValue(text);
            if (linkTargetValue == "Custom")
            {
                value2 = text;
                this.CustomTarget.Disabled = false;
                this.CustomLabel.Disabled = false;
            }
            this.Text.Value = base.LinkAttributes["text"];
            this.Url.Value = value;
            this.Target.Value = linkTargetValue;
            this.CustomTarget.Value = value2;
            this.Class.Value = base.LinkAttributes["class"];
            this.Title.Value = base.LinkAttributes["title"];
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
            string path = this.GetPath();
            string linkTargetAttributeFromValue = LinkForm.GetLinkTargetAttributeFromValue(this.Target.Value, this.CustomTarget.Value);
            Packet packet = new Packet("link", new string[0]);
            LinkForm.SetAttribute(packet, "text", this.Text);
            LinkForm.SetAttribute(packet, "linktype", "external");
            LinkForm.SetAttribute(packet, "url", path);
            LinkForm.SetAttribute(packet, "anchor", string.Empty);
            LinkForm.SetAttribute(packet, "title", this.Title);
            LinkForm.SetAttribute(packet, "class", this.Class);
            LinkForm.SetAttribute(packet, "target", linkTargetAttributeFromValue);
            Context.ClientPage.ClientResponse.SetDialogValue(packet.OuterXml);
            base.OnOK(sender, args);
        }

        /// <summary>
        /// Called when this instance has test.
        /// </summary>
        protected void OnTest()
        {
            string path = this.GetPath();
            if (path.Length > 0)
            {
                Context.ClientPage.ClientResponse.Eval(string.Concat(new string[]
                {
                    "try {window.open('",
                    path,
                    "', '_blank') } catch(e) { alert('",
                    Translate.Text("An error occured:"),
                    " ' + e.description) }"
                }));
            }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <returns>
        /// The path.
        /// </returns>
        /// <contract>
        ///   <ensures condition="not null" />
        /// </contract>
        private string GetPath()
        {
            string text = this.Url.Value;
            if (text.Length > 0 && text.IndexOf("://", System.StringComparison.InvariantCulture) < 0 && !text.StartsWith("/", System.StringComparison.InvariantCulture))
            {
                text = "http://" + text;
            }
            return text;
        }
    }
}
