using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using StaticWebEpiserverPlugin.Configuration;
using StaticWebEpiserverPlugin.Services;
using System;

namespace EpiserverStaticWeb.Business.Initialization
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class StaticWebRegressionTestsInitialization : IInitializableModule
    {
        protected IStaticWebService _staticWebService;

        public void Initialize(InitializationEngine context)
        {
            _staticWebService = ServiceLocator.Current.GetInstance<IStaticWebService>();
            var configuration = StaticWebConfiguration.CurrentSite;
            if (configuration != null && configuration.Enabled)
            {
                // NOTE: Below is for regression testing purpose only
                // For more info regarding event order and content, see https://github.com/7h3Rabbit/StaticWebEpiserverPlugin/issues/2
                _staticWebService.BeforeGeneratePage += ForRegressionTestingOnBeforeGeneratePage;
                _staticWebService.BeforeGetPageContent += ForRegressionTestingOnBeforeGetPageContent;
                _staticWebService.AfterGetPageContent += ForRegressionTestingOnAfterGetPageContent;
                _staticWebService.BeforeTryToFixLinkUrls += ForRegressionTestingOnBeforeTryToFixLinkUrls;
                _staticWebService.BeforeEnsurePageResources += ForRegressionTestingOnBeforeEnsurePageResources;
                _staticWebService.AfterEnsurePageResources += ForRegressionTestingOnAfterEnsurePageResources;
                _staticWebService.BeforeGeneratePageWrite += ForRegressionTestingOnBeforeGeneratePageWrite;
                _staticWebService.AfterGeneratePageWrite += ForRegressionTestingOnAfterGeneratePageWrite;
            }
        }

        private void ForRegressionTestingOnAfterGeneratePageWrite(object sender, StaticWebEpiserverPlugin.Events.StaticWebGeneratePageEventArgs e)
        {
            BasicSanityCheck(e);

            if (e.Items.Count != 7)
            {
                throw new ArgumentException("ForRegressionTesting - Items property should have 7 item in event", nameof(e.Items));
            }

            if (e.PageUrl == null)
            {
                throw new ArgumentException("ForRegressionTesting - PageUrl property must not be null", nameof(e.PageUrl));
            }

            if (e.Content == null)
            {
                throw new ArgumentException("ForRegressionTesting - Content property must not be null", nameof(e.Content));
            }

            if (e.Resources == null)
            {
                throw new ArgumentException("ForRegressionTesting - Resources property must not be null", nameof(e.Resources));
            }

            if (e.CurrentResources == null)
            {
                throw new ArgumentException("ForRegressionTesting - CurrentResources property must not be null", nameof(e.CurrentResources));
            }

            if (e.FilePath == null)
            {
                throw new ArgumentException("ForRegressionTesting - FilePath property must not be null", nameof(e.FilePath));
            }

            // NOTE: Below is for regression testing purpose only
            // For more info regarding event order and content, see https://github.com/7h3Rabbit/StaticWebEpiserverPlugin/issues/2
            // TODO: Validate event arguments also and not only order
            // TODO: Output and validate events
            e.Items.Add("OnAfterGeneratePageWrite", e.Items.Count == 7);     // 7 other event should have been called so 7 item in queue
        }

        private void ForRegressionTestingOnBeforeGeneratePageWrite(object sender, StaticWebEpiserverPlugin.Events.StaticWebGeneratePageEventArgs e)
        {
            BasicSanityCheck(e);

            if (e.Items.Count != 6)
            {
                throw new ArgumentException("ForRegressionTesting - Items property should have 6 item in event", nameof(e.Items));
            }

            if (e.PageUrl == null)
            {
                throw new ArgumentException("ForRegressionTesting - PageUrl property must not be null", nameof(e.PageUrl));
            }

            if (e.Content == null)
            {
                throw new ArgumentException("ForRegressionTesting - Content property must not be null", nameof(e.Content));
            }

            if (e.Resources == null)
            {
                throw new ArgumentException("ForRegressionTesting - Resources property must not be null", nameof(e.Resources));
            }

            if (e.CurrentResources == null)
            {
                throw new ArgumentException("ForRegressionTesting - CurrentResources property must not be null", nameof(e.CurrentResources));
            }

            if (e.FilePath == null)
            {
                throw new ArgumentException("ForRegressionTesting - FilePath property must not be null", nameof(e.FilePath));
            }

            // NOTE: Below is for regression testing purpose only
            // For more info regarding event order and content, see https://github.com/7h3Rabbit/StaticWebEpiserverPlugin/issues/2
            // TODO: Validate event arguments also and not only order
            e.Items.Add("OnBeforeGeneratePageWrite", e.Items.Count == 6);     // 6 other event should have been called so 6 item in queue
        }

        private void ForRegressionTestingOnAfterEnsurePageResources(object sender, StaticWebEpiserverPlugin.Events.StaticWebGeneratePageEventArgs e)
        {
            BasicSanityCheck(e);

            if (e.Items.Count != 5)
            {
                throw new ArgumentException("ForRegressionTesting - Items property should have 5 item in event", nameof(e.Items));
            }

            if (e.PageUrl == null)
            {
                throw new ArgumentException("ForRegressionTesting - PageUrl property must not be null", nameof(e.PageUrl));
            }

            if (e.Content == null)
            {
                throw new ArgumentException("ForRegressionTesting - Content property must not be null", nameof(e.Content));
            }

            if (e.Resources == null)
            {
                throw new ArgumentException("ForRegressionTesting - Resources property must not be null", nameof(e.Resources));
            }

            if (e.CurrentResources == null)
            {
                throw new ArgumentException("ForRegressionTesting - CurrentResources property must not be null", nameof(e.CurrentResources));
            }

            // NOTE: Below is for regression testing purpose only
            // For more info regarding event order and content, see https://github.com/7h3Rabbit/StaticWebEpiserverPlugin/issues/2
            // TODO: Validate event arguments also and not only order
            e.Items.Add("OnAfterEnsurePageResources", e.Items.Count == 5);     // 5 other event should have been called so 5 item in queue
        }

        private void ForRegressionTestingOnBeforeEnsurePageResources(object sender, StaticWebEpiserverPlugin.Events.StaticWebGeneratePageEventArgs e)
        {
            BasicSanityCheck(e);

            if (e.Items.Count != 4)
            {
                throw new ArgumentException("ForRegressionTesting - Items property should have 4 item in event", nameof(e.Items));
            }

            if (e.PageUrl == null)
            {
                throw new ArgumentException("ForRegressionTesting - PageUrl property must not be null", nameof(e.PageUrl));
            }

            if (e.Content == null)
            {
                throw new ArgumentException("ForRegressionTesting - Content property must not be null", nameof(e.Content));
            }

            // NOTE: Below is for regression testing purpose only
            // For more info regarding event order and content, see https://github.com/7h3Rabbit/StaticWebEpiserverPlugin/issues/2
            // TODO: Validate event arguments also and not only order
            e.Items.Add("OnBeforeEnsurePageResources", e.Items.Count == 4);     // 4 other event should have been called so 4 item in queue
        }

        private void ForRegressionTestingOnBeforeTryToFixLinkUrls(object sender, StaticWebEpiserverPlugin.Events.StaticWebGeneratePageEventArgs e)
        {
            BasicSanityCheck(e);

            if (e.Items.Count != 3)
            {
                throw new ArgumentException("ForRegressionTesting - Items property should have 3 item in event", nameof(e.Items));
            }

            if (e.PageUrl == null)
            {
                throw new ArgumentException("ForRegressionTesting - PageUrl property must not be null", nameof(e.PageUrl));
            }

            if (e.Content == null)
            {
                throw new ArgumentException("ForRegressionTesting - Content property must not be null", nameof(e.Content));
            }

            // NOTE: Below is for regression testing purpose only
            // For more info regarding event order and content, see https://github.com/7h3Rabbit/StaticWebEpiserverPlugin/issues/2
            // TODO: Validate event arguments also and not only order
            e.Items.Add("OnBeforeTryToFixLinkUrls", e.Items.Count == 3);     // 3 other event should have been called so 3 item in queue
        }

        private void ForRegressionTestingOnAfterGetPageContent(object sender, StaticWebEpiserverPlugin.Events.StaticWebGeneratePageEventArgs e)
        {
            BasicSanityCheck(e);

            if (e.Items.Count != 2)
            {
                throw new ArgumentException("ForRegressionTesting - Items property should have 2 item in event", nameof(e.Items));
            }

            if (e.PageUrl == null)
            {
                throw new ArgumentException("ForRegressionTesting - PageUrl property must not be null", nameof(e.PageUrl));
            }

            if (e.Content == null)
            {
                throw new ArgumentException("ForRegressionTesting - Content property must not be null", nameof(e.Content));
            }

            // NOTE: Below is for regression testing purpose only
            // For more info regarding event order and content, see https://github.com/7h3Rabbit/StaticWebEpiserverPlugin/issues/2
            // TODO: Validate event arguments also and not only order
            e.Items.Add("OnAfterGetPageContent", e.Items.Count == 2);     // 2 other event should have been called so 2 item in queue
        }

        private void ForRegressionTestingOnBeforeGetPageContent(object sender, StaticWebEpiserverPlugin.Events.StaticWebGeneratePageEventArgs e)
        {
            BasicSanityCheck(e);

            if (e.Items.Count != 1)
            {
                throw new ArgumentException("ForRegressionTesting - Items property should have 1 item in event", nameof(e.Items));
            }

            if (e.PageUrl == null)
            {
                throw new ArgumentException("ForRegressionTesting - PageUrl property must not be null", nameof(e.PageUrl));
            }

            // NOTE: Below is for regression testing purpose only
            // For more info regarding event order and content, see https://github.com/7h3Rabbit/StaticWebEpiserverPlugin/issues/2
            // TODO: Validate event arguments also and not only order
            e.Items.Add("OnBeforeGetPageContent", e.Items.Count == 1);     // ONE other event should have been called so 1 item in queue
        }

        private void ForRegressionTestingOnBeforeGeneratePage(object sender, StaticWebEpiserverPlugin.Events.StaticWebGeneratePageEventArgs e)
        {
            BasicSanityCheck(e);

            if (e.Items.Count != 0)
            {
                throw new ArgumentException("ForRegressionTesting - Items property should have 0 item in event", nameof(e.Items));
            }

            // NOTE: Below is for regression testing purpose only
            // For more info regarding event order and content, see https://github.com/7h3Rabbit/StaticWebEpiserverPlugin/issues/2
            // TODO: Validate event arguments also and not only order
            e.Items.Add("OnBeforeGeneratePage", e.Items.Count == 0);     // No other event should have been called so no other items in queue
        }

        private static void BasicSanityCheck(StaticWebEpiserverPlugin.Events.StaticWebGeneratePageEventArgs e)
        {
            if (e.ContentLink == null)
            {
                throw new ArgumentException("ForRegressionTesting - ContentLink property must not be null", nameof(e.ContentLink));
            }

            if (e.CultureInfo == null)
            {
                throw new ArgumentException("ForRegressionTesting - CultureInfo property must not be null", nameof(e.CultureInfo));
            }

            if (e.Items == null)
            {
                throw new ArgumentException("ForRegressionTesting - Items property must not be null", nameof(e.Items));
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            _staticWebService = null;
        }
    }
}