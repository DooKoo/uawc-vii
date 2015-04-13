using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Xml.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq;

namespace uwcua
{
    public class SiteMapBuilder
    {
        private String MainPageUrl;
        private ConcurrentBag<string> UrlsCollection;
        private List<Task> TasksPool;
        public string FilePath { get; private set; }
        public SiteMapBuilder(string siteUrl)
        {
            //Init main storages;
            UrlsCollection = new ConcurrentBag<string>();
            TasksPool = new List<Task>();


            //Convert main url to represented view;
            if (siteUrl.Last() != '/')
                siteUrl += "/";

            if (!siteUrl.StartsWith("http://") && !siteUrl.StartsWith("https://"))
                siteUrl = "http://" + siteUrl;

            if (siteUrl.IndexOf('/', 8) != siteUrl.LastIndexOf('/')) 
                siteUrl = siteUrl.Remove(siteUrl.IndexOf('/', 8) + 1);

            MainPageUrl = siteUrl;

            //Traverse main page;
            TraverseSite(MainPageUrl, 1);

            //Start traverse all pages with deep=2;
            TasksPool.ForEach((task) => task.Start());
            Task.WaitAll(TasksPool.ToArray());

            //Write data to file;
            WriteXml();
        }

        /// <summary>
        /// Recursive traverse of site and fill UrlsDictionary;
        /// </summary>
        /// <param name="pageUrl">Url of current traversed page</param>
        /// <param name="deep">Depth of recursion</param>
        private void TraverseSite(string pageUrl, int deep)
        {
            if (deep <= 3 && !UrlsCollection.Contains(pageUrl))
            {
                UrlsCollection.Add(pageUrl);
                //Select all <a href...> nodes;
                var aNodes = LoadPage(pageUrl).DocumentNode.SelectNodes("//a");
                if (aNodes != null)
                {
                    foreach (var link in aNodes)
                    {
                        String href = link.GetAttributeValue("href", "");
                        href = ConvertHref(href);
                        if (href != null && href.StartsWith(MainPageUrl))
                        {
                            try
                            {
                                if (deep == 1)
                                {
                                    Task traverseTask = new Task(new Action(() => { TraverseSite(href, deep + 1); }));
                                    TasksPool.Add(traverseTask);
                                }
                                else
                                    TraverseSite(href, deep + 1);   
                            }
                            catch (System.Net.WebException) { }
                            catch (UriFormatException) { }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert reference to represented view, like a http://www.exmple.com/level1/level2 ;
        /// </summary>
        /// <param name="href">Reference that will be represented</param>
        /// <returns>Represented link</returns>
        private string ConvertHref(string href)
        {
            if (href.Length != 0)
            {
                if (href.Length != 1 && href.Substring(0, 2) == "//")
                {
                    href = href.Substring(2);
                }
                else if (href[0] == '/')
                {
                    href = MainPageUrl + href.Substring(1);
                }
                return href;
            }
            return null;
        }

        /// <summary>
        /// Load Html page;
        /// </summary>
        /// <param name="url">Url of page that will be loaded</param>
        /// <returns>Html document that was loaded</returns>
        private HtmlDocument LoadPage(string url)
        {
            HtmlWeb web = new HtmlWeb();
            try
            {
                HtmlDocument page = web.Load(url);
                return page;
            }
            catch (System.Net.WebException e)
            {
                throw new System.Net.WebException(e.Message, e);
            }
            
        }

        /// <summary>
        /// Write a data from Collection to file;
        /// </summary>
        private void WriteXml()
        {
            XNamespace xmlScheme = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XDocument siteMap = new XDocument(new XElement(xmlScheme + "urlxset"));
            foreach (string url in UrlsCollection)
            {
                siteMap.Root.Add(new XElement(
                        xmlScheme + "url",
                        new XElement(xmlScheme + "loc", url),
                        new XElement(xmlScheme + "priority", GetPriority(url))));
                    
            }
            FilePath = "Xmls/" + MainPageUrl.GetHashCode() + ".xml";
            siteMap.Save(FilePath);
        }

        /// <summary>
        /// Get page priority that based on count of '/' in url;
        /// </summary>
        /// <param name="url">Url of page that priority will be counted</param>
        /// <returns>Priority of page</returns>
        private double GetPriority(string url)
        {
            return Math.Round(1.0 / (url.Count((sym) => sym == '/') - 2), 1);
        }
    }
}