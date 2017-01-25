using Sitecore.Diagnostics;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Xml;
using System;
using System.Text.RegularExpressions;

namespace Sitecore.Support.Shell.Applications.Dialogs.MailLink
{
    /// <summary>
    /// Represents a MailLinkForm.
    /// </summary>
    public class MailLinkForm : LinkForm
    {
        /// <summary>
        /// The class.
        /// </summary>
        protected Edit Class;

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
            if (base.LinkType != "mailto")
            {
                value = string.Empty;
            }
            this.Text.Value = base.LinkAttributes["text"];
            this.Url.Value = value;
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
            string mail = this.GetMail();
            if (mail == "__Canceled")
            {
                SheerResponse.Alert("The e-mail address is invalid.", new string[0]);
                return;
            }
            Packet packet = new Packet("link", new string[0]);
            LinkForm.SetAttribute(packet, "text", this.Text);
            LinkForm.SetAttribute(packet, "linktype", "mailto");
            LinkForm.SetAttribute(packet, "url", mail);
            LinkForm.SetAttribute(packet, "anchor", string.Empty);
            LinkForm.SetAttribute(packet, "title", this.Title);
            LinkForm.SetAttribute(packet, "class", this.Class);
            SheerResponse.SetDialogValue(packet.OuterXml);
            base.OnOK(sender, args);
        }

        /// <summary>
        /// Called when this instance has test.
        /// </summary>
        protected void OnTest()
        {
            string mail = this.GetMail();
            SheerResponse.Eval("scForm.browser.getControl('mail').href='" + mail + "'; scForm.browser.getControl('mail').click();");
        }

        /// <summary>
        /// Gets the mail.
        /// </summary>
        /// <returns>
        /// The mail.
        /// </returns>
        /// <contract>
        ///   <ensures condition="not null" />
        /// </contract>
        private string GetMail()
        {
            string text = this.Url.Value;
            string text2 = text;
            if (text2.Length > 0)
            {
                if (text2.IndexOf(":", System.StringComparison.InvariantCulture) >= 0)
                {
                    text2 = text2.Substring(text2.IndexOf(":", System.StringComparison.InvariantCulture) + 1);
                }
                Regex regex = new Regex("^[A-Za-z0-9](([_\\.\\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\\.\\-]?[a-zA-Z0-9]+)*)\\.([A-Za-z]{2,})$", RegexOptions.IgnoreCase);
                if (!regex.IsMatch(text2))
                {
                    return "__Canceled";
                }
            }
            if (text.Length > 0 && text.IndexOf(":", System.StringComparison.InvariantCulture) < 0)
            {
                text = "mailto:" + text;
            }
            return text;
        }
    }
}
