using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.ContentExtentions.AppServices.Dtos
{
    public class ContentModel : Dictionary<string, object>
    {


        /// <summary>
        /// The logical identifier of the content item across versions.
        /// </summary>
        public string ContentItemId

        {
            get
            {
                if (this.ContainsKey("contentItemId"))
                {
                    return this["contentItemId"]?.ToString();
                }
                return null;

            }
        }

        /// <summary>
        /// The logical identifier of the versioned content item.
        /// </summary>
        public string ContentItemVersionId
        {
            get
            {
                if (this.ContainsKey("contentItemVersionId"))
                {
                    return this["contentItemVersionId"]?.ToString();
                }
                return null;
            }
        }

        /// <summary>
        /// The content type of the content item.
        /// </summary>
        public string ContentType
        {
            get
            {
                if (this.ContainsKey("contentType"))
                {
                    return this["contentType"]?.ToString();
                }
                return null;

            }
        }

        /// <summary>
        /// Whether the version is published or not.
        /// </summary>
        //public bool? Published
        //{
        //    get
        //    {
        //        if (this.ContainsKey("published"))
        //        {
        //            return (bool)this["published"];
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        /// <summary>
        /// Whether the version is the latest version of the content item.
        /// </summary>
        //public bool? Latest
        //{
        //    get
        //    {
        //        if (this.ContainsKey("latest"))
        //        {
        //            return (bool)this["latest"];
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        /// <summary>
        /// The text representing this content item.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (this.ContainsKey("displayText"))
                {
                    return this["displayText"]?.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(DisplayText) ? $"{ContentType} ({ContentItemId})" : DisplayText;
        }
    }
}
