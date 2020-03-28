using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EpiserverStaticWeb.Business
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class StaticWebEventInit : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            events.PublishedContent += OnPublishedContent;
        }

        private void OnPublishedContent(object sender, EPiServer.ContentEventArgs e)
        {
            var httpContext = HttpContext.Current;

            if (httpContext == null)
                return;

            var httpContextBase = new HttpContextWrapper(httpContext);

            if (e.Content is PageData)
            {
                var contentLink = e.ContentLink;

                GeneratePage(contentLink);
            }
            else if (e.Content is BlockData)
            {
                var block = e.Content as BlockData;
                var repository = ServiceLocator.Current.GetInstance<IContentRepository>();
                var references = repository.GetReferencesToContent(e.ContentLink, true).ToList();
                var pages = GetPageReferencesToContent(repository, e.ContentLink);

                foreach (var page in pages)
                {
                    GeneratePage(page.ContentLink);
                }
            }
        }

        private static void GeneratePage(ContentReference contentLink)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            var orginalUrl = urlResolver.GetUrl(contentLink);
            if (orginalUrl == null)
                return;

            var rootPath = @"C:\inetpub\example-site\EpiServerStaticWebExampleResultWebSite";

            var relativePath = orginalUrl.Replace("/", @"\");
            if (!relativePath.StartsWith(@"\"))
            {
                relativePath = @"\" + relativePath;
            }
            if (!relativePath.EndsWith(@"\"))
            {
                relativePath = relativePath + @"\";
            }
            relativePath = relativePath;

            System.Net.WebClient webClient = new System.Net.WebClient();
            var html = webClient.DownloadString("http://localhost:49822/" + orginalUrl);

            // TODO: make sure we have all resources for current page
            // <(script|link|img).*(href|src)="(?<resource>[^"]+)
            var matches = System.Text.RegularExpressions.Regex.Matches(html, "<(script|link|img).*(href|src)=\"(?<resource>[^\"]+)");
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var group = match.Groups["resource"];
                if (group.Success)
                {
                    var value = group.Value;
                    if (value.StartsWith("/"))
                    {
                        // TODO: check if local file or db file
                        // TODO: download resource
                        // NOTE: Temporary fix for approved file formats
                        if (value.EndsWith(".css")
                            || value.EndsWith(".js")
                            || value.EndsWith(".png")
                            || value.EndsWith(".jpg")
                            || value.EndsWith(".jpeg")
                            || value.EndsWith(".jpe")
                            || value.EndsWith(".gif")
                            || value.EndsWith(".bmp")
                            || value.EndsWith(".ico"))
                        {
                            System.Net.WebClient resourceClient = new System.Net.WebClient();
                            //byte[] data = resourceClient.DownloadData("http://localhost:49822/" + value);
                            // TODO: check file extension and change filename
                            //resourceClient.ResponseHeaders.

                            var directory = System.IO.Path.GetDirectoryName(rootPath + value.Replace("/", "\\"));
                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }

                            resourceClient.DownloadFile("http://localhost:49822/" + value, rootPath + value.Replace("/", "\\"));
                        }
                    }
                }
            }

            // TODO: make sure we have all source resources for current page
            // <(source).*(srcset)="(?<resource>[^"]+)"

            // TODO: make sure we have all meta resources for current page
            // Below matches ALL meta content that is a URL
            // <(meta).*(content)="(?<resource>(http:\/\/|https:\/\/|\/)[^"]+)"
            // Below matches ONLY known properties
            // <(meta).*(property|name)="(twitter:image|og:image)".*(content)="(?<resource>[http:\/\/|https:\/\/|\/][^"]+)"

            if (!System.IO.Directory.Exists(rootPath + relativePath))
            {
                System.IO.Directory.CreateDirectory(rootPath + relativePath);
            }

            System.IO.File.WriteAllText(rootPath + relativePath + "index.html", html);
        }

        public List<PageData> GetPageReferencesToContent(IContentRepository repository, ContentReference contentReference)
        {
            var list = GetPagesRecursively(repository, contentReference)
                .Filter(new FilterTemplate()) // exclude container pages
                .Filter(new FilterPublished()) // exclude unpublished pages
                .Distinct()
                .ToList();

            return list;
        }

        private IEnumerable<PageData> GetPagesRecursively(IContentRepository repository, ContentReference contentReference)
        {
            var references = repository.GetReferencesToContent(contentReference, true).ToList();
            foreach (var reference in references)
            {
                var content = repository.Get<IContent>(reference.OwnerID);

                // if content is PageData, return it
                var page = content as PageData;
                if (page != null)
                {
                    yield return page;
                }

                // if content is BlockData, return all pages where this block is used
                var block = content as BlockData;
                if (block != null)
                {
                    foreach (var x in GetPagesRecursively(repository, content.ContentLink))
                    {
                        yield return x;
                    }
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}