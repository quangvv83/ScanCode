using System;
using System.Xml.Linq;

namespace ScanCode.Extensions
{
    public static class XElementExtension
    {
        public static XAttribute GetAttribute(this XElement xElement, string name)
        {
            if (xElement == null)
            {
                return null;
            }

            XElement el = xElement.HasAttributes ? xElement : (XElement)xElement.FirstNode;
            if (el == null)
            {
                return null;
            }

            XAttribute attr = el.FirstAttribute;
            while (attr != null)
            {
                if (attr.Name.LocalName.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return attr;
                }
                attr = attr.NextAttribute;
            }
            return null;
        }

        public static void ForeachAttribute(this XElement el, Action<XAttribute> action)
        {
            if (el.HasAttributes)
            {
                var attr = el.FirstAttribute;
                while (attr != null)
                {
                    action(attr);
                    attr = attr.NextAttribute;
                }
            }
        }

    }
}
