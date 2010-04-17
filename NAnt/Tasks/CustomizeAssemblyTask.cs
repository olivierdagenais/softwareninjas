using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using NAnt.Core;
using NAnt.Core.Attributes;
using SoftwareNinjas.Core;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// Updates a <c>Properties/CustomInfo.cs</c> file, if needed, for each project, based on environment variables
    /// and an XML file.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///     Applies the customization to each of the <b>Core</b> and <b>NAnt</b> projects.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <customizeAssembly projects="Core,NAnt" />
    ///     ]]>
    ///   </code>
    /// </example>
    /// 
    /// <example>
    ///   <para>
    ///     Applies the customization to each of the <b>Core</b> and <b>NAnt</b> projects, using the specified
    ///     <b>Version.xml</b> file.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <customizeAssembly projects="Core,NAnt" versionFile="Version.xml" />
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("customizeAssembly")]
    public class CustomizeAssemblyTask : AbstractProjectTask
    {
        private const string BuildNumber = "BUILD_NUMBER";
        private const string RegisteredUserDisplayName = "REGISTERED_USER_DISPLAY_NAME";
        private const string RegisteredUserEmailAddress = "REGISTERED_USER_EMAIL_ADDRESS";

        private readonly int _buildNumber;
        private readonly string _registeredUserDisplayName;
        private readonly string _registeredUserEmailAddress;

        /// <summary>
        /// Default constructor for NAnt itself.
        /// </summary>
        public CustomizeAssemblyTask()
            : base(true)
        {
            #region BUILD_NUMBER
            string buildNumberString = Environment.GetEnvironmentVariable(BuildNumber);
            if (null == buildNumberString || buildNumberString.Length == 0)
            {
                _buildNumber = -1;
            }
            else
            {
                try
                {
                    _buildNumber = Convert.ToInt32(buildNumberString, 10);
                }
                catch (FormatException)
                {
                    _buildNumber = -1;
                }
            }
            #endregion

            _registeredUserDisplayName = Environment.GetEnvironmentVariable(RegisteredUserDisplayName);
            _registeredUserEmailAddress = Environment.GetEnvironmentVariable(RegisteredUserEmailAddress);
        }

        /// <summary>
        /// Parameterized cosntructor for unit testing.
        /// </summary>
        /// 
        /// <param name="logging">
        /// Whether logging is enabled or not.
        /// </param>
        /// 
        /// <param name="buildNumber">
        /// The number to assign this particular build.
        /// </param>
        /// 
        /// <param name="registeredUserDisplayName">
        /// The name of the user.
        /// </param>
        /// 
        /// <param name="registeredUserEmailAddress">
        /// The e-mail address of the user.
        /// </param>
        public CustomizeAssemblyTask(bool logging, int buildNumber,
            string registeredUserDisplayName, string registeredUserEmailAddress)
            : base(logging)
        {
            _buildNumber = buildNumber;
            _registeredUserDisplayName = registeredUserDisplayName;
            _registeredUserEmailAddress = registeredUserEmailAddress;
        }

        /// <summary>
        /// The version XML.
        /// </summary>
        internal IXPathNavigable Version
        {
            get;
            set;
        }

        /// <summary>
        /// The location of the version XML file.  The default is <c>${basedir}/Version.xml</c>.
        /// </summary>
        /// 
        /// <example>
        ///   <para>
        ///     The version XML file must contain a <c>&lt;version&gt;</c> root element with both <b>major</b> and
        ///     <b>minor</b> attributes, such as the following:
        ///   </para>
        ///   <code>
        ///     <![CDATA[
        /// <version major="1" minor="0" />
        ///     ]]>
        ///   </code>
        /// </example>
        [TaskAttribute("versionFile", Required = false)]
        public FileInfo VersionFile
        {
            get;
            set;
        }

        /// <summary>
        /// Performs the customization.
        /// </summary>
        protected override void ExecuteTask()
        {
            if (-1 == _buildNumber)
            {
                Log(Level.Info, "No customization performed.");
                return;
            }

            #region transform
            Log(Level.Verbose, "Loading stylesheet...");
            var transform = new XslCompiledTransform();
            using (var stream = AssemblyExtensions.OpenScopedResourceStream<CustomizeAssemblyTask>("CustomInfo_cs.xsl"))
            {
                using (var reader = XmlReader.Create(stream))
                {
                    transform.Load(reader);
                }
            }
            #endregion

            #region Version
            if (null == Version)
            {
                if (null == VersionFile)
                {
                    VersionFile = new FileInfo(Path.Combine(BaseDirectory.FullName, "Version.xml"));
                }
                Log(Level.Verbose, "Loading VersionFile = {0}...", VersionFile.FullName);
                using (var stream = VersionFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var doc = new XmlDocument();
                    doc.Load(stream);
                    Version = doc;
                }
            }
            #endregion

            #region arguments
            var arguments = new XsltArgumentList();
            Action<string, object> addParam = (name, value) => 
            {
                if (value != null)
                {
                    Log(Level.Verbose, "Adding param {0} = {1}", name, value);
                    arguments.AddParam(name, String.Empty, value);
                }
            };
            addParam("buildNumber", _buildNumber);
            addParam("registeredUserDisplayName", _registeredUserDisplayName);
            addParam("registeredUserEmailAddress", _registeredUserEmailAddress);
            #endregion

            foreach (string project in EnumerateProjectNames())
            {
                Log(Level.Info, "Customizing {0}...", project);
                var projectDir = Path.Combine(BaseDirectory.FullName, project);
                var outputFile = Path.Combine(projectDir, "Properties/CustomInfo.cs");
                if (File.Exists(outputFile))
                {
                    Log(Level.Verbose, "Deleting {0}...", outputFile);
                    File.Delete(outputFile);
                }

                Log(Level.Verbose, "Generating {0}...", outputFile);
                using (var stream = File.Open(outputFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    transform.Transform(Version, arguments, stream);
                }
            }
        }
    }
}
