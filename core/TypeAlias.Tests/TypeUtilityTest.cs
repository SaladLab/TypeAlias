using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TypeAlias.Tests
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

    public class TypeUtilityTest : AssertionHelper
    {
        [Test]
        public void Test_GetType_Should_Handle_AssemblyQualifiedName()
        {
            var type = TypeUtility.GetType(typeof(object).AssemblyQualifiedName);
            Assert.AreEqual(typeof(object), type);
        }

        [Test]
        public void Test_GetType_Should_Handle_FullName()
        {
            var type = TypeUtility.GetType(typeof(object).FullName);
            Assert.AreEqual(typeof(object), type);
        }

        [Test]
        public void Test_GetTypes()
        {
            var hit = false;
            foreach (var type in TypeUtility.GetTypes(TypeUtility.GetContainingAssembly(typeof(object))))
            {
                if (type == typeof(object))
                    hit = true;
            }
            Assert.That(hit);
        }

        [Test]
        public void Test_GetTypesContainingAttribute()
        {
            var result = new List<TypeUtility.TypeAndAttribute<FooAttribute>>(
                TypeUtility.GetTypesContainingAttribute<FooAttribute>(
                    TypeUtility.GetContainingAssembly(typeof(TypeUtilityTest))));
            Assert.AreEqual(2, result.Count); // BarClass, CarClass
        }
    }
}
