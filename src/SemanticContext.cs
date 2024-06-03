using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public struct Scope<T>
{
    // private T value;
    public Dictionary<string, T> values = new();

    public Scope() { }

    public void Add(string key, T value)
    {
        values.Add(key, value);
    }

    public T Get(string name)
    {
        return values[name];
    }

    public bool Contains(string name)
    {
        return values.ContainsKey(name);
    }
}

public class SemanticContext<T>
{
    public List<Scope<T>> Scopes = new();

    public SemanticContext() { }

    public void AllocateScope()
    {
        Scopes.Add(new Scope<T>());
    }

    public void AddValue(Tokens name, T value)
    {
        for (int i = 0; i < Scopes.Count; i++)
        {
            if (Scopes[i].values.ContainsKey(name.buffer))
            {
                throw new VaraibleAlreadyDefinedException(
                    $"Variable {name.buffer} already exists on line ${name.GetLine() + 1}"
                );
            }
        }

        Scopes[Scopes.Count - 1].Add(name.buffer, value);
    }

    public T GetValue(Tokens name)
    {
        for (int i = 0; i < Scopes.Count; i++)
        {
            if (Scopes[i].Contains(name.buffer))
            {
                return Scopes[i].Get(name.buffer);
            }
        }

        throw new VaraibleDoesntExistException(
            $"Varaible {name.buffer} doesnt exist on line {name.GetLine() + 1}"
        );
    }

    public T GetValue(Tokens name, int index)
    {
        return Scopes[index].Get(name.buffer);
    }

    public void DeallocateScope()
    {
        int l = Scopes.Count - 1;
        Scopes.RemoveAt(l);
    }

    public int GetSize()
    {
        return Scopes.Count - 1;
    }
}
