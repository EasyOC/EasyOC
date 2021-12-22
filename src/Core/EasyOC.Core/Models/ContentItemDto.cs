using System;

namespace EasyOC.Core.Models
{
    // This does not implement 'IContent`.
    // It keeps no original track of the content item.
    // It could. We could json ignore it
    // Which might be useful if this is being passed between services, and needs to be converted back to a content item.
    // Worht thinking about.
    //[JsonConverter(typeof(JsonInheritanceConverter), "contentType")]
    public class ContentItemDto : ContentElementDto
    {
        /// <summary>
        /// The primary key in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The logical identifier of the content item across versions.
        /// </summary>
        public string ContentItemId { get; set; }

        /// <summary>
        /// The logical identifier of the versioned content item.
        /// </summary>
        public string ContentItemVersionId { get; set; }

        /// <summary>
        /// The content type of the content item.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Whether the version is published or not.
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Whether the version is the latest version of the content item.
        /// </summary>
        public bool Latest { get; set; }

        /// <summary>
        /// When the content item version has been updated.
        /// </summary>
        public DateTime? ModifiedUtc { get; set; }

        /// <summary>
        /// When the content item has been published.
        /// </summary>
        public DateTime? PublishedUtc { get; set; }

        /// <summary>
        /// When the content item has been created or first published.
        /// </summary>
        public DateTime? CreatedUtc { get; set; }

        /// <summary>
        /// The name of the user who first created this content item version
        /// and owns content rights.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// The name of the user who last modified this content item version.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The text representing this content item.
        /// </summary>
        public string DisplayText { get; set; }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(DisplayText) ? $"{ContentType} ({ContentItemId})" : DisplayText;
        }
    }
}



