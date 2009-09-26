// 
// TextTransformation.cs
//  
// Original Author:
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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Reflection;

namespace Microsoft.VisualStudio.TextTemplating
{
	
	
	/// <summary>
	/// The abstract base class for all generated transformation classes. This class also provides utility methods and
	/// properties for use in text template code.
	/// </summary>
	/// 
	/// <remarks>
	/// <para>
	/// The text template transformation process has two steps. In the first step, the text template transformation
	/// engine creates a class that is referred to as the generated transformation class. In the second step, the engine
	/// compiles and executes the generated transformation class, to produce the generated text output. The generated
	/// transformation class inherits from <see cref="TextTransformation"/>.
	/// </para>
	/// <para>
	/// Any class specified in an <c>inherits</c> directive in a text template must itself inherit from
	/// <see cref="TextTransformation"/>. <see cref="TransformText()"/> is the only abstract member of this class.
	/// </para>
	/// </remarks>
	public abstract class TextTransformation : IDisposable
	{
		Stack<int> indents = new Stack<int> ();
		string currentIndent = "";
		CompilerErrorCollection errors = new CompilerErrorCollection ();
		StringBuilder builder = new StringBuilder ();
		readonly IFormatProvider formatProvider;
		readonly object[] formatProviderAsParameterArray;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="TextTransformation"/> class.
		/// </summary>
		public TextTransformation () : this (CultureInfo.InvariantCulture)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="TextTransformation"/> class with the specified
		/// <paramref name="formatProvider"/>.
		/// </summary>
		/// 
		/// <param name="formatProvider">
		/// The <see cref="IFormatProvider"/> to use when converting <see cref="Object"/> instances to
		/// <see cref="String"/> instances.
		/// </param>
		public TextTransformation (IFormatProvider formatProvider)
		{
			this.formatProvider = formatProvider;
			this.formatProviderAsParameterArray = new object[] { formatProvider };
		}

		/// <summary>
		/// Initializes the <see cref="TextTransformation"/> class.  
		/// </summary>
		protected internal virtual void Initialize ()
		{
		}
		
		/// <summary>
		/// When overridden in a derived class, generates the text output of the transformation. 
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
		/// calls <see cref="TransformText"/> on the compiled generated transformation class to execute the text
		/// template and generate the text output.
		/// </remarks>
		public abstract string TransformText ();
		
		#region Errors
		
		/// <summary>
		/// Creates a new error to store information about errors that occur during the text template transformation
		/// process.
		/// </summary>
		/// 
		/// <param name="message">
		/// A message that describes the error.
		/// </param>
		/// 
		/// <remarks>
		/// Adds the error to the collection of <see cref="Errors"/> for the text template transformation process. Sets
		/// the line number of the error to -1, and the column number of the error to -1 by default.
		/// </remarks>
		public void Error (string message)
		{
			AddError (message);
		}
		
		/// <summary>
		/// Creates a new warning to store information about errors that occur during the text template transformation
		/// process.
		/// </summary>
		/// 
		/// <param name="message">
		/// A message that describes the warning.
		/// </param>
		/// 
		/// <remarks>
		/// Adds the warning to the collection of <see cref="Errors"/> for the text template transformation process.
		/// Sets the line number of the error to -1, and the column number of the error to -1 by default.
		/// </remarks>
		public void Warning (string message)
		{
			AddError (message).IsWarning = true;
		}
		
		CompilerError AddError (string message)
		{
			CompilerError err = new CompilerError ();
			err.Column = err.Line = -1;
			err.ErrorText = message;
			errors.Add (err);
			return err;
		}
		
		/// <summary>
		/// Gets the error collection for the text template transformation process.
		/// </summary>
		/// 
		/// <value>
		/// A <see cref="CompilerErrorCollection"/> that contains the errors and warnings raised during the text
		/// template transformation process. 
		/// </value>
		protected internal CompilerErrorCollection Errors {
			get { return errors; }
		}
		
		#endregion
		
		#region Indents
		
		/// <summary>
		/// Removes the most recently added text from <see cref="CurrentIndent"/>.
		/// </summary>
		/// 
		/// <returns>
		/// A <see cref="String"/> that contains the text most recently added to <see cref="CurrentIndent"/>.
		/// <see cref="CurrentIndent"/> is commonly called without capturing the return value.
		/// </returns>
		/// 
		/// <remarks>
		/// The <see cref="CurrentIndent"/> represents text that is prefixed to each line of the generated text output.
		/// The indent text can be spaces only, for example "    ", or it can include words.
		/// <see cref="PushIndent(String)"/> adds text to <see cref="CurrentIndent"/>, and can be called more than once.
		/// <see cref="PopIndent()"/> removes the most recently added text from <see cref="CurrentIndent"/>, and can be
		/// called more than once. <see cref="ClearIndent"/> removes all text from the <see cref="CurrentIndent"/>.
		/// </remarks>
		public string PopIndent ()
		{
			if (indents.Count == 0)
				return "";
			int lastPos = currentIndent.Length - indents.Pop ();
			string last = currentIndent.Substring (lastPos);
			currentIndent = currentIndent.Substring (0, lastPos);
			return last; 
		}
		
