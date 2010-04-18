using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;
using NAnt.Core.Types;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// Evaluates an XPath expression against an XML file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike the <see cref="XmlPeekTask"/>, the <see cref="XpathTask"/> evaluates any XPath expression.
    /// </para>
    /// </remarks>
    /// <example>
    ///   <para>
    ///   The example provided assumes that the following XML file (coverage.xml)
    ///   exists in the current build directory.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <coverage profilerVersion="1.5.8 Beta" driverVersion="1.5.8.0" startTime="2010-04-17T16:16:31.2782764-04:00">
    ///   <module moduleId="21" name="StringExtensions.dll" assembly="StringExtensions">
    ///     <method name="Capitalize" excluded="false" instrumented="true" class="StringExtensions.StringExtensions">
    ///       <seqpnt visitcount="12" line="10" column="13" endline="10" endcolumn="42" excluded="false" />
    ///       <seqpnt visitcount="12" line="11" column="13" endline="11" endcolumn="31" excluded="false" />
    ///       <seqpnt visitcount="12" line="12" column="31" endline="12" endcolumn="36" excluded="false" />
    ///       <seqpnt visitcount="23" line="12" column="22" endline="12" endcolumn="27" excluded="false" />
    ///       <seqpnt visitcount="23" line="14" column="17" endline="14" endcolumn="38" excluded="false" />
    ///       <seqpnt visitcount="11" line="16" column="21" endline="16" endcolumn="31" excluded="false" />
    ///       <seqpnt visitcount="0" line="18" column="25" endline="18" endcolumn="52" excluded="false" />
    ///       <seqpnt visitcount="7" line="19" column="25" endline="19" endcolumn="37" excluded="false" />
    ///       <seqpnt visitcount="0" line="22" column="25" endline="22" endcolumn="38" excluded="false" />
    ///       <seqpnt visitcount="12" line="26" column="21" endline="26" endcolumn="47" excluded="false" />
    ///       <seqpnt visitcount="6" line="27" column="25" endline="27" endcolumn="40" excluded="false" />
    ///       <seqpnt visitcount="12" line="28" column="21" endline="28" endcolumn="34" excluded="false" />
    ///       <seqpnt visitcount="0" line="12" column="28" endline="12" endcolumn="30" excluded="false" />
    ///       <seqpnt visitcount="11" line="31" column="13" endline="31" endcolumn="34" excluded="false" />
    ///       <seqpnt visitcount="11" line="32" column="9" endline="32" endcolumn="10" excluded="false" />
    ///     </method>
    ///   </module>
    /// </coverage>
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>
    ///   The example will count the <c>seqpnt</c> elements that have been visited at least once.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <xpath
    ///     file="coverage.xml"
    ///     query="count(//seqpnt[@visitcount > 0])"
    ///     property="visited.sequencePoints">
    /// </xpath>
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("xpath")]
    public class XpathTask : TestableTask
    {
        /// <summary>
        /// Default constructor for NAnt itself.
        /// </summary>
        public XpathTask(): this(true)
        {
        }

        /// <summary>
        /// Parameterized constructor for unit testing.
        /// </summary>
        ///
        /// <param name="logging">
        /// Whether logging is enabled or not.
        /// </param>
        public XpathTask(bool logging)
            : base(logging)
        {
        }

        /// <summary>
        /// The path to the XML file.
        /// </summary>
        [TaskAttribute("file", Required = true)]
        public FileInfo XmlFile
        {
            get;
            set;
        }

        /// <summary>
        /// The property that receives the string representation of the result of evaluating the <see cref="XPath"/>
        /// expression.
        /// </summary>
        [TaskAttribute("property", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string Property
        {
            get;
            set;
        }

        /// <summary>
        /// The XPath expression that will be evaluated against <see cref="XmlFile"/>.
        /// </summary>
        [TaskAttribute("query", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string XPath
        {
            get;
            set;
        }

        /// <summary>
        /// Namespace definitions to resolve prefixes in the XPath expression.
        /// </summary>
        [BuildElementCollection("namespaces", "namespace")]
        public XmlNamespaceCollection Namespaces
        {
            get;
            set;
        }

        /// <summary>
        /// Evaluates the <see cref="XPath"/> against the <see cref="XmlFile"/> and assigns the string representation
        /// of the result to the property identified by <see cref="Property"/>.
        /// </summary>
        protected override void ExecuteTask() {
            Log(Level.Info, "Peeking at '{0}' with XPath expression '{1}'.", XmlFile.FullName,  XPath);

            var document = new XmlDocument();
            document.Load(XmlFile.FullName);

            var resolver = new XmlNamespaceManager(document.NameTable);
            foreach (var xmlNamespace in Namespaces)
            {
                if (xmlNamespace.IfDefined && !xmlNamespace.UnlessDefined)
                {
                    resolver.AddNamespace(xmlNamespace.Prefix, xmlNamespace.Uri);
                }
            }

            var value = Evaluate(document, resolver, XPath);
            Properties[Property] = value;
        }

        internal static string Evaluate(IXPathNavigable document, IXmlNamespaceResolver resolver, string query)
        {
            var result = String.Empty;

            var navigator = document.CreateNavigator();
            object contents = null;
            if (navigator != null)
            {
                contents = navigator.Evaluate(query, resolver);
            }
            if (contents != null)
            {
                if (contents is XPathNodeIterator)
                {
                    var it = (XPathNodeIterator) contents;
                    if (it.MoveNext())
                    {
                        result = it.Current.Value;
                    }
                }
                else
                {
                    result = contents.ToString();
                }
            }

            return result;
        }
    }
}
