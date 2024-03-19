<a href="https://www.nuget.org/packages/am1goo.System.Text.Json.Combiner" rel="nofollow">
<img alt="NuGet Version" src="https://img.shields.io/nuget/v/am1goo.System.Text.Json.Combiner">
</a>

# System.Text.Json.Combiner

#### How it works
You can deserialize a lot of inlined `JSON` files in one `JSON` file, like that:

*Root file:*
```json
{
  "param1": "param1",
  "param2": 2,
  "param3": 3.3,
  "inner1": {
    "include": "file://inline/inner_object_1.json"
  },
  "inner2": {
    "include": "inline/inner_object_2.json"
  }
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

Use `JsonCombiner` instead of `JsonSerializer` to deserialize JSON file from file system and inherit each inlined class or struct from `IJsonCombine` interface
```csharp
public TestObject LoadFromFile(string relativePath)
{
  string path = Path.Combine(Environment.CurrentDirectory, relativePath);
  return JsonCombiner.Deserialize<TestObject>(path, options);
}
```
