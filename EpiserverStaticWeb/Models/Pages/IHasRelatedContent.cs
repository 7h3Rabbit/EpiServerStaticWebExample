using EPiServer.Core;

namespace EpiserverStaticWeb.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
