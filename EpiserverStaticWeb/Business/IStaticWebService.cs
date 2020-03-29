using EPiServer.Core;

namespace EpiserverStaticWeb.Business
{
    public interface IStaticWebService
    {
        void GeneratePage(ContentReference contentLink);
        void GeneratePagesDependingOnBlock(ContentReference contentLink);
    }
}