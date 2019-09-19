using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Collectiblox;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class UtilTest {
        [Test]
        public void TestGetEnumerableOfType() {
            var types = Util.GetEnumerableOfType<UtilTestA>().ToArray();
            Assert.AreEqual(types.Count(), 2);
            Assert.AreSame(typeof(UtilTestA1), types[0].GetType());
            Assert.AreSame(typeof(UtilTestA2), types[1].GetType());
        }
        [Test]
        public void TestGetEnumerableOfTypeLinq() {
            var types = Util.GetEnumerableOfTypeLinq<UtilTestA>().ToArray();
            Assert.AreEqual(types.Count(), 2);
            Assert.AreSame(typeof(UtilTestA1), types[0].GetType());
            Assert.AreSame(typeof(UtilTestA2), types[1].GetType());
        }
    }
    internal class UtilTestA { }
    internal class UtilTestA1 : UtilTestA { }
    internal class UtilTestA2 : UtilTestA { }
}
