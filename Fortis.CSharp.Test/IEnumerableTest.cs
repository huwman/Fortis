using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortis.CSharp.Tests
{
    [TestClass]
    public class IEnumerableTest
    {
        [TestMethod]
        public void Pick_EmptySource()
        {
            var source = new List<Option<int>>
            {
            };

            var result = source.Pick(Func.Identity);

            Assert.IsFalse(result.Exists());
        }

        [TestMethod]
        public void Pick_NoMatch()
        {
            var source = new List<Option<int>>
            {
                Option.None<int>(),
            };

            var result = source.Pick(Func.Identity);

            Assert.IsFalse(result.Exists());
        }

        [TestMethod]
        public void Pick_Match()
        {
            var source = new List<Option<int>>
            {
                Option.None<int>(),
                Option.OfValue(0),
            };

            var result = source.Pick(Func.Identity);

            Assert.IsTrue(result.Exists());
        }

        [TestMethod]
        public void Choose_EmptySource()
        {
            var source = new List<Option<int>>
            {
            };

            var result = source.Choose(Func.Identity);

            Assert.AreEqual(result.Count(), 0);
        }

        [TestMethod]
        public void Choose_NoMatches()
        {
            var source = new List<Option<int>>
            {
                Option.None<int>(),
            };

            var result = source.Choose(Func.Identity);

            Assert.AreEqual(result.Count(), 0);
        }

        [TestMethod]
        public void Choose_TwoMatches()
        {
            var source = new List<Option<int>>
            {
                Option.None<int>(),
                Option.OfValue(0),
                Option.None<int>(),
                Option.OfValue(1),
            };

            var result = source.Choose(Func.Identity);

            Assert.AreEqual(result.Count(), 2);
        }
    }
}
