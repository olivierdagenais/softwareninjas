using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace SoftwareNinjas.Core.TestDoubles.Stubs
{
    public class StubGenerator
    {
        // TODO: move to StringExtensions
        const char CR = '\r';
        const char LF = '\n';
        static internal IEnumerable<string> EachLine(string input)
        {
            StringBuilder line = new StringBuilder();
            char lastChar = '\0';
            foreach (char c in input)
            {
                switch (c)
                {
                    case LF:
                        yield return line.ToString();
                        line.Length = 0;
                        break;
                    case CR:
                        break;
                    default:
                        if (CR == lastChar)
                        {
                            yield return line.ToString();
                            line.Length = 0;
                        }
                        line.Append(c);
                        break;
                }
                lastChar = c;
            }
            if (line.Length > 0)
            {
                yield return line.ToString();
            }
        }

        private static readonly RegexOptions standardOptions =
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace;
        // TODO: support interfaces that inherit from others
        private static readonly Regex interfaceDeclaration = new Regex(@"
(?<visibility>private|protected|internal|public)?\s*
interface\s+
I(?<interfaceName>\S+)
", 
            standardOptions);
        internal static Pair<string, string> DecodeInterfaceLine(string line)
        {
            var m = interfaceDeclaration.Match(line);
            if (!m.Success)
            {
                return null;
            }
            var visGroup = m.Groups["visibility"];
            string visibility = "public";
            if (visGroup.Success)
            {
                visibility = visGroup.Value;
            }
            string interfaceName = m.Groups["interfaceName"].Value;

            return new Pair<string,string>(visibility, interfaceName);
        }

        internal static string BuildClassLine(string visibility, string interfaceName)
        {
            return String.Format("{0} class S{1} : I{1}", visibility, interfaceName);
        }

        internal static string ExtractLeadingWhitespace(string line)
        {
            string trimmed = line.TrimStart();
            int numLeadingCharacters = line.Length - trimmed.Length;
            string leading = line.Substring(0, numLeadingCharacters);
            return leading;
        }

        private static readonly Regex methodDeclaration = new Regex(@"
(?<returnType>\S+)\s+
(?<methodName>\S+)\s*
\(\s*
   (?<arguments>[^)]+)?
\);
", standardOptions);

        internal static ParsedMethod DecodeMethodLine(string line)
        {
            var m = methodDeclaration.Match(line);
            if (!m.Success)
            {
                return null;
            }
            var methodName = m.Groups["methodName"].Value;
            var method = new ParsedMethod(methodName);

            var retval = new ParsedParameter(m.Groups["returnType"].Value, "retval");
            method.Parameters.Add(retval);

            var argGroup = m.Groups["arguments"];
            if (argGroup.Success)
            {
                foreach (var pair in DecodeArguments(argGroup.Value))
                {
                    method.Parameters.Add(new ParsedParameter(pair.First, pair.Second));
                }
            }

            return method;
        }

        private static readonly Regex argumentsDeclaraion = new Regex(@"
(?<argType>\S+)\s*
(?<argName>[^\s,]+)\s*
,?
", standardOptions);

        internal static IEnumerable<Pair<string, string>> DecodeArguments(string arguments)
        {
            var matches = argumentsDeclaraion.Matches(arguments);
            foreach (Match m in matches)
            {
                yield return new Pair<string, string>(m.Groups["argType"].Value, m.Groups["argName"].Value);
            }
        }

        internal static string Generate(string interfaceSource)
        {
            StringBuilder output = new StringBuilder(interfaceSource.Length * 2);
            string interfaceName = null;
            foreach (string line in EachLine(interfaceSource))
            {
                var leadingSpace = ExtractLeadingWhitespace(line);
                var pair = DecodeInterfaceLine(line);
                if (pair != null)
                {
                    // [visibility] interface I{interfaceName} => [visibility] class S{interfaceName} : I{interfaceName}
                    interfaceName = pair.Second;
                    output.Append(leadingSpace);
                    output.AppendLine(BuildClassLine(pair.First, interfaceName));
                }
                else
                {
                    var method = DecodeMethodLine(line);
                    if (method != null)
                    {
                        // {returnType} {methodName} ([{argType} {argName}[,...]]);
                        // =>
                        // public {Action|Func[<{argType}, ...>]} {methodName};
                        // {returnType} I{interfaceName}.{methodName}([{argType} {argName}[,...]])
                        // {
                        //     if ({methodName} != null)
                        //     {
                        //         {methodName}([{argumentName}[,...]]);
                        //     }
                        // }
                        output.Append(leadingSpace);
                        output.AppendLine(BuildFieldLine(method));

                        output.Append(leadingSpace);
                        output.AppendLine(BuildInterfaceMethodImplementation(leadingSpace, interfaceName, method));
                    }
                    else
                    {
                        // default => pass through
                        output.AppendLine(line);
                    }
                }
            }
            return output.ToString();
        }

        internal static string BuildInterfaceMethodImplementation(string leadingSpace, string interfaceName, ParsedMethod method)
        {
            string paramDecl = method.CallableParameters.Join(", ");
            string paramUse = method.CallableParameters.Join(", ", (pp) => pp.Name);

            string result;
            if (method.HasReturnType)
            {
                // TODO: need to return something appropriate for the return type if the field isn't set, not just null
                string returnType = method.Parameters[0].Type;
                result = String.Format(@"{1} I{2}.{3}({4})
{0}{{
{0}    if ({3} != null)
{0}    {{
{0}        return {3}({5});
{0}    }}
{0}    else
{0}    {{
{0}        return null;
{0}    }}
{0}}}",
                leadingSpace, returnType, interfaceName, method.Name, paramDecl, paramUse);
            }
            else
            {
               result = String.Format(@"void I{1}.{2}({3})
{0}{{
{0}    if ({2} != null)
{0}    {{
{0}        {2}({4});
{0}    }}
{0}}}",
                leadingSpace, interfaceName, method.Name, paramDecl, paramUse);
            }
            return result;
        }

        internal static string BuildFieldLine(ParsedMethod method)
        {
            string functorType = String.Empty;
            if (method.HasArguments)
            {
                IEnumerable<ParsedParameter> orderedParameters = method.CallableParameters;
                if (method.HasReturnType)
                {
                    orderedParameters = orderedParameters.Compose(method.Parameters[0]);
                }

                functorType = String.Format("<{0}>", orderedParameters.Join(", ", (pp) => pp.Type));
            }
            return String.Format("public {0}{1} {2};", 
                method.HasReturnType ? "Func" : "Action", functorType, method.Name);
        }
    }
}
