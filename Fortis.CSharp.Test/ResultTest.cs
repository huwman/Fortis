using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Contracts;

namespace Fortis.CSharp.Tests
{
    [TestClass]
    public class ResultTest
    {
        [TestMethod]
        public void Error_EqualityTest()
        {
            var p = Result.Error<string, int>("Error");
            var q = Result.Error<string, int>("Error");

            Assert.AreEqual(p, q);
            Assert.IsTrue(p == q);
        }

        [TestMethod]
        public void Success_EqualityTest()
        {
            var p = Result.Success<string, int>(5);
            var q = Result.Success<string, int>(5);

            Assert.AreEqual(p, q);
            Assert.IsTrue(p == q);
        }

        [TestMethod]
        public void SuccessAndError_EqualityTest()
        {
            var p = Result.Success<string, int>(5);
            var q = Result.Error<string, int>("5");

            Assert.AreNotEqual(p, q);
            Assert.IsTrue(p != q);
        }

        [TestMethod]
        public void MapIntToString()
        {
            var p = Result.Success<string, int>(5);
            var q = p.Map(v => v.ToString());

            Assert.IsTrue(q.IsSuccess());
            Assert.AreEqual(q, Result.Success<string, string>("5"));
        }

        [TestMethod]
        public void Error_FormatError()
        {
            var p = Result.Error<int, int>(-1);
            var q = p.FormatError(e => String.Format("Error code {0}", e));

            Assert.IsTrue(q.IsError());
        }

        [TestMethod]
        public void Success_AndThen()
        {
            var p = Result.Success<string, int>(5);
            var q = p.AndThen(v => Result.Success<string, int>(v));

            Assert.IsTrue(q.IsSuccess());
        }
    }
}
