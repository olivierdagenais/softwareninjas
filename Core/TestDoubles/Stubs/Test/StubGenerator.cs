using System;
using System.Collections.Generic;
using System.Text;

using Parent = SoftwareNinjas.Core.TestDoubles.Stubs;
using EE = SoftwareNinjas.Core.Test.EnumerableExtensions;
using NUnit.Framework;

namespace SoftwareNinjas.Core.TestDoubles.Stubs.Test
{
    [TestFixture]
    public class StubGenerator
    {
        private static readonly ParsedMethod runNoArgs = new ParsedMethod("Run")
        {
            { "void", "retval" }
        };

        // IEnumerable<Pair<string, string>> DecodeArguments(string arguments)
        private static readonly ParsedMethod decodeArguments = new ParsedMethod("DecodeArguments")
        {
            { "IEnumerable<Pair<string, string>>", "retval" },
            { "string", "arguments" }
        };

        [Test]
        public void EachLine_Empty()
        {
            EE.EnumerateSame(new string[] { }, Parent.StubGenerator.EachLine(String.Empty));
        }

        [Test]
        public void EachLine_OneLine()
        {
            EE.EnumerateSame(new string[] { "one" }, Parent.StubGenerator.EachLine("one"));
        }

        [Test]
        public void EachLine_EndsWithEmptyLine()
        {
            EE.EnumerateSame(new string[] { "one" }, Parent.StubGenerator.EachLine("one\n"));
        }

        [Test]
        public void EachLine_ConsecutiveEmptyLines()
        {
            EE.EnumerateSame(new string[] { "one", "" }, Parent.StubGenerator.EachLine("one\n\n"));
        }

        [Test]
        public void EachLine_CR()
        {
            EE.EnumerateSame(new string[] { "one", "two" }, Parent.StubGenerator.EachLine("one\rtwo"));
        }

        [Test]
        public void EachLine_LF()
        {
            EE.EnumerateSame(new string[] { "one", "two" }, Parent.StubGenerator.EachLine("one\ntwo"));
        }

        [Test]
        public void EachLine_CRLF()
        {
            EE.EnumerateSame(new string[] { "one", "two" }, Parent.StubGenerator.EachLine("one\r\ntwo"));
        }

        [Test]
        public void DecodeInterfaceLine_NoMatch()
        {
            Assert.IsNull(Parent.StubGenerator.DecodeInterfaceLine("    {"));
            Assert.IsNull(Parent.StubGenerator.DecodeInterfaceLine("void Run();"));
        }

        [Test]
        public void DecodeInterfaceLine_Typical()
        {
            var actual = Parent.StubGenerator.DecodeInterfaceLine("    public interface IOneMethodNoArgs");
            Assert.AreEqual("public", actual.First);
            Assert.AreEqual("OneMethodNoArgs", actual.Second);
        }

        [Test]
        public void BuildClassLine_Typical()
        {
            Assert.AreEqual("public class SOneMethodNoArgs : IOneMethodNoArgs",
                Parent.StubGenerator.BuildClassLine("public", "OneMethodNoArgs"));
        }

        [Test]
        public void ExtractLeadingWhitespace_None()
        {
            Assert.AreEqual(String.Empty, Parent.StubGenerator.ExtractLeadingWhitespace("none"));
        }

        [Test]
        public void ExtractLeadingWhitespace_Space()
        {
            Assert.AreEqual(" ", Parent.StubGenerator.ExtractLeadingWhitespace(" space"));
        }

        [Test]
        public void ExtractLeadingWhitespace_Tab()
        {
            Assert.AreEqual("\t", Parent.StubGenerator.ExtractLeadingWhitespace("\ttab"));
        }

        [Test]
        public void ExtractLeadingWhitespace_Spaces()
        {
            Assert.AreEqual("    ",
                Parent.StubGenerator.ExtractLeadingWhitespace("    public interface IOneMethodNoArgs"));
        }

        [Test]
        public void DecodeArguments_One()
        {
            var expected = new PairList<string, string>
            {
                { "int", "x" },
            };
            EE.EnumerateSame(expected,
                Parent.StubGenerator.DecodeArguments("int x"));
        }

        [Test]
        public void DecodeArguments_Two()
        {
            var expected = new PairList<string, string>
            {
                { "string", "visibility" },
                { "string", "interfaceName" }
            };
            EE.EnumerateSame(expected,
                Parent.StubGenerator.DecodeArguments("string visibility, string interfaceName"));
        }

        [Test]
        public void BuildFieldLine_Action()
        {
            Assert.AreEqual("public Action Run;", 
                Parent.StubGenerator.BuildFieldLine(runNoArgs));
        }

        [Test]
        public void BuildFieldLine_Typical()
        {
            Assert.AreEqual("public Func<string, IEnumerable<Pair<string, string>>> DecodeArguments;",
                Parent.StubGenerator.BuildFieldLine(decodeArguments));
        }

        [Test]
        public void BuildInterfaceMethodImplementation_Typical()
        {
            Assert.AreEqual(@"IEnumerable<Pair<string, string>> IStub.DecodeArguments(string arguments)
{
    if (DecodeArguments != null)
    {
        return DecodeArguments(arguments);
    }
    else
    {
        return null;
    }
}",
                Parent.StubGenerator.BuildInterfaceMethodImplementation(String.Empty, "Stub", decodeArguments));
        }

        [Test]
        public void Compile_OneMethodNoArgs()
        {
            var interfaceSource = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareNinjas.Core.TestDoubles.Stubs.Test
{
    public interface IOneMethodNoArgs
    {
        void Run();
    }
}
";
            string actualStubSource = Parent.StubGenerator.Generate(interfaceSource);
            string expectedStubSource = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareNinjas.Core.TestDoubles.Stubs.Test
{
    public class SOneMethodNoArgs : IOneMethodNoArgs
    {
        public Action Run;
        void IOneMethodNoArgs.Run()
        {
            if (Run != null)
            {
                Run();
            }
        }
    }
}
";
            Assert.AreEqual(expectedStubSource, actualStubSource);
        }
    }
}
