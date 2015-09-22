using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace TypeAlias.Tests.Net20
{
    internal class FooAttribute : Attribute
    {
    }

    [Foo]
    internal class BarClass
    {
    }

    [Foo]
    internal class CarClass
    {
    }

    public class TestTypeUtility : AssertionHelper
    {
        [Test]
        public static void Test_GetType_Should_Handle_AssemblyQualifiedName()
        {
            var type = TypeUtility.GetType(typeof(object).AssemblyQualifiedName);
            Assert.AreEqual(typeof(object), type);
        }

        [Test]
        public static void Test_GetType_Should_Handle_FullName()
        {
            var type = TypeUtility.GetType(typeof(object).FullName);
            Assert.AreEqual(typeof(object), type);
        }

        [Test]
        public static void Test_GetTypes()
        {
            var hit = false;
            foreach (var type in TypeUtility.GetTypes(typeof(object).Assembly))
            {
                if (type == typeof(object))
                    hit = true;
            }
            Assert.That(hit);
        }

        [Test]
        public static void Test_GetTypesContainingAttribute()
        {
            var result = new List<TypeUtility.TypeAndAttribute<FooAttribute>>(
                TypeUtility.GetTypesContainingAttribute<FooAttribute>(typeof(TestTypeUtility).Assembly));
            Assert.AreEqual(2, result.Count); // BarClass, CarClass
        }
    }
}