		/// <summary>
		/// Adds text to <see cref="CurrentIndent"/>, which is prefixed to each line of the generated text output. 
		/// </summary>
		/// 
		/// <param name="indent">
		/// The text to add to <see cref="CurrentIndent"/>. If <see cref="CurrentIndent"/> already contains text,
		/// <paramref name="indent"/> is appended to the existing text.
		/// </param>
		/// 
		/// <remarks>
		/// The <see cref="CurrentIndent"/> represents text that is prefixed to each line of the generated text output.
		/// The indent text can be spaces only, for example "    ", or it can include words.
		/// <see cref="PushIndent(String)"/> adds text to <see cref="CurrentIndent"/>, and can be called more than once.
		/// <see cref="PopIndent()"/> removes the most recently added text from <see cref="CurrentIndent"/>, and can be
		/// called more than once. <see cref="ClearIndent"/> removes all text from the <see cref="CurrentIndent"/>.
		/// </remarks>
		public void PushIndent (string indent)
		{
			indents.Push (indent.Length);
			currentIndent += indent;
		}
		
		/// <summary>
		/// Resets the <see cref="CurrentIndent"/> to an empty string.
		/// </summary>
		/// 
		/// <remarks>
		/// The <see cref="CurrentIndent"/> represents text that is prefixed to each line of the generated text output.
		/// The indent text can be spaces only, for example "    ", or it can include words.
		/// <see cref="PushIndent(String)"/> adds text to <see cref="CurrentIndent"/>, and can be called more than once.
		/// <see cref="PopIndent()"/> removes the most recently added text from <see cref="CurrentIndent"/>, and can be
		/// called more than once. <see cref="ClearIndent"/> removes all text from the <see cref="CurrentIndent"/>.
		/// </remarks>
		public void ClearIndent ()
		{
			currentIndent = "";
			indents.Clear ();
		}
		
		/// <summary>
		/// Gets the current indent text, which is prefixed to each line of the generated text output.
		/// </summary>
		/// 
		/// <value>
		/// A <see cref="String"/> that contains the text that is prefixed to each line of the generated text output.
		/// </value>
		/// 
		/// <remarks>
		/// The <see cref="CurrentIndent"/> represents text that is prefixed to each line of the generated text output.
		/// The indent text can be spaces only, for example "    ", or it can include words.
		/// <see cref="PushIndent(String)"/> adds text to <see cref="CurrentIndent"/>, and can be called more than once.
		/// <see cref="PopIndent()"/> removes the most recently added text from <see cref="CurrentIndent"/>, and can be
		/// called more than once. <see cref="ClearIndent"/> removes all text from the <see cref="CurrentIndent"/>.
		/// </remarks>
		public string CurrentIndent {
			get { return currentIndent; }
		}
		
		#endregion
		
		#region Writing
		
		
		/// <summary>
		/// Returns a <b>String</b> that represents the specified <b>Object</b>.
		/// </summary>
		/// 
		/// <param name="objectToConvert">
		/// The <see cref="Object"/> to represent as a <see cref="String"/>.
		/// </param>
		/// 
		/// <returns>
		/// A <b>String</b> that represents the specified <b>Object</b>.
		/// </returns>
		internal string ToStringWithCulture (object objectToConvert)
		{
			if (objectToConvert == null)
				return null;

			IConvertible conv = objectToConvert as IConvertible;
			if (conv != null)
				return conv.ToString(formatProvider);

			MethodInfo mi = objectToConvert.GetType ().GetMethod ("ToString", new Type[] { typeof (IFormatProvider) });
			if (mi != null && mi.ReturnType == typeof (String))
				return (string) mi.Invoke (objectToConvert, formatProviderAsParameterArray);
			return objectToConvert.ToString ();
		}
		
		/// <summary>
		/// Gets or sets the string that the text template transformation process is using to assemble the generated
		/// text output.
		/// </summary>
		/// 
		/// <value>
		/// A <see cref="StringBuilder"/> that contains the generated text transformation.
		/// </value>
		protected StringBuilder GenerationEnvironment {
			get { return builder; }
			set {
				if (value == null)
					throw new ArgumentNullException ();
				builder = value;
			}
		}
		
