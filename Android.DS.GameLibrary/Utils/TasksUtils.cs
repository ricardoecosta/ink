#if WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Tasks;

namespace GameLibrary.Utils
{
    public class TasksUtils
    {
        public static void ShareLink(string title, string msg, string linkUri)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.Title = title;
            shareLinkTask.LinkUri = new Uri(linkUri);
            shareLinkTask.Message = msg;
            shareLinkTask.Show();
        }

        public static void OpenLinkOnBrowser(String link)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(link);
            webBrowserTask.Show();
        }

        public static void SearchOnApplicationMarket(String searchTerms)
        {
            MarketplaceSearchTask searchMarketplaceTask = new MarketplaceSearchTask();
            searchMarketplaceTask.ContentType = MarketplaceContentType.Applications;
            searchMarketplaceTask.SearchTerms = searchTerms;
            searchMarketplaceTask.Show();
        }
    }
}
#endif