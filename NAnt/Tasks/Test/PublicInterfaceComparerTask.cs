using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using SoftwareNinjas.Core;
using CoreTest = SoftwareNinjas.Core.Test;
using Parent = SoftwareNinjas.NAnt.Tasks;

namespace SoftwareNinjas.NAnt.Tasks.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.PublicInterfaceComparerTask"/>.
    /// </summary>
    [TestFixture]
    public class PublicInterfaceComparerTask
    {
        private const BindingFlags DefaultBindingFlags = 
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        private const BindingFlags NonPublicInstance =
            BindingFlags.NonPublic | BindingFlags.Instance;
        private const string TextileBlockModifier = "Textile.BlockModifier";
        private const string TextileFormatterStateAttribute = "Textile.FormatterStateAttribute";
        private const string TextileBlocksCodeBlockModifier = "Textile.Blocks.CodeBlockModifier";
        private const string TextileBlocksPhraseBlockModifier = "Textile.Blocks.PhraseBlockModifier";
        private const string TextileBlocksBoldPhraseBlockModifier = "Textile.Blocks.BoldPhraseBlockModifier";
        private const string TextileBlocksCapitalsBlockModifier = "Textile.Blocks.CapitalsBlockModifier";
        private const string TextileBlocksHyperLinkBlockModifier = "Textile.Blocks.HyperLinkBlockModifier";
        private const string TextileBlocksNoTextileBlockModifier = "Textile.Blocks.NoTextileBlockModifier";
        private const string TextileFormatterState = "Textile.FormatterState";

        private static readonly MemberInfo[] EmptyMemberInfoSequence = new MemberInfo[] { };

        private readonly Assembly _baseline, _manual, _visibility;

        /// <summary>
        /// Initializes the fields used for testing.
        /// </summary>
        public PublicInterfaceComparerTask()
        {
            var basePath = Environment.CurrentDirectory.CombinePath("Tasks", "Test");
            var baselineFullPath = Path.Combine(basePath, "Textile-base.dll");
            _baseline = Assembly.LoadFile(baselineFullPath);
            var manualFullPath = Path.Combine(basePath, "Textile-manual.dll");
            _manual = Assembly.LoadFile(manualFullPath);
            var visibilityFullPath = Path.Combine(basePath, "Textile-visibility.dll");
            _visibility = Assembly.LoadFile(visibilityFullPath);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Assembly,Assembly)"/> method with
        /// two references to the same assembly.
        /// </summary>
        [Test]
        public void Compare_SameAssembly()
        {
            // arrange
            var me = Assembly.GetExecutingAssembly();
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(me, me);
            // assert
            CoreTest.EnumerableExtensions.EnumerateSame(EmptyMemberInfoSequence, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Assembly,Assembly)"/> method with
        /// two assemblies that have no differences.
        /// </summary>
        [Test]
        public void Compare_DifferentButIdenticalAssembly()
        {
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(_baseline, _manual);
            // assert
            CoreTest.EnumerableExtensions.EnumerateSame(EmptyMemberInfoSequence, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Assembly,Assembly)"/> method with
        /// two assemblies that have lots of differences.
        /// </summary>
        [Test]
        public void Compare_DifferentAssemblies()
        {
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(_baseline, _visibility).ToList();
            // assert
            Assert.AreEqual(103, actual.Count);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(MethodBase,MethodBase)"/> method with
        /// two references to the same method.
        /// </summary>
        [Test]
        public void AreEqual_SameMethodInfoInstances()
        {
            // arrange
            var baseline = this.GetType().GetMethod("AreEqual_SameMethodInfoInstances");
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baseline, baseline);
            // assert
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(MethodBase,MethodBase)"/> method with
        /// two identical methods.
        /// </summary>
        [Test]
        public void AreEqual_IdenticalMethodInfoInstances()
        {
            // arrange
            var baseline = GetModifyLineMethod(_baseline, TextileBlockModifier);
            var challenger = GetModifyLineMethod(_manual, TextileBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baseline, challenger);
            // assert
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(MethodBase,MethodBase)"/> method with
        /// methods of the same name, but from different classes.
        /// </summary>
        [Test]
        public void AreEqual_MethodInfoInstancesFromDifferentClasses()
        {
            // arrange
            var baseline = GetModifyLineMethod(_baseline, TextileBlockModifier);
            var challenger = GetModifyLineMethod(_manual, TextileBlocksBoldPhraseBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baseline, challenger);
            // assert
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(MethodBase,MethodBase)"/> method with
        /// two overloads of the same method that have a different number of parameters.
        /// </summary>
        [Test]
        public void AreEqual_MethodInfoInstancesFromOverloadsWithDifferentNumberOfParameters()
        {
            // arrange
            var baseline = GetHyperLinkBlockModifierInnerModifyLineMethod(_manual, new[] {typeof (String)});
            var challenger = GetHyperLinkBlockModifierInnerModifyLineMethod
                (_manual, new[] {typeof (String), typeof (MatchEvaluator)});
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baseline, challenger);
            // assert
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(MethodBase,MethodBase)"/> method with
        /// two overloads of the same method that have the same number of parameters.
        /// </summary>
        [Test]
        public void AreEqual_MethodInfoInstancesFromOverloadsWithDifferentTypesOfParameters()
        {
            // arrange
            var baseline = GetConvertToStringOverload(typeof (bool));
            var challenger = GetConvertToStringOverload(typeof (byte));
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baseline, challenger);
            // assert
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(MethodBase,MethodBase)"/> method with
        /// two methods that have different visibility (protected vs. internal).
        /// </summary>
        [Test]
        public void AreEqual_MethodInfoInstancesWithDifferentVisibility()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlocksPhraseBlockModifier);
            var challenger = _visibility.GetType(TextileBlocksPhraseBlockModifier);
            var baselinePmf = GetPhraseModifierFormatMethod(baseline);
            var challengerPmf = GetPhraseModifierFormatMethod(challenger);
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baselinePmf, challengerPmf);
            // assert
            Assert.AreEqual(false, actual);
        }

        private static MethodInfo GetPhraseModifierFormatMethod(Type type)
        {
            return type.GetMethod("PhraseModifierFormat", 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(MethodBase,MethodBase)"/> method with
        /// two methods that have technically the same visibility (protected vs. protected internal).
        /// </summary>
        [Test]
        public void AreEqual_MethodInfoInstancesWithProtectedInternalVisibility()
        {
            // arrange
            var baseline = _baseline.GetType(TextileFormatterState);
            var challenger = _visibility.GetType(TextileFormatterState);
            var baselineGcfs = GetCurrentFormatterStateGetter(baseline);
            var challengerGcfs = GetCurrentFormatterStateGetter(challenger);
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baselineGcfs, challengerGcfs);
            // assert
            Assert.AreEqual(true, actual);
        }

        private static MethodInfo GetCurrentFormatterStateGetter(Type type)
        {
            return type.GetMethod("get_CurrentFormatterState", NonPublicInstance);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(MethodBase,MethodBase)"/> method with
        /// two methods that have different "instance-ness" (static vs. instance).
        /// </summary>
        [Test]
        public void AreEqual_MethodInfoInstancesWithDifferentInstanceness()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlocksCodeBlockModifier);
            var challenger = _visibility.GetType(TextileBlocksCodeBlockModifier);
            var baselineCfme = GetCodeFormatMatchEvaluatorMethod(baseline);
            var challengerCfme = GetCodeFormatMatchEvaluatorMethod(challenger);
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baselineCfme, challengerCfme);
            // assert
            Assert.AreEqual(false, actual);
        }

        private static MethodInfo GetCodeFormatMatchEvaluatorMethod(Type type)
        {
            return type.GetMethod("CodeFormatMatchEvaluator",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        }

        private static MethodInfo GetConvertToStringOverload(Type type)
        {
            var convertType = typeof (Convert);
            var result = convertType.GetMethod("ToString", DefaultBindingFlags | BindingFlags.ExactBinding,
                null, new[] { type }, null);
            return result;
        }

        private static MethodInfo GetModifyLineMethod(Assembly assembly, string className)
        {
            var blockModifierType = assembly.GetType(className);
            var formalParameterTypes = new[] {typeof (String)};
            var result = blockModifierType.GetMethod("ModifyLine", 
                DefaultBindingFlags | BindingFlags.ExactBinding, null, formalParameterTypes, null);
            return result;
        }

        private static MethodInfo GetHyperLinkBlockModifierInnerModifyLineMethod
            (Assembly assembly, Type[] formalParameterTypes)
        {
            var blockModifierType = assembly.GetType(TextileBlocksHyperLinkBlockModifier);
            var result = blockModifierType.GetMethod("InnerModifyLine",
                DefaultBindingFlags | BindingFlags.NonPublic | BindingFlags.ExactBinding, 
                null, formalParameterTypes, null);
            return result;
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(Type,Type)"/> method with
        /// two references to the same type.
        /// </summary>
        [Test]
        public void AreEqual_SameTypeInstance()
        {
            // arrange
            var me = GetType();
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(me, me);
            // assert
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(Type,Type)"/> method with
        /// two identical types.
        /// </summary>
        [Test]
        public void AreEqual_IdenticalTypeInstances()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlockModifier);
            var challenger = _manual.GetType(TextileBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baseline, challenger);
            // assert
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.AreEqual(Type,Type)"/> method with
        /// two different types.
        /// </summary>
        [Test]
        public void AreEqual_DifferentTypeInstances()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlocksCapitalsBlockModifier);
            var challenger = _visibility.GetType(TextileBlocksCapitalsBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baseline, challenger);
            // assert
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Type,Type)"/> method with
        /// two references to the same type.
        /// </summary>
        [Test]
        public void Compare_SameTypeInstance()
        {
            // arrange
            var me = GetType();
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(me, me);
            // assert
            CoreTest.EnumerableExtensions.EnumerateSame(EmptyMemberInfoSequence, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Type,Type)"/> method with
        /// two identical types.
        /// </summary>
        [Test]
        public void Compare_IdenticalTypeInstances()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlockModifier);
            var challenger = _manual.GetType(TextileBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            CoreTest.EnumerableExtensions.EnumerateSame(EmptyMemberInfoSequence, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Type,Type)"/> method with
        /// types that don't have the same methods.
        /// </summary>
        [Test]
        public void Compare_TypeInstancesWithDifferentMethods()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlocksCapitalsBlockModifier);
            var challenger = _visibility.GetType(TextileBlocksCapitalsBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            var baselineConclude = baseline.GetMethod("Conclude");
            var challengerCapitalsFormatMatchEvaluator = challenger.GetMethod("CapitalsFormatMatchEvaluator");
            var expected = new MemberInfo[] { baselineConclude, challengerCapitalsFormatMatchEvaluator };
            CoreTest.EnumerableExtensions.EnumerateSame(expected, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Type,Type)"/> method with
        /// a type that had protected methods in the baseline, but now they are internal.
        /// </summary>
        [Test]
        public void Compare_TypeInstanceWithProtectedMethod()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlocksPhraseBlockModifier);
            var challenger = _visibility.GetType(TextileBlocksPhraseBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            var baselineConclude = baseline.GetMethod("Conclude");
            var baselineConstructor = baseline.GetConstructor
                (NonPublicInstance, null, Type.EmptyTypes, null);
            var threeStrings = new[] {typeof (string), typeof (string), typeof (string)};
            var baselinePhraseModifierFormat = baseline.GetMethod
                ("PhraseModifierFormat", NonPublicInstance, null, threeStrings, null);
            var expected = new MemberInfo[] {baselinePhraseModifierFormat, baselineConclude, baselineConstructor};
            CoreTest.EnumerableExtensions.EnumerateSame(expected, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Type,Type)"/> method with
        /// a type that had a static method in the baseline, but now it is an instance method.
        /// </summary>
        [Test]
        public void Compare_TypeInstanceWithStaticMethod()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlocksCodeBlockModifier);
            var challenger = _visibility.GetType(TextileBlocksCodeBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            var baselineConclude = baseline.GetMethod("Conclude");
            var baselineCfme = baseline.GetMethod("CodeFormatMatchEvaluator");
            var challengerCfme = challenger.GetMethod("CodeFormatMatchEvaluator");
            var expected = new MemberInfo[] {baselineConclude, baselineCfme, challengerCfme};
            // compare with a dictionary, because the ordering is unpredictable
            var actualDictionary = new Dictionary<MemberInfo, MemberInfo>();
            foreach (var memberInfo in actual)
            {
                actualDictionary.Add(memberInfo, memberInfo);
            }
            foreach (var memberInfo in expected)
            {
                if(actualDictionary.ContainsKey(memberInfo))
                {
                    actualDictionary.Remove(memberInfo);
                }
                else
                {
                    Assert.Fail("Actual did not contain '{0}'", memberInfo);
                }
            }
            if(actualDictionary.Count > 0)
            {
                Assert.Fail("There were leftover items in actual.");
            }
        }

        /// <summary>
        /// Test the <see cref="Parent.PublicInterfaceComparerTask.IsVisible(MethodBase)"/> method
        /// with a method marked public.
        /// </summary>
        [Test]
        public void IsVisibleMethodBase_Public()
        {
            var baseline = _baseline.GetType(TextileBlocksBoldPhraseBlockModifier);
            var baselineMethod = baseline.GetMethod("ModifyLine");
            var actual = Parent.PublicInterfaceComparerTask.IsVisible(baselineMethod);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Test the <see cref="Parent.PublicInterfaceComparerTask.IsVisible(MethodBase)"/> method
        /// with a method marked protected.
        /// </summary>
        [Test]
        public void IsVisibleMethodBase_Protected()
        {
            var baseline = _baseline.GetType(TextileBlocksPhraseBlockModifier);
            var baselineConstructor = baseline.GetConstructor
                (NonPublicInstance, null, Type.EmptyTypes, null);
            var actual = Parent.PublicInterfaceComparerTask.IsVisible(baselineConstructor);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Test the <see cref="Parent.PublicInterfaceComparerTask.IsVisible(MethodBase)"/> method
        /// with a method marked protected internal.
        /// </summary>
        [Test]
        public void IsVisibleMethodBase_ProtectedInternal()
        {
            var visibility = _visibility.GetType("Textile.States.CodeFormatterState");
            var visibilityFixEntities = visibility.GetMethod
                ("FixEntities", NonPublicInstance, null, new[] {typeof (String)}, null);
            var actual = Parent.PublicInterfaceComparerTask.IsVisible(visibilityFixEntities);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Test the <see cref="Parent.PublicInterfaceComparerTask.IsVisible(MethodBase)"/> method
        /// with a method marked internal.
        /// </summary>
        [Test]
        public void IsVisibleMethodBase_Internal()
        {
            var formatterStateType = _baseline.GetType("Textile.FormatterState");
            var baseline = _baseline.GetType("Textile.TextileFormatter");
            var oneFormatterState = new[] { formatterStateType };
            var baselineMethod = baseline.GetMethod
                ("ChangeState", NonPublicInstance, null, oneFormatterState, null);
            var actual = Parent.PublicInterfaceComparerTask.IsVisible(baselineMethod);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// Test the <see cref="Parent.PublicInterfaceComparerTask.IsVisible(MethodBase)"/> method
        /// with a method marked private.
        /// </summary>
        [Test]
        public void IsVisibleMethodBase_Private()
        {
            var baseline = _baseline.GetType("Textile.TextileFormatter");
            var oneString = new[] {typeof (string)};
            var baselineMethod = baseline.GetMethod
                ("CleanWhiteSpace", NonPublicInstance, null, oneString, null);
            var actual = Parent.PublicInterfaceComparerTask.IsVisible(baselineMethod);
            Assert.AreEqual(false, actual);
        }


        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Type,Type)"/> method with
        /// types that don't have the same fields and methods.
        /// </summary>
        [Test]
        public void Compare_TypeInstancesWithDifferentFields()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlocksHyperLinkBlockModifier);
            var challenger = _visibility.GetType(TextileBlocksHyperLinkBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            var baselineConclude = baseline.GetMethod("Conclude");
            var challengerMRel = challenger.GetField("m_rel");
            var expected = new MemberInfo[] { baselineConclude, challengerMRel };
            CoreTest.EnumerableExtensions.EnumerateSame(expected, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Type,Type)"/> method with
        /// types that don't have the same fields and methods, but the baseline and challenger are swapped.
        /// </summary>
        [Test]
        public void Compare_TypeInstancesWithDifferentFieldsReversed()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlocksHyperLinkBlockModifier);
            var challenger = _visibility.GetType(TextileBlocksHyperLinkBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(challenger, baseline);
            // assert
            var baselineConclude = baseline.GetMethod("Conclude");
            var challengerMRel = challenger.GetField("m_rel");
            var expected = new MemberInfo[] { challengerMRel, baselineConclude };
            CoreTest.EnumerableExtensions.EnumerateSame(expected, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Type,Type)"/> method with
        /// two references to the same type that contains a field.
        /// </summary>
        [Test]
        public void Compare_TypeInstancesWithFields()
        {
            // arrange
            var challenger = _visibility.GetType(TextileBlocksHyperLinkBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(challenger, challenger);
            // assert
            CoreTest.EnumerableExtensions.EnumerateSame(EmptyMemberInfoSequence, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Type,Type)"/> method with
        /// types that have differences in their properties.
        /// </summary>
        [Test]
        public void Compare_TypeInstancesWithDifferentProperties()
        {
            // arrange
            var baseline = _baseline.GetType(TextileFormatterStateAttribute);
            var challenger = _visibility.GetType(TextileFormatterStateAttribute);
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            var challengerPatternSet = challenger.GetMethod("set_Pattern", new[] {typeof (String)});
            var expected = new MemberInfo[] { challengerPatternSet };
            CoreTest.EnumerableExtensions.EnumerateSame(expected, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.PublicInterfaceComparerTask.Compare(Type,Type)"/> method with
        /// types that have different events and methods.
        /// </summary>
        [Test]
        public void Compare_TypeInstancesWithDifferentEvents()
        {
            // arrange
            var baseline = _baseline.GetType(TextileBlocksNoTextileBlockModifier);
            var challenger = _visibility.GetType(TextileBlocksNoTextileBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            var baselineConclude = baseline.GetMethod("Conclude");
            var challengerAddStuff = challenger.GetMethod("add_Stuff", new[] {typeof (EventHandler)});
            var challengerRemoveStuff = challenger.GetMethod("remove_Stuff", new[] {typeof (EventHandler)});
            var expected = new MemberInfo[] { baselineConclude, challengerAddStuff, challengerRemoveStuff };
            CoreTest.EnumerableExtensions.EnumerateSame(expected, actual);
        }
    }
}
