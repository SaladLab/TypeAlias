using System;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;
using TypeAlias;

namespace UseCaseForSerialization
{
    [TypeAlias, ProtoContract]
    internal class AliceClass
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(2)] public int Value;

        public override string ToString()
        {
            return String.Format("AliceClass {{ Name: {0}, Value: {1} }}", Name, Value);
        }
    }

    internal class Program
    {
        private static TypeModel TypeModel;
        private static TypeAliasTable TypeAliasTable;

        private static void Main(string[] args)
        {
            TypeModel = RuntimeTypeModel.Create();
            TypeAliasTable = new TypeAliasTable();

            // Serialize
            var writeStream = new MemoryStream();
            var obj1 = new AliceClass { Name = "Wonderland", Value = 100 };
            Console.WriteLine("Before Serialize: {0}", obj1);
            Serialize(writeStream, obj1);
            var data = writeStream.ToArray();
            Console.WriteLine("DataSize: {0}", data.Length);

            // Deserialize
            var readStream = new MemoryStream(data);
            var obj2 = Deserialize(readStream, (int)readStream.Length);
            Console.WriteLine("After Serialize: {0}", obj2);
        }

        private static void Serialize(Stream stream, object obj)
        {
            // Write alias of type
            var alias = TypeAliasTable.GetAlias(obj.GetType());
            var aliasBuf = BitConverter.GetBytes(alias);
            stream.Write(aliasBuf, 0, aliasBuf.Length);

            // Write proto-buf stream of object
            TypeModel.Serialize(stream, obj);
        }

        private static object Deserialize(Stream stream, int length)
        {
            // Read alias of type
            var aliasBuf = new byte[4];
            stream.Read(aliasBuf, 0, aliasBuf.Length);
            var alias = BitConverter.ToInt32(aliasBuf, 0);
            var type = TypeAliasTable.GetType(alias);

            // Read proto-buf stream of object
            return TypeModel.Deserialize(stream, null, type, length - aliasBuf.Length);
        }
    }
}
