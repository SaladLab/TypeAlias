using NUnit.Framework;

namespace TypeAlias.Tests.Net20
{
    [TypeAlias(1)]
    internal class AliceClass
    {
    }

    [TypeAlias(2)]
    internal class BobClass
    {
    }

    [TypeAlias]
    internal class EveClass
    {
    }

    [TypeAlias(100)]
    internal struct TestStruct
    {
    }

    [TypeAlias(101)]
    internal interface ITestInterface
    {
    }

    public class TestTypeAliasTable : AssertionHelper
    {
        [Test]
        public static void Test_GetType_Hit()
        {
            var table = new TypeAliasTable();
            Assert.AreEqual(typeof(AliceClass), table.GetType(1));
            Assert.AreEqual(typeof(BobClass), table.GetType(2));
            Assert.AreEqual(typeof(EveClass), table.GetType(1747347853));
            Assert.AreEqual(typeof(TestStruct), table.GetType(100));
            Assert.AreEqual(typeof(ITestInterface), table.GetType(101));
        }

        [Test]
        public static void Test_GetType_Miss()
        {
            var table = new TypeAliasTable();
            Assert.AreEqual(null, table.GetType(10));
        }

        [Test]
        public static void Test_GetAlias_Hit()
        {
            var table = new TypeAliasTable();
            Assert.AreEqual(1, table.GetAlias(typeof(AliceClass)));
            Assert.AreEqual(2, table.GetAlias(typeof(BobClass)));
            Assert.AreEqual(1747347853, table.GetAlias(typeof(EveClass)));
        }

        [Test]
        public static void Test_GetAlias_Miss()
        {
            var table = new TypeAliasTable();
            Assert.AreEqual(0, table.GetAlias(typeof(TestTypeAliasTable)));
        }

        [Test]
        public static void Test_AddTypeAlias()
        {
            var table = new TypeAliasTable();
            table.AddTypeAlias(typeof(TestTypeAliasTable), 100);
            Assert.AreEqual(typeof(TestTypeAliasTable), table.GetType(100));
            Assert.AreEqual(100, table.GetAlias(typeof(TestTypeAliasTable)));
        }

        [Test]
        public static void Test_AddTypeAlias_UsingCalculatedAlias()
        {
            var table = new TypeAliasTable();
            table.AddTypeAlias(typeof(TestTypeAliasTable));
            Assert.AreNotEqual(0, table.GetAlias(typeof(TestTypeAliasTable)));
        }
    }
}
