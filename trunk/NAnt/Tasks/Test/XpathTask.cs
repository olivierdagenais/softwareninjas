using System;
using System.Xml;
using System.Xml.XPath;

using NUnit.Framework;
using Parent = SoftwareNinjas.NAnt.Tasks;

namespace SoftwareNinjas.NAnt.Tasks.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.XpathTask"/>.
    /// </summary>
    [TestFixture]
    public class XpathTask
    {
        private readonly IXPathNavigable _testDocument;
        const string CoverageXml =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<coverage profilerVersion=""1.5.8 Beta"" driverVersion=""1.5.8.0"" startTime=""2010-04-17T16:16:31.2782764-04:00"">
  <module moduleId=""21"" name=""StringExtensions.dll"" assembly=""StringExtensions"">
    <method name=""Capitalize"" excluded=""false"" instrumented=""true"" class=""StringExtensions.StringExtensions"">
      <seqpnt visitcount=""12"" line=""10"" column=""13"" endline=""10"" endcolumn=""42"" excluded=""false""/>
      <seqpnt visitcount=""12"" line=""11"" column=""13"" endline=""11"" endcolumn=""31"" excluded=""false""/>
      <seqpnt visitcount=""12"" line=""12"" column=""31"" endline=""12"" endcolumn=""36"" excluded=""false""/>
      <seqpnt visitcount=""23"" line=""12"" column=""22"" endline=""12"" endcolumn=""27"" excluded=""false""/>
      <seqpnt visitcount=""23"" line=""14"" column=""17"" endline=""14"" endcolumn=""38"" excluded=""false""/>
      <seqpnt visitcount=""11"" line=""16"" column=""21"" endline=""16"" endcolumn=""31"" excluded=""false""/>
      <seqpnt visitcount=""0"" line=""18"" column=""25"" endline=""18"" endcolumn=""52"" excluded=""false""/>
      <seqpnt visitcount=""7"" line=""19"" column=""25"" endline=""19"" endcolumn=""37"" excluded=""false""/>
      <seqpnt visitcount=""0"" line=""22"" column=""25"" endline=""22"" endcolumn=""38"" excluded=""false""/>
      <seqpnt visitcount=""12"" line=""26"" column=""21"" endline=""26"" endcolumn=""47"" excluded=""false""/>
      <seqpnt visitcount=""6"" line=""27"" column=""25"" endline=""27"" endcolumn=""40"" excluded=""false""/>
      <seqpnt visitcount=""12"" line=""28"" column=""21"" endline=""28"" endcolumn=""34"" excluded=""false""/>
      <seqpnt visitcount=""0"" line=""12"" column=""28"" endline=""12"" endcolumn=""30"" excluded=""false""/>
      <seqpnt visitcount=""11"" line=""31"" column=""13"" endline=""31"" endcolumn=""34"" excluded=""false""/>
      <seqpnt visitcount=""11"" line=""32"" column=""9"" endline=""32"" endcolumn=""10"" excluded=""false""/>
    </method>
  </module>
  <element>text node</element>
</coverage>";

        /// <summary>
        /// Loads the XML that will be used for tests.
        /// </summary>
        public XpathTask()
        {
            var doc = new XmlDocument();
            doc.LoadXml(CoverageXml);
            _testDocument = doc;
        }

        /// <summary>
        /// Tests the <see cref="Parent.XpathTask.Evaluate(IXPathNavigable, IXmlNamespaceResolver, string)" /> method
        /// with the query given as the example in the documentation.
        /// </summary>
        [Test]
        public void Evaluate_SampleQuery()
        {
            Assert.AreEqual("12", Parent.XpathTask.Evaluate(_testDocument, null, "count(//seqpnt[@visitcount > 0])"));
        }

        /// <summary>
        /// Tests the <see cref="Parent.XpathTask.Evaluate(IXPathNavigable, IXmlNamespaceResolver, string)" /> method
        /// with a query that obtains the value of an attribute.
        /// </summary>
        [Test]
        public void Evaluate_AttributeValue()
        {
            Assert.AreEqual("Capitalize", Parent.XpathTask.Evaluate(_testDocument, null, "//method/@name"));
        }

        /// <summary>
        /// Tests the <see cref="Parent.XpathTask.Evaluate(IXPathNavigable, IXmlNamespaceResolver, string)" /> method
        /// with a query that obtains the text content of a node under an element.
        /// </summary>
        [Test]
        public void Evaluate_ElementValue()
        {
            Assert.AreEqual("text node", Parent.XpathTask.Evaluate(_testDocument, null, "/coverage/element/text()"));
        }

        /// <summary>
        /// Tests the <see cref="Parent.XpathTask.Evaluate(IXPathNavigable, IXmlNamespaceResolver, string)" /> method
        /// with a query that obtains a nodeset, which isn't supported.
        /// </summary>
        [Test]
        public void Evaluate_NodeSet()
        {
            Assert.AreEqual(String.Empty, Parent.XpathTask.Evaluate(_testDocument, null, "//seqpnt"));
        }
    }
}
