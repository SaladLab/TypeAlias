# TypeAlias

When writing .NET type info into stream, most of time Type.AssemblyQualifiedName is used.
It's good though sometimes it doesn't fit.

#### AssemblyQualifiedName is lengthy.
 
Common AssemblyQualifiedName looks like following:

> TopNamespace.SubNameSpace.ContainingClass+NestedClass, MyAssembly, Version=1.3.0.0, Culture=neutral, PublicKeyToken=b17a5c561934e089``

When we need to squeeze every drop for saving network bandwidth or storage space, lengthy name will be first target to be optimized.

#### Deal with same type in different assemblies.

AssemblyQualifiedName is good at making clear from ambiguous situation that there're types that has same name in different assemblies.
But it could be intended. For example, there're two clients that will communicate to each other. But their versions are different. (only one of them were updated)
If one of clients send a message with type-info, the other client should understand a message. In this time, strict assembly version checking need to be suppressed.

## Where can I get it?

```
PM> Install-Package TypeAlias
```

## How to use

### Set a alias to a type

Before using, types need to be aliased in following ways.

#### Set explicit alias by attribute

```
[TypeAlias(1)]
class AliceClass // alias of AliasClass = 1
{
}
```

#### Set auto-generated alias by attribute

If alias left empty, auto-generated alias will be used.
To keep this value permanent, library uses always djb2 hashing algorithm and 
this won't be changed to keep backward compatibility.

```
[TypeAlias] // alias of AliasClass = TODO
class BobClass
{
}
```

Auto-generated alias will be between 0x100 and 0x7FFFFFFF.

#### Set explicit alias by call

Using AddTypeAlias method can be used to set alias to type.
This is the only way if you cannot modify source containing target types.

```
var table = new TypeAliasTable();
table.AddTypeAlias(typeof(String), 10);
```

### Lookup table

After setting aliases to types, we can lookup the alias from type, and vise versa.

#### Get an alias from a type

```
[TypeAlias(1)] class AliceClass {}
[TypeAlias(2)] class BobClass {}

var table = new TypeAliasTable();
var alias1 = table.GetType(typeof(AliceClass)); // alias1 = 1
var alias2 = table.GetType(typeof(BobClass));   // alias2 = 2
```

#### Get a type from an alias

```
[TypeAlias(1)] class AliceClass {}
[TypeAlias(2)] class BobClass {}

var table = new TypeAliasTable();
var type1 = table.GetType(1); // type = typeof(AliceClass)
var type2 = table.GetType(2); // type = typeof(BobClass)
```

## Use-Case for serialization

Define a type that will be serialized and serialized.
In this sample, protobuf-net is used for serializing body of object.

```
[TypeAlias, ProtoContract]
class AliceClass
{
    [ProtoMember(1)] public string Name;
    [ProtoMember(2)] public int Value;

    public override string ToString()
    {
        return String.Format("AliceClass {{ Name: {0}, Value: {1} }}", Name, Value);
    }
}
```

Define Serialize and Deserialize method. It will use type alias for saving type info.

```
static TypeModel TypeModel;
static TypeAliasTable TypeAliasTable;

static void Serialize(Stream stream, object obj)
{
    // Write alias of type
    var alias = TypeAliasTable.GetAlias(obj.GetType());
    var aliasBuf = BitConverter.GetBytes(alias);
    stream.Write(aliasBuf, 0, aliasBuf.Length);

    // Write proto-buf stream of object
    TypeModel.Serialize(stream, obj);
}

static object Deserialize(Stream stream, int length)
{
    // Read alias of type
    var aliasBuf = new byte[4];
    stream.Read(aliasBuf, 0, aliasBuf.Length);
    var alias = BitConverter.ToInt32(aliasBuf, 0);
    var type = TypeAliasTable.GetType(alias);

    // Read proto-buf stream of object
    return TypeModel.Deserialize(stream, null, type, length - aliasBuf.Length);
}
```

Test drive

```
static void Test()
{
    TypeModel = RuntimeTypeModel.Create();
    TypeAliasTable = new TypeAliasTable();

    // Serialize
    var writeStream = new MemoryStream();
    var obj1 = new AliceClass { Name = "Wonderland", Value = 100 };
    Serialize(writeStream, obj1);
    var data = writeStream.ToArray();

    // Deserialize
    var readStream = new MemoryStream(data);
    var obj2 = Deserialize(readStream, (int)readStream.Length);
}
```

## Caveat

### TypeAliasTable is quite expensive to create

In constructing TypeAliasTable object, it scans whole assmeblies and finding types which have a TypeAlias attribute.
It can be a long process. So instead of creating table everytime, create table one time and keep it and reuse it when needed.

### Delayed loading assemblies need to be resolved

When table scan types, an assembly containig types you need could not be loaded yet.
It's the way that .NET runtime saves cpu and memory but it makes table have incomplete type list.
You can avoid this problem by following way

```
static void Init()
{
    // it cause .NET runtime to load an assembly containing ImportantType
    var type = typeof(ImportantType);

    // it's safe to create TypeAliasTable
    var = new TypeAliasTable();
}
```
