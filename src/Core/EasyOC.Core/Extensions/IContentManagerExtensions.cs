using EasyOC.Core.Models;
using OrchardCore.ContentManagement;
using System;
using System.Threading.Tasks;

namespace EasyOC.Core.Extensions
{
    public static class IContentManagerExtensions
    {
        // What would be even nicer is to infer the content type from the discriminator.
        public static async Task<TDto> NewDtoAsync<TDto>(this IContentManager contentManager, string contentType)
            where TDto : ContentItemDto
        {
            return (await contentManager.NewAsync(contentType))
                .ToDto<TDto>();
        }

        public static async Task<TDto> NewDtoAsync<TDto>(this IContentManager contentManager, string contentType, Action<TDto> action)
            where TDto : ContentItemDto
        {
            var dto = (await contentManager.NewAsync(contentType))
                .ToDto<TDto>();

            action?.Invoke(dto);

            return dto;
        }
    }
}



