using System;
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
        private const string TextileBlockModifier = "Textile.BlockModifier";
        private const string TextileFormatterStateAttribute = "Textile.FormatterStateAttribute";
        private const string TextileBlocksBoldPhraseBlockModifier = "Textile.Blocks.BoldPhraseBlockModifier";
        private const string TextileBlocksCapitalsBlockModifier = "Textile.Blocks.CapitalsBlockModifier";
        private const string TextileBlocksHyperLinkBlockModifier = "Textile.Blocks.HyperLinkBlockModifier";
        private const string TextileBlocksNoTextileBlockModifier = "Textile.Blocks.NoTextileBlockModifier";

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
            // arrange
            var challengerBlockModifier = _visibility.GetType(TextileBlockModifier);
            var challengerTbcbm = _visibility.GetType(TextileBlocksCapitalsBlockModifier);
            var challengerTbhlbm = _visibility.GetType(TextileBlocksHyperLinkBlockModifier);
            var challengerTfsa = _visibility.GetType(TextileFormatterStateAttribute);
            var challengerTbntbm = _visibility.GetType(TextileBlocksNoTextileBlockModifier);
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(_baseline, _visibility);
            // assert
            var expected = new MemberInfo[]
            {
                GetBaselineConclude("BlockModifier"),
                challengerBlockModifier.GetConstructor(Type.EmptyTypes),
                GetBaselineConclude("Blocks.PhraseBlockModifier"),
                GetBaselineConclude("Blocks.DeletedPhraseBlockModifier"),
                GetBaselineConclude("Blocks.BoldPhraseBlockModifier"),
                GetBaselineConclude("Blocks.InsertedPhraseBlockModifier"),
                GetBaselineConclude("Blocks.GlyphBlockModifier"),
                GetBaselineConclude("Blocks.EmphasisPhraseBlockModifier"),
                GetBaselineConclude("Blocks.PreBlockModifier"),
                GetBaselineConclude("Blocks.HyperLinkBlockModifier"),
                challengerTbhlbm.GetField("m_rel"),
                challengerTfsa.GetMethod("set_Pattern", new[] { typeof(String) }),
                GetBaselineConclude("Blocks.SubScriptPhraseBlockModifier"),
                GetBaselineConclude("Blocks.SuperScriptPhraseBlockModifier"),
                GetBaselineConclude("Blocks.ItalicPhraseBlockModifier"),
                GetBaselineConclude("Blocks.NoTextileBlockModifier"),
                challengerTbntbm.GetMethod("add_Stuff", new[] { typeof(EventHandler) }),
                challengerTbntbm.GetMethod("remove_Stuff", new[] { typeof(EventHandler) }),
                GetBaselineConclude("Blocks.FootNoteReferenceBlockModifier"),
                GetBaselineConclude("Blocks.StrongPhraseBlockModifier"),
                GetBaselineConclude("Blocks.SpanPhraseBlockModifier"),
                GetBaselineConclude("Blocks.ImageBlockModifier"),
                GetBaselineConclude("Blocks.CitePhraseBlockModifier"),
                GetBaselineConclude("Blocks.CapitalsBlockModifier"),
                challengerTbcbm.GetMethod("CapitalsFormatMatchEvaluator"),
                GetBaselineConclude("Blocks.CodeBlockModifier"),
            };
            CoreTest.EnumerableExtensions.EnumerateSame(expected, actual);
        }

        private MethodInfo GetBaselineConclude(string className)
        {
            var name = "Textile." + className;
            var type = _baseline.GetType(name);
            Assert.IsNotNull(type, "Unable to find type: {0}", name);
            return type.GetMethod("Conclude");
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
            var baseline = GetConvertToStringOverload(typeof(bool));
            var challenger = GetConvertToStringOverload(typeof(byte));
            // act
            var actual = Parent.PublicInterfaceComparerTask.AreEqual(baseline, challenger);
            // assert
            Assert.AreEqual(false, actual);
        }

        // TODO: check two methods with the same signature, but one is static, the other is instance

        private static MethodInfo GetConvertToStringOverload(Type type)
        {
            var convertType = typeof(Convert);
            var result = convertType.GetMethod("ToString", DefaultBindingFlags | BindingFlags.ExactBinding,
                null, new[] { type }, null);
            return result;
        }

        private static MethodInfo GetModifyLineMethod(Assembly assembly, string className)
        {
            var blockModifierType = assembly.GetType(className);
            var formalParameterTypes = new[] { typeof(String) };
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
            var baselineConclude = baseline.GetMethod("Conclude");
            var challengerCapitalsFormatMatchEvaluator = challenger.GetMethod("CapitalsFormatMatchEvaluator");
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            CoreTest.EnumerableExtensions.EnumerateSame
                (new MemberInfo[] { baselineConclude, challengerCapitalsFormatMatchEvaluator }, actual);
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
            var baselineConclude = baseline.GetMethod("Conclude");
            var challengerMRel = challenger.GetField("m_rel");
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            CoreTest.EnumerableExtensions.EnumerateSame
                (new MemberInfo[] { baselineConclude, challengerMRel }, actual);
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
            var baselineConclude = baseline.GetMethod("Conclude");
            var challengerMRel = challenger.GetField("m_rel");
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(challenger, baseline);
            // assert
            CoreTest.EnumerableExtensions.EnumerateSame
                (new MemberInfo[] { challengerMRel, baselineConclude }, actual);
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
            CoreTest.EnumerableExtensions.EnumerateSame
                (EmptyMemberInfoSequence, actual);
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
            var challengerPatternSet = challenger.GetMethod("set_Pattern", new[] {typeof (String)});
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            CoreTest.EnumerableExtensions.EnumerateSame
                (new MemberInfo[] { challengerPatternSet }, actual);
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
            var baselineConclude = baseline.GetMethod("Conclude");
            var challengerAddStuff = challenger.GetMethod("add_Stuff", new[] {typeof (EventHandler)});
            var challengerRemoveStuff = challenger.GetMethod("remove_Stuff", new[] {typeof (EventHandler)});
            // act
            var actual = Parent.PublicInterfaceComparerTask.Compare(baseline, challenger);
            // assert
            CoreTest.EnumerableExtensions.EnumerateSame
                (new MemberInfo[] { baselineConclude, challengerAddStuff, challengerRemoveStuff }, actual);
        }
    }
}
