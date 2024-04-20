using System.Collections;
using Lexxer;
using LLVMSharp.Interop;

public struct Var
{
    public LLVMValueRef valueRef;
    public LLVMTypeRef type;

    public Var(LLVMTypeRef typeRef, LLVMValueRef valueRef)
    {
        this.valueRef = valueRef;
        this.type = typeRef;
    }
}

struct ScopeDimensions
{
    public Dictionary<string, Var> Vars;
    public bool Returns;
    public Dictionary<string, FunctionNode> functions;

    public ScopeDimensions()
    {
        Vars = new Dictionary<string, Var>();
        Returns = false;
        functions = new Dictionary<string, FunctionNode>();
    }
}

public sealed class Context
{
    private List<ScopeDimensions> ScopeDimension;
    private LLVMTypeRef _CurrentRetType;
    public Dictionary<string, FunctionNode> functions;

    // public Dictionary<string,

    // Property with getter and setter
    public LLVMTypeRef CurrentRetType
    {
        get { return _CurrentRetType; }
        set { _CurrentRetType = value; }
    }

    public Context()
    {
        this.ScopeDimension = new List<ScopeDimensions>();
        AllocateScope();
    }

    public void AllocateScope()
    {
        ScopeDimensions s = new ScopeDimensions();
        ScopeDimension.Add(s);
    }

    public int ScopeSize()
    {
        return ScopeDimension.Count - 1;
    }

    public void DeallocateScope()
    {
        int l = ScopeDimension.Count - 1;
        ScopeDimension.RemoveAt(l);
    }

    public void AddNewVar(LLVMTypeRef type, string name, LLVMValueRef value)
    {
        for (int i = 0; i < ScopeDimension.Count; i++)
        {
            if (ScopeDimension[i].Vars.ContainsKey(name))
            {
                throw new VaraibleAlreadyDefinedException();
            }
        }
        ScopeDimension[ScopeDimension.Count - 1].Vars.Add(name, new Var(type, value));
    }

    public Var GetVar(string name)
    {
        for (int i = 0; i < ScopeDimension.Count; i++)
        {
            if (ScopeDimension[i].Vars.ContainsKey(name))
            {
                return ScopeDimension[i].Vars[name];
            }
        }
        throw new VaraibleDoesntExistException();
    }

    public void Setret()
    {
        ScopeDimensions s = ScopeDimension[ScopeDimension.Count - 1];
        s.Returns = true;
        ScopeDimension[ScopeDimension.Count - 1] = s;
    }

    public void AddFunction(string name, FunctionNode functionNode)
    {
        functions.Add(name, functionNode);
    }

    public FunctionNode GetFunction(string name)
    {
        return functions[name];
    }

    public bool GetRet()
    {
        return ScopeDimension[ScopeDimension.Count - 1].Returns;
    }
}

public class VaraibleDoesntExistException : Exception { }

public class VaraibleAlreadyDefinedException : Exception { }
