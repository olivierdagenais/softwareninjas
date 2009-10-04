using System;
using System.Collections.Generic;
using System.Text;

using Parent = System.Linq;
using NUnit.Framework;

namespace System.Linq.Test
{
	/// <summary>
	/// A class to test <see cref="Parent.Enumerable"/>.
	/// </summary>
	[TestFixture]
	public class Enumerable
	{
		/// <summary>
		/// Tests <see cref="Parent.Enumerable.Last{TSource}"/>
		/// with an adaptation of the example in the MSDN code documentation.
		/// </summary>
		[Test]
		public void Last_FromMsdnExample()
		{
			int[] numbers = { 9, 34, 65, 92, 87, 435, 3, 54, 
			                    83, 23, 87, 67, 12, 19 };

			int last = numbers.Last();

			Assert.AreEqual(19, last);
		}

		/// <summary>
		/// Tests <see cref="Parent.Enumerable.Last{TSource}"/>
		/// with an empty sequence.
		/// </summary>
		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void Last_EmptySequence()
		{
			int[] numbers = { };
			numbers.Last();
		}

		/// <summary>
		/// Tests <see cref="Parent.Enumerable.Last{TSource}"/>
		/// with a null sequence.
		/// </summary>
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void Last_NullSequence()
		{
			int[] numbers = null;
			numbers.Last();
		}

		/// <summary>
		/// Tests <see cref="Parent.Enumerable.Any{TSource}"/>
		/// with an adaptation of the example in the MSDN code documentation.
		/// </summary>
		[Test]
		public void Any_FromMsdnExample()
		{
			List<int> numbers = new List<int> { 1, 2 };
			bool hasElements = numbers.Any();

			Assert.IsTrue(hasElements);
		}

		/// <summary>
		/// Tests <see cref="Parent.Enumerable.Any{TSource}"/>
		/// with an empty sequence.
		/// </summary>
		[Test]
		public void Any_EmptySequence()
		{
			int[] numbers = { };
			bool hasElements = numbers.Any();

			Assert.IsFalse(hasElements);
		}

		/// <summary>
		/// Tests <see cref="Parent.Enumerable.Any{TSource}"/>
		/// with a null sequence.
		/// </summary>
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void Any_NullSequence()
		{
			int[] numbers = null;
			numbers.Any();
		}
	}
}
