using System.IO;
using System.Text;
using static Gwen.Net.Platform.GwenPlatform;

namespace Gwen.Net.Xml
{
    /// <summary>
    ///     Implement this in a class that can be used as a source for XML parser.
    /// </summary>
    public interface IXmlSource
    {
        Stream GetStream();
    }

    /// <summary>
    ///     XML source as a string.
    /// </summary>
    public class XmlStringSource : IXmlSource
    {
        private readonly Encoding m_encoding;

        private readonly string m_xml;

        public XmlStringSource(string xml, Encoding encoding = null)
        {
            m_xml = xml;

            if (encoding == null)
            {
                m_encoding = new UTF8Encoding();
            }
            else
            {
                m_encoding = encoding;
            }
        }

        public Stream GetStream()
        {
            Stream stream = new MemoryStream();
            StreamWriter writer = new(stream, m_encoding);
            writer.Write(m_xml);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }

    /// <summary>
    ///     XML source as a file.
    /// </summary>
    public class XmlFileSource : IXmlSource
    {
        private readonly string m_fileName;

        public XmlFileSource(string fileName)
        {
            m_fileName = fileName;
        }

        public Stream GetStream()
        {
            return GetFileStream(m_fileName, isWritable: false);
        }
    }
}