		/// <summary>
		/// Appends a string representation of the specified object to the generated text output.
		/// </summary>
		/// 
		/// <param name="objectToAppend">
		/// The <see cref="Object"/> to append.
		/// </param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para>
		/// Enlarging the value of the underlying <see cref="StringBuilder"/> would exceed
		/// <see cref="StringBuilder.MaxCapacity"/>.
		/// </para>
		/// </exception>
		public void Write (object objectToAppend)
		{
			GenerationEnvironment.Append (ToStringWithCulture (objectToAppend));
		}
		
		/// <summary>
		/// Appends a copy of the specified string to the generated text output.
		/// </summary>
		/// 
		/// <param name="textToAppend">
		/// The string to append.
		/// </param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para>
		/// Enlarging the value of the underlying <see cref="StringBuilder"/> would exceed
		/// <see cref="StringBuilder.MaxCapacity"/>.
		/// </para>
		/// </exception>
		public void Write (string textToAppend)
		{
			GenerationEnvironment.Append (textToAppend);
		}
		
		/// <summary>
		/// Appends a formatted string, which contains zero or more format specifications, to the generated text output.
		/// Each format specification is replaced by the string representation of a corresponding object argument.
		/// </summary>
		/// 
		/// <param name="format">
		/// A string that contains zero or more format specifications.
		/// </param>
		/// 
		/// <param name="args">
		/// An array of objects to format.
		/// </param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <para>
		/// <paramref name="format"/> is <see langword="null"/>.
		/// </para>
		/// <para>
		/// -or-
		/// </para>
		/// <para>
		/// <paramref name="args"/> is <see langword="null"/>.
		/// </para>
		/// </exception>
		/// 
		/// <exception cref="FormatException">
		/// <para>
		/// <paramref name="format"/> is invalid.
		/// </para>
		/// </exception>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para>
		/// Enlarging the value of the underlying <see cref="StringBuilder"/> would exceed
		/// <see cref="StringBuilder.MaxCapacity"/>.
		/// </para>
		/// </exception>
		public void Write (string format, params object[] args)
		{
			GenerationEnvironment.AppendFormat (format, args);
		}
		
		/// <summary>
		/// Appends a copy of the specified string and the default line terminator to the generated text output.
		/// </summary>
		/// 
		/// <param name="textToAppend">
		/// The string to write.
		/// </param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para>
		/// Enlarging the value of the underlying <see cref="StringBuilder"/> would exceed
		/// <see cref="StringBuilder.MaxCapacity"/>.
		/// </para>
		/// </exception>
		public void WriteLine (string textToAppend)
		{
			GenerationEnvironment.Append (CurrentIndent);
			GenerationEnvironment.AppendLine (textToAppend);
		}
		
		/// <summary>
		/// Appends a formatted string, which contains zero or more format specifications, and the default line
		/// terminator, to the generated text output. Each format specification is replaced by the string representation
		/// of a corresponding object argument.
		/// </summary>
		/// 
		/// <param name="format">
		/// A string that contains zero or more format specifications.
		/// </param>
		/// 
		/// <param name="args">
		/// An array of objects to format.
		/// </param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <para>
		/// <paramref name="format"/> is <see langword="null"/>.
		/// </para>
		/// <para>
		/// -or-
		/// </para>
		/// <para>
		/// <paramref name="args"/> is <see langword="null"/>.
		/// </para>
		/// </exception>
		/// 
		/// <exception cref="FormatException">
		/// <para>
		/// <paramref name="format"/> is invalid.
		/// </para>
		/// </exception>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para>
		/// Enlarging the value of the underlying <see cref="StringBuilder"/> would exceed
		/// <see cref="StringBuilder.MaxCapacity"/>.
		/// </para>
		/// </exception>
		public void WriteLine (string format, params object[] args)
		{
			GenerationEnvironment.Append (CurrentIndent);
			GenerationEnvironment.AppendFormat (format, args);
			GenerationEnvironment.AppendLine ();
		}

		#endregion
		
		#region Dispose
		
		/// <summary>
		/// Releases all resources used by the <see cref="TextTransformation"/>.
		/// </summary>
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
		
		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="TextTransformation"/> and optionally releases the
		/// managed resources.
		/// </summary>
		/// 
		/// <param name="disposing">
		/// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release
		/// only unmanaged resources.
		/// </param>
		protected virtual void Dispose (bool disposing)
		{
		}
		
		/// <summary>
		/// Destructor.
		/// </summary>
		~TextTransformation ()
		{
			Dispose (false);
		}
		
		#endregion

	}
}
