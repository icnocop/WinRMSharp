using System.Xml.Linq;
using System.Xml.Serialization;
using WinRMSharp.Contracts;

namespace WinRMSharp.Utils
{
    internal static class Xml
    {
        private static readonly Dictionary<string, string> NamespaceToPrefix = new()
        {
            { Namespace.XML_SCHEMA, "xsd" },
            { Namespace.XML_SCHEMA_INSTANCE, "xsi" },
            { Namespace.SOAP_ENVELOPE, "env" },
            { Namespace.ADDRESSING, "a" },
            { Namespace.CIM_BINDING, "b" },
            { Namespace.ENUMERATION, "n" },
            { Namespace.TRANSFER, "x" },
            { Namespace.WSMID, "wsmid" },
            { Namespace.WSMAN_0, "w" },
            { Namespace.WSMAN_1, "p" },
            { Namespace.WSMAN_SHELL, "rsp" },
            { Namespace.WSMAN_CONFIG, "cfg"}
        };

        public static string Serialize<T>(T obj)
        {
            // First pass: Generate temporary XML to detect used namespaces
            string tempXml = SerializeTemp(obj);

            // Extract namespaces actually used in the XML
            HashSet<string> usedNamespaces = ExtractNamespaces(tempXml);

            // Build XmlSerializerNamespaces with desired prefixes
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            foreach (string nsUri in usedNamespaces)
            {
                string? prefix = NamespaceToPrefix[nsUri];
                namespaces.Add(prefix, nsUri);
            }

            // Second pass: Serialize with filtered namespaces
            return SerializeFinal(obj, namespaces);
        }

        private static string SerializeTemp<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringWriter writer = new StringWriter();
            serializer.Serialize(writer, obj);
            return writer.ToString();
        }

        private static string SerializeFinal<T>(T obj, XmlSerializerNamespaces namespaces)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringWriter writer = new StringWriter();
            serializer.Serialize(writer, obj, namespaces);
            return writer.ToString();
        }

        private static HashSet<string> ExtractNamespaces(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            HashSet<string> namespaces = new HashSet<string>();

            foreach (XElement? element in doc.Descendants())
            {
                foreach (XAttribute? attr in element.Attributes().Where(a => a.IsNamespaceDeclaration))
                {
                    namespaces.Add(attr.Value.Trim());
                }
            }

            return namespaces;
        }

        public static XDocument Parse(this string text)
        {
            return XDocument.Parse(text);
        }

        public static string? Format(this string text)
        {
            try
            {
                XDocument doc = XDocument.Parse(text);
                return doc.ToString();
            }
            catch (Exception)
            {
                return text;
            }
        }
    }
}
