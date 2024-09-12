namespace LacusLLVM.SemanticAanylyzerVisitor.Backend;

public class CompileContext<T>
{
    public Dictionary<string, T> Values { get; set; } = new();

    public void Add(string name, T value)
    {
        if (Values.ContainsKey(name))
        {
            Values.Remove(name);
        }

        Values.Add(name, value);
    }

    public T Get(string name)
    {
        return Values[name];
    }
}