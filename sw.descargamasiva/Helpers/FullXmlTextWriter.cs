using System.IO;
using System.Text;
using System.Xml;

namespace sw.descargamasiva
{
    public class FullXmlTextWriter : XmlTextWriter
    {
        public FullXmlTextWriter(TextWriter w) : base(w)
        {
        }

        public FullXmlTextWriter(Stream w, Encoding encoding) : base(w, encoding)
        {
        }

        public FullXmlTextWriter(string filename, Encoding encoding) : base(filename, encoding)
        {
        }
        
        public override void WriteEndElement()
        {
            base.WriteFullEndElement();
        }
    }
}