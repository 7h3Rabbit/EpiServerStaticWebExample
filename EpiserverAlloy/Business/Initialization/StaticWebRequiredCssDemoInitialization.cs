using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using StaticWebEpiserverPlugin.Configuration;
using StaticWebEpiserverPlugin.RequiredCssOnly.Services;
using StaticWebEpiserverPlugin.Services;
using System.Linq;

namespace EpiserverStaticWeb.Business.Initialization
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class StaticWebRequiredCssDemoInitialization : IInitializableModule
    {
        protected IStaticWebService _staticWebService;

        public void Initialize(InitializationEngine context)
        {
            _staticWebService = ServiceLocator.Current.GetInstance<IStaticWebService>();
            var configuration = StaticWebConfiguration.Current;
            if (configuration != null && configuration.Enabled)
            {
                _staticWebService.AfterEnsurePageResources += OnAfterEnsurePageResources;
            }
        }

        private void OnAfterEnsurePageResources(object sender, StaticWebEpiserverPlugin.Events.StaticWebGeneratePageEventArgs e)
        {
            if (e.Content == null)
                return;

            var configuration = StaticWebConfiguration.Current;
            if (configuration == null || !configuration.Enabled)
            {
                return;
            }

            var html = e.Content;

            var requiredCssService = ServiceLocator.Current.GetInstance<RequiredCssOnlyService>();

            var cssResources = e.CurrentResources.Where(resource => resource.Value != null && resource.Value.EndsWith(".css")).Select(pair => pair.Value);
            foreach (var resource in cssResources)
            {
                var filePath = configuration.OutputPath + resource.Replace("/", "\\");
                if (!System.IO.File.Exists(filePath))
                {
                    continue;
                }

                var cssContent = System.IO.File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                var newCssContent = requiredCssService.RemoveUnusedRules(cssContent, html);

                html = html.Replace(@"<link href=""" + resource + @""" rel=""stylesheet""/>", $"<style>{newCssContent}</style>");
            }

            e.Content = html;
        }

        public void Uninitialize(InitializationEngine context)
        {
            _staticWebService.AfterEnsurePageResources -= OnAfterEnsurePageResources;
            _staticWebService = null;
        }
    }
}