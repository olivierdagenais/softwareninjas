﻿using System;
using System.Collections.Generic;

using Parent = SoftwareNinjas.MSBuild;
using NUnit.Framework;
using Microsoft.VisualStudio.TextTemplating;
using Mono.TextTemplating;
using System.IO;

namespace SoftwareNinjas.MSBuild.Test
{
	
	/// <summary>
	/// Because the MSBuild &lt;UsingTask&gt; 
	/// </summary>
	[TestFixture]
	public class TemplateCompiler
	{
		const string ParseSample1 =
@"<#@ template language=""C#v3.5"" #>
Line One
Line Two
<#
foo
#>
Line Three <#= bar #>
Line Four
<#+ 
baz \#>
#>
";

		[Test]
		public void GenerateCode_Typical ()
		{
			string input = ParseSample1.Replace("\r\n", "\n");
			string expectedOutput = OutputSample1;
			Generate(
				input,
				"Microsoft.VisualStudio.TextTemplating", 
				"GeneratedTextTransformation4f504ca0", 
				expectedOutput
			);
		}

		void Generate (string input, string namespac, string className, string expectedOutput)
		{
			var host = new EngineHost();
			string actualOutput;
			using (var writer = new StringWriter())
			{
				Parent.TemplateCompiler.GenerateCode(host, input, namespac, className, writer);
				actualOutput = writer.ToString();
			}

			Assert.AreEqual (0, host.Errors.Count);
			var output = StripHeader (actualOutput);
			Assert.AreEqual (expectedOutput, output);
		}

		[Test]
		public void StripHeader_Typical()
		{
			var input = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4918
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.VisualStudio.TextTemplating {
";

			var expectedOutput = @"
namespace Microsoft.VisualStudio.TextTemplating {
";
			var actualOutput = StripHeader (input);
			Assert.AreEqual (expectedOutput, actualOutput);
		}

		static string StripHeader (string input)
		{
			using (var writer = new StringWriter ())
			{
				using (var reader = new StringReader (input))
				{
					for (int i = 0; i < 9; i++)
					{
						reader.ReadLine ();
					}
					string line;
					while (( line = reader.ReadLine () ) != null)
					{
						writer.WriteLine (line);
					}
				}
				return writer.ToString ();
			}
		}
		
		#region Expected output strings
		
		public static string OutputSample1 = 
@"
namespace Microsoft.VisualStudio.TextTemplating {
    
    
    public partial class GeneratedTextTransformation4f504ca0 : Microsoft.VisualStudio.TextTemplating.TextTransformation {
        
        
        #line 9 """"
 
baz \#>

        #line default
        #line hidden
        
        
        public GeneratedTextTransformation4f504ca0() : 
                base() {
        }
        
        public GeneratedTextTransformation4f504ca0(System.IFormatProvider formatProvider) : 
                base(formatProvider) {
        }
        
        public override string TransformText() {
            
            #line 2 """"
            this.Write(""Line One\nLine Two\n"");
            
            #line default
            #line hidden
            
            #line 4 """"

foo

            
            #line default
            #line hidden
            
            #line 7 """"
            this.Write(""Line Three "");
            
            #line default
            #line hidden
            
            #line 7 """"
            this.Write( bar );
            
            #line default
            #line hidden
            
            #line 7 """"
            this.Write(""\nLine Four\n"");
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
    }
}
";
		#endregion
	}
}
