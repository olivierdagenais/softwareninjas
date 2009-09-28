using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SoftwareNinjas.Core;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.TextTemplating;
using Mono.TextTemplating;

namespace SoftwareNinjas.MSBuild
{
	/// <summary>
	/// Compiles <b>T4 Templates</b> into C# classes.
	/// </summary>
	/// 
	/// <example>
	///   <para>
	///     To work around a
	///     <see href="http://stackoverflow.com/questions/541546/pre-build-msbuild-task-to-update-assemblyinfo-not-in-sync-with-built-exe">
	///			bug in Visual Studio
	///		</see>, first add the following line in the first <code>&lt;PropertyGroup&gt;</code> element of your
	///		<b>.csproj</b> file:
	///   </para>
	///   <code>
	///     <![CDATA[
	/// <UseHostCompilerIfAvailable>FALSE</UseHostCompilerIfAvailable>
	/// ]]>
	///   </code>
	///   
	///   <para>
	///     ...then, to actually compile all T4 template files marked as "None" to source code, add the following block
	///     of XML after the <b>&lt;Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" /&gt;</b> line in the
	///     <b>.csproj</b> file:
	///   </para>
	///   <code>
	///     <![CDATA[
	/// <UsingTask TaskName="SoftwareNinjas.MSBuild.TemplateCompiler" AssemblyFile="SoftwareNinjas.MSBuild.dll" />
	/// <Target Name="BeforeTemplateCompilation">
	///   <CreateItem Include="@(None)" Condition="'%(Extension)'=='.tt'">
	///     <Output TaskParameter="Include" ItemName="_TemplateFiles" />
	///   </CreateItem>
	/// </Target>
	/// <Target Name="TemplateCompilation" DependsOnTargets="BeforeTemplateCompilation" Inputs="@(_TemplateFiles)"
	///   Outputs="@(_TemplateFiles->'%(RelativeDir)%(Filename).Generated.cs')"
	///   >
	///   <TemplateCompiler TemplateFiles="@(_TemplateFiles)" RootNamespace="$(RootNamespace)" />
	/// </Target>
	/// <Target Name="BeforeCompile" DependsOnTargets="TemplateCompilation">
	/// </Target>
	/// ]]>
	///   </code>
	/// </example>
	public class TemplateCompiler : Task
	{

		/// <summary>
		/// Template files to be compiled.
		/// </summary>
		public ITaskItem[] TemplateFiles
		{
			get;
			set;
		}

		/// <summary>
		/// The RootNamespace of the project, to use when building up the namespace of a generated class.
		/// </summary>
		public string RootNamespace
		{
			get;
			set;
		}

		/// <summary>
		/// Performs the template compilation.
		/// </summary>
		/// 
		/// <returns>
		/// <see langword="true"/> if the processing succeeded; <see langword="false"/> otherwise.
		/// </returns>
		public override bool Execute()
		{
			var host = new EngineHost();
			var rootNamespace = RootNamespace;
			var projectRootFolder = Environment.CurrentDirectory;
			var newLine = Environment.NewLine;
			foreach (var templateFile in TemplateFiles)
			{
				var inputPath = templateFile.ItemSpec;
				Log.LogMessage (MessageImportance.Normal, "Compiling T4 template {0}", inputPath);
				try
				{
					CompileTemplate (host, projectRootFolder, inputPath, rootNamespace, newLine);
				}
				catch(Exception e)
				{
					Log.LogErrorFromException (e);
				}
			}

			foreach (CompilerError err in host.Errors)
			{
				Log.LogError ("{0}({1},{2}): {3} {4}", err.FileName, err.Line, err.Column,
				              err.IsWarning? "WARNING" : "ERROR", err.ErrorText);
			}
			return !Log.HasLoggedErrors;
		}
		
		#region Helpers

		internal static void CompileTemplate (EngineHost host, string projectRootFolder, string inputPath,
			string rootNamespace, string newLine)
		{
			var className = Path.GetFileNameWithoutExtension (inputPath);
			host.TemplateFile = Path.GetFileName(inputPath);
			var targetFolder = Path.GetDirectoryName (inputPath);
			var absoluteInputPath = Path.Combine (projectRootFolder, inputPath);
			var inputText = File.ReadAllText (absoluteInputPath);
			var fullNamespace = rootNamespace;
			if (!String.IsNullOrEmpty(targetFolder))
			{
				fullNamespace += "." + ConvertPathToNamespace(targetFolder);
			}

			var fileName = "{0}.Generated.cs".FormatInvariant (className);
			var relativeOutputPath = Path.Combine (targetFolder, fileName);
			var absoluteOutputPath = Path.Combine (projectRootFolder, relativeOutputPath);
			using (var writer = new StreamWriter (absoluteOutputPath, false, Encoding.UTF8))
			{
				writer.NewLine = newLine;
				GenerateCode (host, inputText, fullNamespace, className, writer);
			}
		}

		internal static string ConvertPathToNamespace (string path)
		{
			return path.Replace ('/', '.').Replace ('\\', '.');
		}

		internal static void GenerateCode (ITextTemplatingEngineHost host, string content, string namespac, string name,
			                               TextWriter writer)
		{
			ParsedTemplate pt = ParsedTemplate.FromText (content, host);
			if (pt.Errors.HasErrors) {
				host.LogErrors (pt.Errors);
				return;
			}
			
			TemplateSettings settings = TemplatingEngine.GetSettings (host, pt);
			if (name != null)
				settings.Name = name;
			if (namespac != null)
				settings.Namespace = namespac;
			if (pt.Errors.HasErrors)
			{
				host.LogErrors (pt.Errors);
				return;
			}
			
			var ccu = TemplatingEngine.GenerateCompileUnit (host, pt, settings);
			if (pt.Errors.HasErrors) {
				host.LogErrors (pt.Errors);
				return;
			}
			
			var opts = new System.CodeDom.Compiler.CodeGeneratorOptions ();
			settings.Provider.GenerateCodeFromCompileUnit (ccu, writer, opts);
		}

		#endregion

	}
}
