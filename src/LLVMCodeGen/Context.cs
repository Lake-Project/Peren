using System.Collections;
using System.Runtime.CompilerServices;
using Lexxer;
using LLVMSharp.Interop;

public struct Var
{
    public LLVMValueRef valueRef;
    public Dictionary<string, Var> dependecyGraph;
    public LLVMTypeRef type;
    public LLVMValueRef constant;
    public bool IsConstant;

    public Var(LLVMTypeRef typeRef, LLVMValueRef valueRef)
    {
        this.valueRef = valueRef;
        this.dependecyGraph = new Dictionary<string, Var>();
        this.type = typeRef;
        IsConstant = false;
    }

    public Var(LLVMTypeRef typeRef, LLVMValueRef var, LLVMValueRef constant)
    {
        this.valueRef = var;
        this.dependecyGraph = new Dictionary<string, Var>();
        this.type = typeRef;
        this.constant = constant;
        IsConstant = true;
    }
}

public struct Types
{
    public LLVMTypeRef type;

    public Types(LLVMTypeRef typeRef)
    {
        this.type = typeRef;
    }
}

struct ScopeDimensions
{
    public Dictionary<string, Type> Types;
    public Dictionary<string, Var> Vars;
    public bool Returns;

    public ScopeDimensions()
    {
        Vars = new Dictionary<string, Var>();
        Types = new Dictionary<string, Type>();
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

    public void AddNewVar(LLVMTypeRef type, Tokens name, LLVMValueRef value)
    {
        for (int i = 0; i < ScopeDimension.Count; i++)
        {
            if (
                ScopeDimension[i].Vars.ContainsKey(name.buffer)
                || ScopeDimension[i].Types.ContainsKey(name.buffer)
            )
            {
                throw new VaraibleAlreadyDefinedException(
                    $"Varaible {name.buffer} already exists on line ${name.GetLine()}"
                );
            }
        }
        ScopeDimension[ScopeDimension.Count - 1].Vars.Add(name.buffer, new Var(type, value));
    }

    // public void AddNewVarAsConst(
    //     LLVMTypeRef type,
    //     string name,
    //     LLVMValueRef var,
    //     LLVMValueRef constant
    // )
    // {
    //     for (int i = 0; i < ScopeDimension.Count; i++)
    //     {
    //         if (ScopeDimension[i].Vars.ContainsKey(name))
    //         {
    //             throw new VaraibleAlreadyDefinedException($"Varaible {}");
    //         }
    //     }
    //     ScopeDimension[ScopeDimension.Count - 1].Vars.Add(name, new Var(type, var, constant));
    // }

    // public void UpdateConst(string name, LLVMValueRef val)
    // {
    //     for (int i = 0; i < ScopeDimension.Count; i++)
    //     {
    //         if (ScopeDimension[i].Vars.ContainsKey(name))
    //         {
    //             Var a = ScopeDimension[i].Vars[name];
    //             a.constant = val;
    //             a.IsConstant = true;
    //             ScopeDimension[i].Vars[name] = a;
    //             return;
    //         }
    //     }
    //     throw new VaraibleDoesntExistException(
    //         $"Varaible {name.buffer} doesnt exist on like {name.GetLine()}"
    //     );
    // }

    // public void removeConst(Tokens name)
    // {
    //     for (int i = 0; i < ScopeDimension.Count; i++)
    //     {
    //         if (ScopeDimension[i].Vars.ContainsKey(name.buffer))
    //         {
    //             Var a = ScopeDimension[i].Vars[name.buffer];
    //             a.IsConstant = false;
    //             ScopeDimension[i].Vars[name.buffer] = a;
    //             return;
    //         }
    //     }
    //     throw new VaraibleDoesntExistException(
    //         $"Varaible {name.buffer} doesnt exist on like {name.GetLine()}"
    //     );
    // }

    public Var GetVar(Tokens name)
    {
        for (int i = 0; i < ScopeDimension.Count; i++)
        {
            if (ScopeDimension[i].Vars.ContainsKey(name.buffer))
            {
                return ScopeDimension[i].Vars[name.buffer];
            }
        }
        throw new VaraibleDoesntExistException("$");
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

    public Function GetFunction(Tokens name)
    {
        if (!functions.ContainsKey(name.buffer))
            throw new Exception("function doesnte exist");
        return functions[name.buffer];
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

public class VaraibleDoesntExistException : Exception
{
    public VaraibleDoesntExistException(string message) : base(message) { }
}

public class VaraibleAlreadyDefinedException : Exception
{
    public VaraibleAlreadyDefinedException(string message) : base(message) { }
}
