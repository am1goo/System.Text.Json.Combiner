# System.Text.Json.Combiner

#### How it works
Use `JsonCombiner` instead of `JsonSerializer` to deserialize JSON file from file system
```csharp
public TestObject LoadFromFile(string relativePath)
{
  string path = Path.Combine(Environment.CurrentDirectory, relativePath);
  return JsonCombiner.Deserialize<TestObject>(path, options);
}
```
