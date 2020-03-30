using EPiServer.Core;
using System.Globalization;

namespace EpiserverStaticWeb.Business
{
    public interface IStaticWebService
    {
        void GeneratePage(ContentReference contentLink, CultureInfo language);
        void GeneratePagesDependingOnBlock(ContentReference contentLink);
    }
}