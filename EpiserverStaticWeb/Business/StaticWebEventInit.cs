using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

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

        private void OnPublishedContent(object sender, ContentEventArgs e)
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
            var rootUrl = "http://localhost:49822/";

            var relativePath = orginalUrl.Replace("/", @"\");
            if (!relativePath.StartsWith(@"\"))
            {
                relativePath = @"\" + relativePath;
            }
            if (!relativePath.EndsWith(@"\"))
            {
                relativePath = relativePath + @"\";
            }

            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            var html = webClient.DownloadString(rootUrl + orginalUrl);

            EnsurePageResources(rootUrl, rootPath, html);

            if (!Directory.Exists(rootPath + relativePath))
            {
                Directory.CreateDirectory(rootPath + relativePath);
            }

            File.WriteAllText(rootPath + relativePath + "index.html", html);
        }

        private static void EnsurePageResources(string rootUrl, string rootPath, string html)
        {
            // TODO: make sure we have all resources for current page
            // <(script|link|img).*(href|src)="(?<resource>[^"]+)
            var matches = Regex.Matches(html, "<(script|link|img).*(href|src)=\"(?<resource>[^\"]+)");
            foreach (Match match in matches)
            {
                var group = match.Groups["resource"];
                if (group.Success)
                {
                    var resourceUrl = group.Value;
                    EnsureResource(rootUrl, rootPath, resourceUrl);
                }
            }

            // TODO: make sure we have all source resources for current page
            // <(source).*(srcset)="(?<resource>[^"]+)"

            // TODO: make sure we have all meta resources for current page
            // Below matches ALL meta content that is a URL
            // <(meta).*(content)="(?<resource>(http:\/\/|https:\/\/|\/)[^"]+)"
            // Below matches ONLY known properties
            // <(meta).*(property|name)="(twitter:image|og:image)".*(content)="(?<resource>[http:\/\/|https:\/\/|\/][^"]+)"
        }

        private static void EnsureResource(string rootUrl, string rootPath, string resourceUrl)
        {
            if (resourceUrl.StartsWith("/"))
            {
                //switch (Path.GetExtension(resourceUrl).ToLower())
                //{
                //    case ".css":
                //        break;
                //    case ".js":
                //    case ".woff":
                //    case ".woff2":
                //    case ".png":
                //    case ".jpg":
                //    case ".jpeg":
                //    case ".jpe":
                //    case ".gif":
                //    case ".ico":
                //    case ".pdf":
                //        break;
                //    case ".bmp":
                //        break;
                //}
                // TODO: check if local file or db file
                // TODO: download resource
                if (resourceUrl.EndsWith(".css"))
                {
                    EnsureCssResources(rootUrl, rootPath, resourceUrl);
                }
                else if (resourceUrl.EndsWith(".js")
                || resourceUrl.EndsWith(".woff2")
                || resourceUrl.EndsWith(".woff")
                || resourceUrl.EndsWith(".png")
                || resourceUrl.EndsWith(".webp")
                || resourceUrl.EndsWith(".jpg")
                || resourceUrl.EndsWith(".jpeg")
                || resourceUrl.EndsWith(".jpe")
                || resourceUrl.EndsWith(".gif")
                || resourceUrl.EndsWith(".ico")
                || resourceUrl.EndsWith(".pdf"))
                {
                    // For approved file extensions that we don't want to do any changes on
                    var downloadUrl = rootUrl + resourceUrl;
                    var filepath = rootPath + resourceUrl.Replace("/", "\\");

                    DownloadFile(downloadUrl, filepath);
                }
                else if (resourceUrl.EndsWith(".bmp")
                    || resourceUrl.EndsWith(".mp4")
                    || resourceUrl.EndsWith(".flv")
                    || resourceUrl.EndsWith(".webm"))
                {
                    // don't download files of this extensions
                }
                else if (resourceUrl.EndsWith(".html")
                    || resourceUrl.EndsWith(".htm")
                    || resourceUrl.EndsWith("/"))
                {
                    // don't download web pages
                }
                else
                {
                    // TODO: Download and look if we recognize content-type
                }
            }
        }

        private static void EnsureCssResources(string rootUrl, string rootPath, string url)
        {
            // TODO: Download and ensure files referenced are downloaded also
            WebClient referencableClient = new WebClient();
            referencableClient.Encoding = Encoding.UTF8;
            byte[] data = referencableClient.DownloadData(rootUrl + url);
            var content = Encoding.UTF8.GetString(data);

            var matches = Regex.Matches(content, "url\\([\"|']{0,1}(?<resource>[^[\\)\"|']+)");
            foreach (Match match in matches)
            {
                var group = match.Groups["resource"];
                if (group.Success)
                {
                    var resourceUrl = group.Value;
                    var directory = url.Substring(0, url.LastIndexOf('/'));
                    var changedDir = false;
                    while (resourceUrl.StartsWith("../"))
                    {
                        changedDir = true;
                        resourceUrl = resourceUrl.Remove(0, 3);
                        directory = directory.Substring(0, directory.LastIndexOf('/'));
                    }

                    if (changedDir)
                    {
                        resourceUrl = directory.Replace(@"\", "/") + "/" + resourceUrl;
                    }

                    DownloadFile(rootUrl + resourceUrl, rootPath + resourceUrl.Replace("/", @"\"));
                }
            }
            // TODO: support download of referenced resources
            // url\(["|']{0,1}(?<resource>[^[\)"|']+)

            // TODO: check file extension and change filename
            //resourceClient.ResponseHeaders.
        }

        private static void DownloadFile(string downloadUrl, string filepath)
        {
            using (WebClient resourceClient = new WebClient())
            {
                resourceClient.Encoding = Encoding.UTF8;

                var directory = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                resourceClient.DownloadFile(downloadUrl, filepath);
            }
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