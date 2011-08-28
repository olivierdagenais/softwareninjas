using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NAnt.Core;
using NAnt.Core.Attributes;
using SoftwareNinjas.Core;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// Compares the public interface of two <see cref="Assembly"/> instances.
    /// </summary>
    /// <example>
    ///   <para>
    ///   The example will set the <c>numberOfDifferences</c> property to the result of comparing both versions of
    ///   the <c>MyProject.dll</c> assembly.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <publicInterfaceComparer
    ///     baseline="base/MyProject/bin/Debug/MyProject.dll"
    ///     challenger="challenger/MyProject/bin/Debug/MyProject.dll"
    ///     property="numberOfDifferences"
    /// />
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("publicInterfaceComparer")]
    public class PublicInterfaceComparerTask : TestableTask
    {
        /// <summary>
        /// Default constructor for NAnt itself.
        /// </summary>
        public PublicInterfaceComparerTask() : base(true)
        {
            
        }

        internal PublicInterfaceComparerTask(bool logging) : base(logging)
        {
        }

        /// <summary>
        /// The path to the baseline <see cref="Assembly"/>.
        /// </summary>
        [TaskAttribute("baseline", Required = true)]
        public FileInfo BaselineFile
        {
            get;
            set;
        }

        /// <summary>
        /// The path to the "challenger" <see cref="Assembly"/>, against which to compare the "baseline".
        /// </summary>
        [TaskAttribute("challenger", Required = true)]
        public FileInfo ChallengerFile
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the property that receives number of public interface differences.
        /// </summary>
        [TaskAttribute("property", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string Property
        {
            get;
            set;
        }

        /// <summary>
        /// The path to the report file to write.
        /// </summary>
        [TaskAttribute("report", Required = false)]
        public FileInfo ReportFile
        {
            get;
            set;
        }

        /// <summary>
        /// Runs the public interface comparison task.
        /// </summary>
        protected override void ExecuteTask()
        {
            var baselineFullPath = BaselineFile.FullName;
            Log(Level.Verbose, "Loading baseline assembly from {0}", baselineFullPath);
            var baseline = Assembly.LoadFile(baselineFullPath);

            var challengerFullPath = ChallengerFile.FullName;
            Log(Level.Verbose, "Loading challenger assembly from {0}", challengerFullPath);
            var challenger = Assembly.LoadFile(challengerFullPath);

            Log(Level.Info, "Comparing {0} to {1}", baselineFullPath, challengerFullPath);
            var results = Compare(baseline, challenger);
            var count = 0;
            using (var report = ReportFile == null ? TextWriter.Null : new StreamWriter(ReportFile.FullName, false))
            {
                foreach (var memberInfo in results)
                {
                    var description = Describe(memberInfo);
                    Log(Level.Debug, "Difference: {0}", description);
                    report.WriteLine(description);
                    count++;
                }
            }
            if (Property != null)
            {
                Project.Properties[Property] = count.ToString();
            }
        }

        internal static IEnumerable<MemberInfo> Compare(MemberInfo baselineMember, Dictionary<MemberInfo, MemberInfo> challengerMemberDictionary)
        {
            MemberInfo foundChallenger = null;
            foreach (var challengerMember in challengerMemberDictionary.Values)
            {
                if (challengerMember is Type)
                {
                    var baselineType = (Type) baselineMember;
                    var challengerType = (Type) challengerMember;
                    if (HaveSameName(baselineType, challengerType))
                    {
                        foundChallenger = challengerType;
                        foreach (var memberInfo in Compare(baselineType, challengerType))
                        {
                            yield return memberInfo;
                        }
                    }
                }
            }
            if (foundChallenger == null)
            {
            }
            else
            {
                challengerMemberDictionary.Remove(foundChallenger);
            }
        }

        internal static IEnumerable<Type> GetVisibleTypes(Assembly assembly)
        {
            return assembly.GetTypes().Filter(t => t.IsVisible);
        }

        internal static IEnumerable<MemberInfo> Compare(Assembly baseline, Assembly challenger)
        {
            var baselineTypes = GetVisibleTypes(baseline);
            var challengerTypeDictionary = CopyToDictionary((IEnumerable<Type>)GetVisibleTypes(challenger));
            foreach (var baselineType in baselineTypes)
            {
                Type foundChallenger = null;
                foreach (var challengerType in challengerTypeDictionary.Values)
                {
                    if (HaveSameName(baselineType, challengerType))
                    {
                        foundChallenger = challengerType;
                        break;
                    }
                }
                if (foundChallenger == null)
                {
                    yield return baselineType;
                }
                else
                {
                    challengerTypeDictionary.Remove(foundChallenger);
                    foreach (var memberInfo in Compare(baselineType, foundChallenger))
                    {
                        yield return memberInfo;
                    }
                }
            }
            foreach (var challengerType in challengerTypeDictionary.Values)
            {
                yield return challengerType;
            }
        }

        internal static IDictionary<Type, Type> CopyToDictionary(IEnumerable<Type> types)
        {
            var result = new Dictionary<Type, Type>();
            foreach (var type in types)
            {
                result.Add(type, type);
            }
            return result;
        }

        internal static bool AreEqual(MemberInfo baseline, MemberInfo challenger)
        {
            if (baseline is MethodInfo)
            {
                if (challenger is MethodInfo)
                {
                    return AreEqual((MethodBase) baseline, (MethodBase) challenger);
                }
                return false;
            }
            if (baseline is ConstructorInfo)
            {
                if (challenger is ConstructorInfo)
                {
                    return AreEqual((MethodBase) baseline, (MethodBase) challenger);
                }
                return false;
            }
            if (baseline is FieldInfo)
            {
                if (challenger is FieldInfo)
                {
                    return AreEqual((FieldInfo) baseline, (FieldInfo) challenger);
                }
                return false;
            }
            throw new ArgumentException("Parameter 'baseline' is an unsupported type.", "baseline");
        }

        internal static bool AreEqual(FieldInfo baseline, FieldInfo challenger)
        {
            return baseline.Name == challenger.Name;
        }

        internal static bool AreEqual(MethodBase baseline, MethodBase challenger)
        {
            var result = baseline.Name == challenger.Name
                           && HaveSameName(baseline.DeclaringType, challenger.DeclaringType)
                           && IsProtected(baseline) == IsProtected(challenger)
                           && IsPublic(baseline) == IsPublic(challenger)
                           && baseline.IsStatic == challenger.IsStatic;
            var baseParameters = baseline.GetParameters();
            var challengerParameters = challenger.GetParameters();
            if (result)
            {
                result = baseParameters.Length == challengerParameters.Length;
            }
            if (result)
            {
                for (var i = 0; i < baseParameters.Length; i++)
                {
                    var baseParameter = baseParameters[i].ParameterType;
                    var challengerParameter = challengerParameters[i].ParameterType;
                    if (!HaveSameName(baseParameter, challengerParameter))
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        internal static bool AreEqual(PropertyInfo baseline, PropertyInfo challenger)
        {
            var result = baseline.Name == challenger.Name
                           && HaveSameName(baseline.DeclaringType, challenger.DeclaringType);
            if (result)
            {
                var baselineMembers = baseline.GetAccessors();
                var challengerMembers = challenger.GetAccessors();
                var comparison = Compare(baselineMembers, challengerMembers);
                var e = comparison.GetEnumerator();
                // the properties are equal if there are no differences in their accessor methods
                result = !e.MoveNext();
            }
            return result;
        }

        internal static bool HaveSameName(Type baseline, Type challenger)
        {
            var result = baseline.Namespace == challenger.Namespace
                         && baseline.Name == challenger.Name;
            var baselineGenericArguments = baseline.GetGenericArguments();
            var challengerGenericArguments = challenger.GetGenericArguments();
            if (result)
            {
                result = baselineGenericArguments.Length == challengerGenericArguments.Length;
            }
            if (result)
            {
                for (var i = 0; i < baselineGenericArguments.Length; i++)
                {
                    var baselineGenericArgument = baselineGenericArguments[i];
                    var challengerGenericArgument = challengerGenericArguments[i];
                    if (!HaveSameName(baselineGenericArgument, challengerGenericArgument))
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        internal static Dictionary<MemberInfo, MemberInfo> CopyToDictionary(IEnumerable<MemberInfo> memberInfos)
        {
            var members = new Dictionary<MemberInfo, MemberInfo>();
            foreach (var memberInfo in memberInfos)
            {
                members.Add(memberInfo, memberInfo);
            }
            return members;
        }

        internal static bool AreEqual(Type baseline, Type challenger)
        {
            var comparison = Compare(baseline, challenger);
            var e = comparison.GetEnumerator();
            // two types are equal if there are no differences between them
            var result = !e.MoveNext();
            return result;
        }

        internal static IEnumerable<MemberInfo> Compare(Type baseline, Type challenger)
        {
            var result = HaveSameName(baseline, challenger);
            if (result)
            {
                var baselineMembers = GetVisibleMembers(baseline);
                var challengerMembers = GetVisibleMembers(challenger);
                foreach (var memberInfo in Compare(baselineMembers, challengerMembers))
                {
                    yield return memberInfo;
                }
            }
        }

        internal static IEnumerable<MemberInfo> GetVisibleMembers(Type type)
        {
            var unfiltered = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic
                                             | BindingFlags.Instance | BindingFlags.Static);
            var result = unfiltered.Filter(IsVisible);
            return result;
        }

        internal static IEnumerable<MemberInfo> Compare
            (IEnumerable<MemberInfo> baselineMembers, IEnumerable<MemberInfo> challengerMembers)
        {
            var challengerMemberDictionary = CopyToDictionary(challengerMembers);
            foreach (var baselineMember in baselineMembers)
            {
                if (baselineMember is Type)
                {
                    foreach (var memberInfo in Compare(baselineMember, challengerMemberDictionary))
                    {
                        yield return memberInfo;
                    }
                }
                else if (baselineMember is PropertyInfo)
                {
                    var baselineProperty = (PropertyInfo) baselineMember;
                    ProcessPropertyInfo(baselineProperty, challengerMemberDictionary);
                }
                else if (baselineMember is EventInfo)
                {
                    var baselineEvent = (EventInfo) baselineMember;
                    ProcessEventInfo(baselineEvent, challengerMemberDictionary);
                }
                else
                {
                    MemberInfo foundChallenger = null;
                    foreach (var challengerMember in challengerMemberDictionary.Values)
                    {
                        if (AreEqual(baselineMember, challengerMember))
                        {
                            foundChallenger = challengerMember;
                            break;
                        }
                    }
                    if (foundChallenger == null)
                    {
                        yield return baselineMember;
                    }
                    else
                    {
                        challengerMemberDictionary.Remove(foundChallenger);
                    }
                }
            }
            foreach (var challengerMember in challengerMemberDictionary.Values)
            {
                if (challengerMember is PropertyInfo)
                {
                    // skip
                }
                else if (challengerMember is EventInfo)
                {
                    // skip
                }
                else
                {
                    yield return challengerMember;                    
                }
            }
        }

        internal static void ProcessEventInfo(EventInfo baselineEvent, Dictionary<MemberInfo, MemberInfo> challengerMemberDictionary)
        {
            EventInfo foundChallenger = null;
            foreach (var challengerMember in challengerMemberDictionary.Values)
            {
                if (challengerMember is EventInfo)
                {
                    var challengerEvent = (EventInfo) challengerMember;
                    if (baselineEvent.Name == challengerEvent.Name)
                    {
                        foundChallenger = challengerEvent;
                        break;
                    }
                }
            }
            if (foundChallenger == null)
            {
                // the add_{Name} and remove_{Name} methods will already show up attached to the type, do not yield
            }
            else
            {
                challengerMemberDictionary.Remove(foundChallenger);
            }
        }

        internal static void ProcessPropertyInfo
            (PropertyInfo baselineProperty, Dictionary<MemberInfo, MemberInfo> challengerMemberDictionary)
        {
            PropertyInfo foundChallenger = null;
            foreach (var challengerMember in challengerMemberDictionary.Values)
            {
                if (challengerMember is PropertyInfo)
                {
                    var challengerProperty = (PropertyInfo) challengerMember;
                    if (baselineProperty.Name == challengerProperty.Name)
                    {
                        foundChallenger = challengerProperty;
                        break;
                    }
                }
            }
            if (foundChallenger == null)
            {
                // the accessors will already show up attached to the type, do not yield
            }
            else
            {
                challengerMemberDictionary.Remove(foundChallenger);
            }
        }

        internal static bool IsVisible(MemberInfo memberInfo)
        {
            if (memberInfo is MethodBase)
            {
                return IsVisible((MethodBase) memberInfo);
            }
            if (memberInfo is FieldInfo)
            {
                return IsVisible((FieldInfo) memberInfo);
            }
            if (memberInfo is EventInfo)
            {
                return true;
            }
            if (memberInfo is Type)
            {
                return IsVisible((Type) memberInfo);
            }
            if (memberInfo is PropertyInfo)
            {
                // we aren't processing them anyway; we instead focus on associated accessor methods
                return false;
            }
            throw new ArgumentException("Parameter 'memberInfo' is an unsupported type.", "memberInfo");
        }

        internal static bool IsVisible(Type type)
        {
            return type.IsNested && (type.IsNestedPublic || type.IsNestedFamily || type.IsNestedFamORAssem);
        }

        internal static bool IsVisible(MethodBase methodBase)
        {
            return IsProtected(methodBase) || IsPublic(methodBase);
        }

        internal static bool IsProtected(MethodBase methodBase)
        {
            return methodBase.IsFamily || methodBase.IsFamilyOrAssembly;
        }

        internal static bool IsPublic(MethodBase methodBase)
        {
            return methodBase.IsPublic;
        }

        internal static bool IsVisible(FieldInfo fieldInfo)
        {
            return fieldInfo.IsPublic || fieldInfo.IsFamily || fieldInfo.IsFamilyOrAssembly;
        }

        internal static string Describe(MethodBase methodBase)
        {
            return "{0} {1}".FormatInvariant(Describe(methodBase.ReflectedType), methodBase.ToString());
        }

        internal static string Describe(Type type)
        {
            return type.FullName;
        }

        internal static string Describe(EventInfo eventInfo)
        {
            return "{0} {1}".FormatInvariant(Describe(eventInfo.ReflectedType), eventInfo.ToString());
        }

        internal static string Describe(FieldInfo fieldInfo)
        {
            return "{0} {1}".FormatInvariant(Describe(fieldInfo.ReflectedType), fieldInfo.ToString());
        }

        internal static string Describe(MemberInfo memberInfo)
        {
            if (memberInfo is MethodBase)
            {
                return Describe((MethodBase) memberInfo);
            }
            if (memberInfo is EventInfo)
            {
                return Describe((EventInfo) memberInfo);
            }
            if (memberInfo is FieldInfo)
            {
                return Describe((FieldInfo) memberInfo);
            }
            if (memberInfo is Type)
            {
                return Describe((Type) memberInfo);
            }
            throw new ArgumentException("Unsupported MemberInfo instance", "memberInfo");
        }
    }
}
