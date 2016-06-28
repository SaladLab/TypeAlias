using System.Reflection;
using NUnit.Framework;

namespace TypeAlias.Tests
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

    [TypeAlias(10)]
    internal struct TestStruct
    {
    }

    [TypeAlias(11)]
    internal interface ITestInterface
    {
    }

    public class TypeAliasTableTest : AssertionHelper
    {
        private TypeAliasTable CreateTypeAliasTable()
        {
            TypeUtility.AssemblyProvider = new FixedAssemblyProvider(
                new Assembly[]
                {
                    TypeUtility.GetContainingAssembly(typeof(object)),
                    TypeUtility.GetContainingAssembly(typeof(TypeAliasTableTest))
                });
            return new TypeAliasTable();
        }

        private Assembly GetAssembly()
        {
#if COREFX
            return GetType().GetTypeInfo().Assembly;
#else
            return GetType().Assembly;
#endif
        }
        [Test]
        public void Test_GetType_Hit()
        {
            var table = CreateTypeAliasTable();
            Assert.AreEqual(typeof(AliceClass), table.GetType(1));
            Assert.AreEqual(typeof(BobClass), table.GetType(2));
            Assert.AreEqual(typeof(EveClass), table.GetType(483686294));
            Assert.AreEqual(typeof(TestStruct), table.GetType(10));
            Assert.AreEqual(typeof(ITestInterface), table.GetType(11));
        }

        [Test]
        public void Test_GetType_Miss()
        {
            var table = CreateTypeAliasTable();
            Assert.AreEqual(null, table.GetType(9));
        }

        [Test]
        public void Test_GetAlias_Hit()
        {
            var table = CreateTypeAliasTable();
            Assert.AreEqual(1, table.GetAlias(typeof(AliceClass)));
            Assert.AreEqual(2, table.GetAlias(typeof(BobClass)));
            Assert.AreEqual(483686294, table.GetAlias(typeof(EveClass)));
        }

        [Test]
        public void Test_GetAlias_Miss()
        {
            var table = CreateTypeAliasTable();
            Assert.AreEqual(0, table.GetAlias(typeof(TypeAliasTableTest)));
        }

        [Test]
        public void Test_AddTypeAlias()
        {
            var table = CreateTypeAliasTable();
            table.AddTypeAlias(typeof(TypeAliasTableTest), 100);
            Assert.AreEqual(typeof(TypeAliasTableTest), table.GetType(100));
            Assert.AreEqual(100, table.GetAlias(typeof(TypeAliasTableTest)));
        }

        [Test]
        public void Test_AddTypeAlias_UsingCalculatedAlias()
        {
            var table = CreateTypeAliasTable();
            table.AddTypeAlias(typeof(TypeAliasTableTest));
            Assert.AreNotEqual(0, table.GetAlias(typeof(TypeAliasTableTest)));
        }

        [Test]
        public void Test_CreateTable_WithCustomAssemblies()
        {
            var table = new TypeAliasTable(new[] { GetAssembly() });
            Assert.AreEqual(typeof(AliceClass), table.GetType(1));
        }

        [Test]
        public void Test_CreateTable_WithFilter()
        {
            var table = new TypeAliasTable(new[] { GetAssembly() },
                                           t => t.Name.StartsWith("Alice") == false);
            Assert.AreEqual(null, table.GetType(1));
            Assert.AreEqual(typeof(BobClass), table.GetType(2));
        }

        [Test]
        public void Test_CreateTable_WithCustomResolveAutoAlias()
        {
            var table = new TypeAliasTable(new[] { GetAssembly() },
                                           null,
                                           t => t.Name.Length);
            Assert.AreEqual(typeof(AliceClass), table.GetType(1));
            Assert.AreEqual(typeof(EveClass), table.GetType(8));
            Assert.AreEqual(null, table.GetType(483686294));
        }
    }
}
