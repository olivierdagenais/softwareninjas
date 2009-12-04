using System;
using System.Collections.Generic;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Extension methods for tree-based structures.
    /// </summary>
    public static class TreeExtensions
    {
        /// <summary>
        /// Traverses a tree of objects by visiting the <paramref name="startingPoint"/> and then its children,
        /// recursively.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the <paramref name="startingPoint"/> as well as any children encountered along the way.
        /// </typeparam>
        /// 
        /// <param name="startingPoint">
        /// The node from which the traversal will begin.
        /// </param>
        /// 
        /// <param name="children">
        /// A method that can obtain the children of instances of <typeparamref name="T"/>.
        /// </param>
        /// 
        /// <returns>
        /// A sequence of the nodes in the tree.
        /// </returns>
        public static IEnumerable<T> PreOrder<T>(this T startingPoint, Func<T, IEnumerable<T>> children)
        {
            yield return startingPoint;
            foreach (var child in children(startingPoint))
            {
                var preOrderedChildren = PreOrder(child, children);
                foreach (var preOrderedChild in preOrderedChildren)
                {
                    yield return preOrderedChild;
                }
            }
        }
    }
}
