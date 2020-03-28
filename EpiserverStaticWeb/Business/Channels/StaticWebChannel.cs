using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace EpiserverStaticWeb.Business.Channels
{
    public class StaticWebChannel : DisplayChannel
    {
        public const string Name = "staticweb";

        public override string DisplayName => "StaticWeb";

        public override string ChannelName
        {
            get
            {
                return Name;
            }
        }

        public override bool IsActive(HttpContextBase context)
        {
            var userAgent = context.GetOverriddenBrowser().Browser;
            return userAgent != null && userAgent.Contains("StaticWebPlugin");
        }
    }
}