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

                //var page = e.Content; // as PageData;
                ////var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
                ////var page2 = urlResolver.Route(new EPiServer.UrlBuilder(httpContext.Request.Url));

                //// Resolve the right Template based on EpiServers own templating engine
                //var templateResolver = ServiceLocator.Current.GetInstance<TemplateResolver>();
                //var model = templateResolver.ResolvePageTemplate(httpContextBase, page);

                //// Resolve the controller
                //var pageController = ServiceLocator.Current.GetInstance(model.TemplateType) as ControllerBase;
                //string controllerName = pageController.GetType().Name.Replace("Controller", string.Empty);

                //// Create routing data for controller
                //RouteData routeData = new RouteData();
                //routeData.Values.Add("currentPage", page);
                //routeData.Values.Add(RoutingConstants.CurrentContentKey, model);
                //routeData.Values.Add(RoutingConstants.ControllerTypeKey, model.TemplateType);
                //// TODO: Modify this to use available language(s)
                //routeData.Values.Add(RoutingConstants.LanguageKey, ContentLanguage.PreferredCulture.Name);
                //routeData.Values.Add(RoutingConstants.ControllerKey, controllerName);
                //routeData.Values.Add(RoutingConstants.NodeKey, page.ContentLink);
                //routeData.Values.Add("contentLink", page.ContentLink);
                //routeData.Values.Add(RoutingConstants.PartialKey, page.ContentLink);
                //routeData.Values.Add(RoutingConstants.ActionKey, "index");


                //var controllerFactory = ServiceLocator.Current.GetInstance<IControllerFactory>();
                //var controller = (ControllerBase)controllerFactory.CreateController(httpContextBase.Request.RequestContext, controllerName);

                ////var layoutController = controller as IModifyLayout;
                ////if (layoutController != null)
                ////{
                ////    PageViewContextFactory _contextFactory = new PageViewContextFactory()
                ////    model.Layout = _contextFactory.CreateLayoutModel(page.ContentLink, viewContext.RequestContext);

                ////    layoutController.ModifyLayout(layoutModel);
                ////}



                //var controllerContext = new ControllerContext(httpContextBase, routeData, controller);
                //var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());

                //var actionDescriptor = controllerDescriptor.FindAction(controllerContext, "index");
                ////var actionViewResult = actionDescriptor.Execute(controllerContext, new Dictionary<string, object>() { { "currentPage", page } }) as ViewResult;
                //var actionViewResult = actionDescriptor.Execute(controllerContext, new Dictionary<string, object>() {
                //    { "currentPage", page },
                //    { "contentLink", page.ContentLink },
                //    { RoutingConstants.CurrentContentKey, model },
                //    { RoutingConstants.ControllerTypeKey, model.TemplateType },
                //    { RoutingConstants.LanguageKey, ContentLanguage.PreferredCulture.Name },
                //    { RoutingConstants.ControllerKey, controllerName },
                //    { RoutingConstants.NodeKey, page.ContentLink },
                //    { RoutingConstants.PartialKey, page.ContentLink },
                //    { RoutingConstants.ActionKey, "index" },
                //    { RoutingConstants.RoutedDataKey, page.ContentLink }
                //}) as ViewResult;
                //var viewResult = actionViewResult.ViewEngineCollection.FindView(controllerContext, actionViewResult.ViewName, actionViewResult.MasterName);


                //using (var stringWriter = new StringWriter())
                //{
                //    var viewContext = new ViewContext(controllerContext, viewResult.View, actionViewResult.ViewData, actionViewResult.TempData, stringWriter);
                //    viewContext.RouteData = routeData;
                //    viewContext.RequestContext.RouteData = routeData;

                //    viewContext.RequestContext.RouteData.DataTokens.Add("currentPage", page);
                //    viewContext.RequestContext.RouteData.DataTokens.Add("contentLink", page.ContentLink);

                //    viewContext.RequestContext.RouteData.DataTokens.Add(RoutingConstants.CurrentContentKey, model);
                //    viewContext.RequestContext.RouteData.DataTokens.Add(RoutingConstants.ControllerTypeKey, model.TemplateType);
                //    viewContext.RequestContext.RouteData.DataTokens.Add(RoutingConstants.LanguageKey, ContentLanguage.PreferredCulture.Name);
                //    viewContext.RequestContext.RouteData.DataTokens.Add(RoutingConstants.ControllerKey, controllerName);
                //    viewContext.RequestContext.RouteData.DataTokens.Add(RoutingConstants.NodeKey, page.ContentLink);
                //    viewContext.RequestContext.RouteData.DataTokens.Add(RoutingConstants.PartialKey, page.ContentLink);
                //    viewContext.RequestContext.RouteData.DataTokens.Add(RoutingConstants.ActionKey, "index");
                //    viewContext.RequestContext.RouteData.DataTokens.Add(RoutingConstants.RoutedDataKey, page.ContentLink);

                //    viewResult.View.Render(viewContext, stringWriter);
                //    var html = stringWriter.ToString();
                //    // TODO: Where to write content?
                //    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\index.html", html);
                //}
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

                //foreach (var reference in references)
                //{
                //    IContent page;
                //    //var contentLink = new ContentReference(reference.ReferencedID.ID, false);
                //    if (repository.TryGet<IContent>(reference.OwnerID, out page))
                //    {
                //        GeneratePage(page.ContentLink);
                //    }
                //}
            }
        }

        private static void GeneratePage(ContentReference contentLink)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            var orginalUrl = urlResolver.GetUrl(contentLink);
            if (orginalUrl == null)
                return;

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

            // TODO: make sure we have all resources for current page also
            // <(script|link|img|source).*(href|src|srcset)="(?<resource>[^"]+)"

            // TODO: make sure we have all meta resources for current page
            // Below matches ALL meta content that is a URL
            // <(meta).*(content)="(?<resource>(http:\/\/|https:\/\/|\/)[^"]+)"
            // Below matches ONLY known properties
            // <(meta).*(property|name)="(twitter:image|og:image)".*(content)="(?<resource>[http:\/\/|https:\/\/|\/][^"]+)"

            var rootPath = @"C:\inetpub\wwwroot";

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