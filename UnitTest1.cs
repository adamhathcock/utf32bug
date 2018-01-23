using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Xunit;

namespace utf32bug
{
    public class UnitTest1
    {
        [Fact]
        public void Should_Transform_Xml_String_And_Xsl_String_To_Result_String_With_Utf32Xml_Declaration()
        {
            // Given
            var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<breakfast_menu>\n    <food>\n        <name>Belgian Waffles</name>\n        <price>$5.95</price>\n        <description>Two of our famous Belgian Waffles with plenty of real maple syrup</description>\n        <calories>650</calories>\n    </food>\n    <food>\n        <name>Strawberry Belgian Waffles</name>\n        <price>$7.95</price>\n        <description>Light Belgian waffles covered with strawberries and whipped cream</description>\n        <calories>900</calories>\n    </food>\n</breakfast_menu>";
            var xsl = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<html xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xsl:version=\"1.0\">\n    <body style=\"font-family:Arial;font-size:12pt;background-color:#EEEEEE\">\n        <xsl:for-each select=\"breakfast_menu/food\">\n            <div style=\"background-color:teal;color:white;padding:4px\">\n                <span style=\"font-weight:bold\">\n                    <xsl:value-of select=\"name\" />\n                    -\n                </span>\n                <xsl:value-of select=\"price\" />\n            </div>\n            <div style=\"margin-left:20px;margin-bottom:1em;font-size:10pt\">\n                <p>\n                    <xsl:value-of select=\"description\" />\n                    <span style=\"font-style:italic\">\n                        (\n                        <xsl:value-of select=\"calories\" />\n                        calories per serving)\n                    </span>\n                </p>\n            </div>\n        </xsl:for-each>\n    </body>\n</html>";
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF32Encoding(false, false, true)
            };

            string resultString;
            using (TextReader
                           xslReader = new StringReader(xsl),
                           xmlReader = new StringReader(xml))
            {
                using (var result = new MemoryStream())
                {

                    var xslXmlReader = XmlReader.Create(xslReader);
                    var xmlXmlReader = XmlReader.Create(xmlReader);
                    var resultXmlTextWriter = XmlWriter.Create(result, settings);


                    var xslTransform = new XslCompiledTransform();
                    xslTransform.Load(xslXmlReader);
                    xslTransform.Transform(xmlXmlReader, resultXmlTextWriter);
                    result.Position = 0;
                    resultString = settings.Encoding.GetString(result.ToArray());
                }
            }
            // When
            var result2 = string.Concat(resultString.Take(39));

            // Then
            Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-32\"?>", result2);
        }
    }
}
