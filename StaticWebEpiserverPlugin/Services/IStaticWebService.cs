using EPiServer.Core;
using System.Globalization;

namespace StaticWebEpiserverPlugin.Services
{
    public interface IStaticWebService
    {
        void GeneratePage(ContentReference contentLink, CultureInfo language);
        void GeneratePagesDependingOnBlock(ContentReference contentLink);
    }
}