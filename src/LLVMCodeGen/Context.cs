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

    public ScopeDimensions()
    {
        Vars = new Dictionary<string, Var>();
        Returns = false;
    }
}

public struct Function
{
    public FunctionNode f;
    public LLVMValueRef ValueRef;
    public LLVMTypeRef type;
    public LLVMTypeRef retType;

    public Function(FunctionNode f, LLVMValueRef valueRef, LLVMTypeRef typeRef, LLVMTypeRef retType)
    {
        this.f = f;
        this.type = typeRef;
        this.ValueRef = valueRef;
        this.retType = retType;
    }
}

public sealed class Context
{
    private List<ScopeDimensions> ScopeDimension;
    private LLVMTypeRef _CurrentRetType;
    private Stack<LLVMTypeRef> types;
    public Dictionary<string, Function> functions = new();

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
        this.types = new Stack<LLVMTypeRef>();
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

    public void AddFunction(
        string name,
        FunctionNode functionNode,
        LLVMValueRef value,
        LLVMTypeRef type,
        LLVMTypeRef retType
    )
    {
        functions.Add(name, new Function(functionNode, value, type, retType));
    }

    public Function GetFunction(string name)
    {
        if (!functions.ContainsKey(name))
            throw new Exception("function doesnte exist");
        return functions[name];
    }

    public bool GetRet()
    {
        return ScopeDimension[ScopeDimension.Count - 1].Returns;
    }

    public void AddToTypeCheckerType(LLVMTypeRef type)
    {
        types.Push(type);
    }

    public LLVMTypeRef GetFromTypeChecker()
    {
        return types.Pop();
    }

    public LLVMValueRef HandleTypes(
        LLVMTypeRef targetType,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        INode expr
    )
    {
        if (targetType.IsOpaqueStruct)
            throw new Exception("structs arent supported yet");
        Dictionary<LLVMTypeRef, IVisitor> visitors =
            new()
            {
                [LLVMTypeRef.Int32] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Int16] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Int8] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Int1] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Float] = new FloatExprVisitor(),
            };
        LLVMValueRef eq = expr.CodeGen(visitors[targetType], builder, module, this);
        LLVMTypeRef type = this.GetFromTypeChecker();
        if (type != targetType)
            if (targetType.IntWidth < type.IntWidth)
                eq = builder.BuildTrunc(eq, targetType, "SET VAR");
            else
                eq = builder.BuildSExt(eq, targetType, "SET VAR");
        return eq;
    }
}

public class VaraibleDoesntExistException : Exception { }

public class VaraibleAlreadyDefinedException : Exception { }
