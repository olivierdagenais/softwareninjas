using System;
using System.Collections.Generic;

using NUnit.Framework;
using Parent = SoftwareNinjas.Core;

namespace SoftwareNinjas.Core.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.TreeExtensions"/>.
    /// </summary>
    [TestFixture]
    public class TreeExtensions
    {
        private class Node : IEnumerable<Node>
        {
            private readonly IList<Node> _children = new List<Node>();
            public IEnumerable<Node> Children { get { return _children; } }

            private readonly string _name;
            public string Name { get { return _name; } }

            public Node(string name)
            {
                _name = name;
            }

            public Node Add(string nodeName)
            {
                return Add(new Node(nodeName));
            }

            public Node Add(Node node)
            {
                _children.Add(node);
                return node;
            }

            #region IEnumerable<Node> Members

            public IEnumerator<Node> GetEnumerator()
            {
                return Children.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return Children.GetEnumerator();
            }

            #endregion
        }

        /// <summary>
        /// Tests the <see cref="Core.TreeExtensions.PreOrder{T}(T,Func{T,IEnumerable{T}})"/> method with
        /// the special case of a single node and no children.
        /// </summary>
        [Test]
        public void PreOrder_OnlyOneNode()
        {
            var root = new Node("root");

            var e = Parent.TreeExtensions.PreOrder(root, n => n.Children).GetEnumerator();

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(root, e.Current);

            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        /// Tests the <see cref="Core.TreeExtensions.PreOrder{T}(T,Func{T,IEnumerable{T}})"/> method with
        /// the scenario of a single level of nodes.
        /// </summary>
        [Test]
        public void PreOrder_OneLevel()
        {
            var felinae = new Node("Felinae");
            var felis = felinae.Add("Felis");
            var lynx = felinae.Add("Lynx");
            var e = Parent.TreeExtensions.PreOrder(felinae, n => n.Children).GetEnumerator();

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(felinae, e.Current);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(felis, e.Current);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(lynx, e.Current);

            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        /// Tests the <see cref="Core.TreeExtensions.PreOrder{T}(T,Func{T,IEnumerable{T}})"/> method with
        /// what a high-level parse tree of a code file might look like.
        /// </summary>
        [Test]
        public void PreOrder_MultipleLevels()
        {
            var compilationUnit = new Node("TreeExtensions.cs")
            {
                new Node("namespace SoftwareNinjas.Core.Test")
                {
                    new Node("public class TreeExtensions")
                    {
                        new Node("private class Node : IEnumerable<Node>")
                        {
                            new Node("public IEnumerable<Node> Children { get { return _children; } }"),
                            new Node("public string Name { get { return _name; } }"),
                            new Node("public Node(string name)"),
                            new Node("public Node Add(string nodeName)"),
                            new Node("public Node Add(Node node)"),
                            new Node("#region IEnumerable<Node> Members")
                            {
                                new Node("public IEnumerator<Node> GetEnumerator()"),
                            },
                            new Node("#region IEnumerable Members")
                            {
                                new Node("System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()"),
                            }
                        },
                        new Node("public void PreOrder_OnlyOneNode()"),
                        new Node("public void PreOrder_OneLevel()"),
                        new Node("public void PreOrder_MultipleLevels()"),
                    },
                },
            };
            var e = Parent.TreeExtensions.PreOrder(compilationUnit, n => n.Children).GetEnumerator();

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(compilationUnit, e.Current);
            Assert.AreEqual("TreeExtensions.cs", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("namespace SoftwareNinjas.Core.Test", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("public class TreeExtensions", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("private class Node : IEnumerable<Node>", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("public IEnumerable<Node> Children { get { return _children; } }", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("public string Name { get { return _name; } }", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("public Node(string name)", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("public Node Add(string nodeName)", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("public Node Add(Node node)", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("#region IEnumerable<Node> Members", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("public IEnumerator<Node> GetEnumerator()", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("#region IEnumerable Members", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("public void PreOrder_OnlyOneNode()", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("public void PreOrder_OneLevel()", e.Current.Name);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("public void PreOrder_MultipleLevels()", e.Current.Name);

            Assert.IsFalse(e.MoveNext());
        }
    }
}
