<a href="https://www.nuget.org/packages/am1goo.System.Text.Json.Combiner" rel="nofollow">
<img alt="NuGet Version" src="https://img.shields.io/nuget/v/am1goo.System.Text.Json.Combiner">
</a>

<a href="https://www.nuget.org/packages/am1goo.System.Text.Json.Combiner" rel="nofollow">
<img alt="NuGet Downloads" src="https://img.shields.io/nuget/dt/am1goo.System.Text.Json.Combiner">
</a>

# System.Text.Json.Combiner

#### What is this:
You can deserialize a lot of inlined `JSON` files in one `JSON` file from different sources (local or remote files), like that:

#### Supported schemes:
- `file://` to load file from system
- `http://` and `https://` to load files from web servers

#### How it can be used:

Use `JsonCombiner` instead of `JsonSerializer` to deserialize JSON file from file system and inherit each inlined class or struct from `IJsonCombine` interface
```csharp
public TestObject LoadFromFile(string relativePath)
{
  string path = Path.Combine(Environment.CurrentDirectory, relativePath);
  return JsonCombiner.Deserialize<TestObject>(path, options);
}
```

Also you can able to make your own json `IJsonLoader` variant and register it via `JsonCombiner.RegisterLoader` method.
```csharp
private void Main(string[] args)
{
  var myLoader = new MyJsonLoader();
  JsonCombiner.RegisterLoader("myhttp", myLoader);
}
```

#### Example:
*Root file:*
```json
{
  "param1": "param1",
  "param2": 2,
  "param3": 3.3,
  "inner1": "file://inline/inner_object_1.json",
  "inner2": "inline/inner_object_2.json",
  "inner3": "http://any.public.host.xyz/json.file.json"
}
```

*File `inline/inner_object_1.json`:*
```json
{
  "arg1": "arg1",
  "arg2": 44,
  "arg3": 55.55
}
```

*File `inline/inner_object_2.json`:*
```json
{
  "arg1": "arg2",
  "arg2": 66,
  "arg3": 77.77
}
```
