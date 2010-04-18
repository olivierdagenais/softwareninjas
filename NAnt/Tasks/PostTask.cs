using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.IO;

using NAnt.Core.Attributes;
using SoftwareNinjas.Core;
using NAnt.Core;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// Submits HTML forms by HTTP using the POST method.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///     POSTs a form to the server at <c>example.com</c> with three fields.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <post action="http://example.com/folder/program.ext?id=42">
    ///   <input name="firstName" value="John" />
    ///   <input name="lastName" value="Doe" />
    ///   <input name="nonSmoker" />
    /// </post>
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("post")]
    public class PostTask : TestableTask
    {
        /// <summary>
        /// Default constructor for NAnt itself.
        /// </summary>
        public PostTask() : this(true)
        {
        }

        /// <summary>
        /// Parameterized constructor for unit testing.
        /// </summary>
        ///
        /// <param name="logging">
        /// Whether logging is enabled or not.
        /// </param>
        public PostTask(bool logging) : base(logging)
        {
        }

        /// <summary>
        /// The URL the form will be submitted to.
        /// </summary>
        [TaskAttribute("action", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string Action
        {
            get;
            set;
        }

        /// <summary>
        /// The fields to submit with the form.
        /// </summary>
        [BuildElementArray("input")]
        public FormField[] Fields
        {
            get;
            set;
        }

        /// <summary>
        /// Submits the form.
        /// </summary>
        protected override void ExecuteTask()
        {
            var wc = new WebClient();
            var nvc = CreateNameValueCollection(Fields);
            Log(Level.Info, "POSTing to '{0}' with {1} form fields.", Action, nvc.Count);
            var result = wc.UploadValues(Action, "POST", nvc);
            using (var resultStream = new MemoryStream(result))
            using (var sr = new StreamReader(resultStream))
            {
                foreach (var line in sr.Lines())
                {
                    Log(Level.Verbose, line);
                }
            }
        }

        internal static NameValueCollection CreateNameValueCollection(IEnumerable<FormField> fields)
        {
            var result = new NameValueCollection();
            foreach (var field in fields)
            {
                result.Add(field.FieldName, field.Value);
            }
            return result;
        }
    }
}
