using Microsoft.AspNetCore.Html;

namespace TransformalizeModule.Services.Contracts {
    public interface ILinkService {
        HtmlString Create(string contentItemId, string actionUrl, bool everything);
    }
}
