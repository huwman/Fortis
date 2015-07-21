using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Contracts;
using System.Collections.Generic;

namespace Fortis.CSharp.Tests
{
    [TestClass]
    public class OptionTest
    {
        [TestMethod]
        public void Some_EqualityTest()
        {
            var p = Option.OfValue(55);
            var q = Option.OfValue(55);

            Assert.AreEqual(p, q);
            Assert.IsTrue(p == q);
        }

        [TestMethod]
        public void None_EqualityTest()
        {
            var p = Option.None<int>();
            var q = Option.None<int>();

            Assert.AreEqual(p, q);
            Assert.IsTrue(p == q);
        }

        [TestMethod]
        public void SomeAndNone_EqualityTest()
        {
            var p = Option.None<int>();
            var q = Option.OfValue(0);

            Assert.AreNotEqual(p, q);
            Assert.IsTrue(p != q);
        }

        [TestMethod]
        public void MapIntToString()
        {
            var p = Option.OfValue(0);
            var q = p.Map(v => v.ToString());

            Assert.IsTrue(q.Exists());
            Assert.AreEqual(q, Option.OfValue("0"));
        }

        [TestMethod]
        public void None_AndThen()
        {
            var p = Option.None<string>();
            var q = p.AndThen<string, int>((v, col) => Int32.TryParse(v, out col.Value));

            Assert.IsFalse(q.Exists());
        }

        [TestMethod]
        public void Some_AndThen()
        {
            var p = Option.OfValue("55");
            var q = p.AndThen<string, int>((v, collector) => Int32.TryParse(v, out collector.Value));

            Assert.IsTrue(q.Exists());
        }

        [TestMethod]
        public void None_WithDefault()
        {
            var p = Option.None<string>();
            var q = p.WithDefault("");

            Assert.AreEqual(q, "");
        }

        [TestMethod]
        public void Some_WithDefault()
        {
            var p = Option.OfValue("Test");
            var q = p.WithDefault("");

            Assert.AreEqual(q, "Test");
        }

        [TestMethod]
        public void None_DictionaryRetrieval()
        {
            var dic = new Dictionary<Option<int>, string>
            {
                { Option.None<int>(), "None" },
            };

            var value = dic.TryGetValue(Option.None<int>());
            Assert.AreEqual(value, Option.OfValue("None"));
        }

        [TestMethod]
        public void Some_DictionaryRetrieval()
        {
            var dic = new Dictionary<Option<int>, string>
            {
                { Option.OfValue(55), "55" },
            };

            var value = dic.TryGetValue(Option.OfValue(55));
            Assert.AreEqual(value, Option.OfValue("55"));
        }
    }
}
