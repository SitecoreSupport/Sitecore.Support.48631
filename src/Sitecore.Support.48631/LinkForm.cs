using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Shell.Applications;
using Sitecore.Web;

namespace Sitecore.Support.Shell.Applications.Dialogs
{
    public class LinkForm: Sitecore.Shell.Applications.Dialogs.LinkForm
    {
        protected override string GetLink()
        {
            UrlHandle urlHandle = UrlHandle.Get();
            return StringUtil.GetString(new string[]
            {
                urlHandle["va"],
                "<link/>"
            });
        }
    }
}
