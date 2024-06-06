using LacusLLVM.Frontend.SemanticAnalysis;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public struct SemanticVar
{
    public LacusType VarType { get; set; }
    public int ScopeLocation { get; set; }

    public SemanticVar(LacusType type, int scopeLocation)
    {
        VarType = type;
        ScopeLocation = scopeLocation;
    }
}

public struct SemanticFunction
{
    public LacusType retType { get; set; }
    public List<LacusType> ParamTypes { get; set; }

    public SemanticFunction(LacusType type, List<LacusType> paramTypes)
    {
        retType = type;
        ParamTypes = paramTypes;
    }
}

public struct SemanticProgram
{
    public SemanticContext<SemanticVar> Vars { get; set; }
    public SemanticContext<SemanticFunction> Functions { get; set; }

    public SemanticProgram(
        SemanticContext<SemanticVar> _Vars,
        SemanticContext<SemanticFunction> _functions
    )
    {
        Vars = _Vars;
        Functions = _functions;
        Vars.AllocateScope();
    }

    public void Deallocate()
    {
        Vars.DeallocateScope();
    }

    public void AddFunction(Tokens name, SemanticFunction value)
    {
        Functions.AddValue(name, value);
    }

    public SemanticFunction GetFunction(Tokens name)
    {
        return Functions.GetValue(name);
    }

    public void AddVar(Tokens name, SemanticVar value)
    {
        Vars.AddValue(name, value);
    }

    public SemanticVar GetVar(Tokens name)
    {
        return Vars.GetValue(name);
    }
}

public class SemanticVisitStatement : StatementVisit
{
    public SemanticContext<SemanticVar> _Context { get; set; }

    public SemanticContext<SemanticFunction> Function { get; init; }

    public SemanticProgram p { get; set; }

    public SemanticVisitStatement()
    {
        Function = new();
        _Context = new();
        Function.AllocateScope();

        p = new(_Context, Function);
    }

    private SemanticFunction function;

    public override void Visit(VaraibleDeclarationNode node)
    {
        p.AddVar(node.name, new SemanticVar(tokenToLacusType(node.type), p.Vars.GetSize()));
        if (node.ExpressionNode != null)
        {
            LacusType t = node.ExpressionNode.Visit(
                new SemanticVisitExpr(p, tokenToLacusType(node.type))
            );
            if (!tokenToLacusType(node.type).CanAccept(t))
                throw new TypeMisMatchException(
                    $"type {t} cant fit "
                        + $"{tokenToLacusType(node.type)} on line {node.type.GetLine()}"
                );
        }
    }

    public override void Visit(VaraibleReferenceStatementNode node)
    {
        SemanticVar v = p.GetVar(node.name);
        LacusType l = node.expression.Visit(new SemanticVisitExpr(p, v.VarType));
        node.ScopeLocation = v.ScopeLocation;
        if (v.VarType.CanAccept(l))
            throw new TypeMisMatchException(
                $"type {l} cant fit " + $"{v.VarType} on line {node.name.GetLine()}"
            );
    }

    public override void Visit(FunctionCallNode node)
    {
        SemanticFunction f = p.Functions.GetValue(node.Name);

        if (node.ParamValues.Count != f.ParamTypes.Count)
            throw new Exception("no matching type");
        for (int i = 0; i < f.ParamTypes.Count; i++)
        {
            LacusType t = node.ParamValues[i].Visit(new SemanticVisitExpr(p, f.ParamTypes[i]));
            if (!f.ParamTypes[i].CanAccept(t))
                throw new Exception("error");
        }
    }

    public override void Visit(FunctionNode node)
    {
        p.Vars.AllocateScope();
        var f = new SemanticFunction(
            tokenToLacusType(node.retType),
            node.Parameters.Select(n => tokenToLacusType(n.type)) //grab all params
                .ToList() // to list of lacus type
        );
        p.Functions.AddValue(node.name, f);
        function = f;
        node.Parameters.ForEach(n => n.Visit(this));
        node.statements.ForEach(n => n.Visit(this));
        p.Vars.DeallocateScope();
    }

    public override void Visit(ReturnNode node)
    {
        LacusType t = new VoidType();
        if (node.expression != null)
        {
            t = node.expression.Visit(new SemanticVisitExpr(p, function.retType));
        }

        if (!function.retType.CanAccept(t))
            throw new Exception("type error");
    }

    public override void Visit(CastNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(ForLoopNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(WhileLoopNode node)
    {
        throw new NotImplementedException();
    }

    private LacusType tokenToLacusType(Tokens type)
    {
        return type.tokenType switch
        {
            TokenType.INT => new IntegerType(),
            TokenType.BOOL => new BoolType(),
            TokenType.FLOAT => new FloatType(),
            TokenType.CHAR => new CharType(),
            TokenType.VOID => new VoidType(),
            _ => throw new Exception("error")
        };
    }
}
