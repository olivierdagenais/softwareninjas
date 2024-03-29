// 
// GenerationTests.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Microsoft.VisualStudio.TextTemplating;

namespace Mono.TextTemplating.Tests
{
	
	
	[TestFixture]
	public class GenerationTests
	{	
		[Test]
		public void Generate ()
		{
			string Input = ParsingTests.ParseSample1;
			string Output = OutputSample1;
			Generate (Input, Output, "\n");
		}
		
		[Test]
		public void GenerateMacNewlines ()
		{
			string MacInput = ParsingTests.ParseSample1.Replace ("\n", "\r");
			string MacOutput = OutputSample1.Replace ("\\n", "\\r").Replace ("\n", "\r");;
			Generate (MacInput, MacOutput, "\r");
		}
		
		[Test]
		public void GenerateWindowsNewlines ()
		{
			string WinInput = ParsingTests.ParseSample1.Replace ("\n", "\r\n");
			string WinOutput = OutputSample1.Replace ("\\n", "\\r\\n").Replace ("\n", "\r\n");
			Generate (WinInput, WinOutput, "\r\n");
		}
		
		//NOTE: we set the newline property on the code generator so that the whole files has matching newlines,
		// in order to match the newlines in the verbatim code blocks
		void Generate (string input, string expectedOutput, string newline)
		{
			DummyHost host = new DummyHost ();
			string className = "GeneratedTextTransformation4f504ca0";
			string code = GenerateCode (host, input, className, newline);
			Assert.AreEqual (0, host.Errors.Count);
			Assert.AreEqual (expectedOutput, StripHeader(code, newline));
		}
		
		#region Helpers
		
		string GenerateCode (ITextTemplatingEngineHost host, string content, string name, string generatorNewline)
		{
			ParsedTemplate pt = ParsedTemplate.FromText (content, host);
			if (pt.Errors.HasErrors) {
				host.LogErrors (pt.Errors);
				return null;
			}
			
			TemplateSettings settings = TemplatingEngine.GetSettings (host, pt);
			if (name != null)
				settings.Name = name;
			if (pt.Errors.HasErrors) {
				host.LogErrors (pt.Errors);
				return null;
			}
			
			var ccu = TemplatingEngine.GenerateCompileUnit (host, pt, settings);
			if (pt.Errors.HasErrors) {
				host.LogErrors (pt.Errors);
				return null;
			}
			
			var opts = new System.CodeDom.Compiler.CodeGeneratorOptions ();
			using (var writer = new System.IO.StringWriter ()) {
				writer.NewLine = generatorNewline;
				settings.Provider.GenerateCodeFromCompileUnit (ccu, writer, opts);
				return writer.ToString ();
			}
		}

        static string StripHeader(string input, string newLine)
        {
            using (var writer = new StringWriter())
            {
                using (var reader = new StringReader(input))
                {
                    for (int i = 0; i < 9; i++)
                    {
                        reader.ReadLine();
                    }
                    string line;
                    while (( line = reader.ReadLine() ) != null)
                    {
                        writer.Write(line);
                        writer.Write(newLine);
                    }
                }
                return writer.ToString();
            }
        }
		
		#endregion
		
		#region Expected output strings
		
		public static string OutputSample1 = 
@"
namespace Microsoft.VisualStudio.TextTemplating {
    
    
    public partial class GeneratedTextTransformation4f504ca0 : Microsoft.VisualStudio.TextTemplating.TextTransformation {
        
        
        #line 9 """"
 
baz \#>

        #line default
        #line hidden
        
        
        /// <summary>
        /// Initializes a new instance of the <see cref=""GeneratedTextTransformation4f504ca0""/> class.
        /// </summary>
        public GeneratedTextTransformation4f504ca0() : 
                base() {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref=""GeneratedTextTransformation4f504ca0""/> class
        /// using the specified <paramref name=""formatProvider""/>
        /// </summary>
        /// 
        /// <param name=""formatProvider"">
        /// The <see cref=""IFormatProvider""/> to use when converting <see cref=""Object""/> instances to
        /// <see cref=""String""/> instances.
        /// </param>
        public GeneratedTextTransformation4f504ca0(System.IFormatProvider formatProvider) : 
                base(formatProvider) {
        }
        
        /// <summary>
        /// Generates the text output of the transformation.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the generated text output of the text template transformation process.
        /// </returns>
        /// 
        /// <remarks>
        /// The text template transformation process has two steps. In the first step, the text template transformation
        /// engine creates a class that is named the generated transformation class. In the second step, the engine
        /// compiles and executes the generated transformation class, to produce the generated text output. The engine
        /// calls <see cref=""TransformText""/> on the compiled generated transformation class to execute the text
        /// template and generate the text output.
        /// </remarks>
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
