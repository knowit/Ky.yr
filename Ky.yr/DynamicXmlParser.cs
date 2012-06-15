using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Xml.Linq;
using System.IO;

namespace Ky.yr
{

    /// <summary>
    /// Takes a XML string as input param and returns a dynamic object that you can just type out the stuff you want.
    /// Like you do in umbraco Razor code
    /// </summary>
    public class DynamicXml : DynamicObject, IEnumerable
    {
        private readonly List<XElement> _elements;

        public DynamicXml(string text)
        {
            var doc = XDocument.Parse(text);
            _elements = new List<XElement> { doc.Root };
        }

        protected DynamicXml(XElement element)
        {
            _elements = new List<XElement> { element };
        }

        protected DynamicXml(IEnumerable<XElement> elements)
        {
            _elements = new List<XElement>(elements);
        }

        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            result = null;
            if (binder.Name == "Value")
                result = _elements[0].Value;
            else if (binder.Name == "Count")
                result = _elements.Count;
            else
            {
                var attr = _elements[0].Attribute(
                    XName.Get(binder.Name));
                if (attr != null)
                    result = attr;
                else
                {
                    var items = _elements.Descendants(
                        XName.Get(binder.Name));
                    if (items == null || items.Count() == 0)
                        return false;
                    result = new DynamicXml(items);
                }
            }
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder,
            object[] indexes, out object result)
        {
            int ndx = (int)indexes[0];
            result = new DynamicXml(_elements[ndx]);
            return true;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var element in _elements)
                yield return new DynamicXml(element);
        }
    }
    
}