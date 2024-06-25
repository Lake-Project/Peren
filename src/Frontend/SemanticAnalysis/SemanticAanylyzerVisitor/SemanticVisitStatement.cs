using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public struct SemanticVar
{
    public LacusType VarType { get; set; }
    public int ScopeLocation { get; set; }

    public AttributesTuple AttributesTupe { get; set; }

    public SemanticVar(LacusType type, int scopeLocation, AttributesTuple attributesTuple)
    {
        VarType = type;
        ScopeLocation = scopeLocation;
        AttributesTupe = attributesTuple;
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
        p.AddVar(
            node.Name,
            new SemanticVar(tokenToLacusType(node.Type), p.Vars.GetSize(), node.AttributesTuple)
        );
        if (node.ExpressionNode != null)
        {
            LacusType t = node.ExpressionNode.Visit(
                new SemanticVisitExpr(p, tokenToLacusType(node.Type))
            );
            if (!tokenToLacusType(node.Type).CanAccept(t))
                throw new TypeMisMatchException(
                    $"type {t} cant fit "
                        + $"{tokenToLacusType(node.Type)} on line {node.Type.GetLine()}"
                );
        }
    }

    public override void Visit(VaraibleReferenceStatementNode node)
    {
        SemanticVar v = p.GetVar(node.Name);

        LacusType l = node.Expression.Visit(new SemanticVisitExpr(p, v.VarType));
        if (v.AttributesTupe.isConst)
            throw new Exception(
                $"type const {v.VarType} cant fit into {l} on line {node.Name.GetLine()}"
            );
        node.ScopeLocation = v.ScopeLocation;
        if (!v.VarType.CanAccept(l))
            throw new TypeMisMatchException(
                $"type {l} cant fit " + $"{v.VarType} on line {node.Name.GetLine()}"
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
            tokenToLacusType(node.RetType),
            node.Parameters.Select(n => tokenToLacusType(n.Type)) //grab all params
                .ToList() // to list of lacus type
        );
        p.Functions.AddValue(node.Name, f);
        this.function = f;
        node.Parameters.ForEach(n => n.Visit(this));
        node.Statements.ForEach(n => n.Visit(this));
        p.Vars.DeallocateScope();
    }

    public override void Visit(ReturnNode node)
    {
        LacusType t = new VoidType();
        if (node.Expression != null)
        {
            t = node.Expression.Visit(new SemanticVisitExpr(p, function.retType));
        }
        if (!function.retType.CanAccept(t))
            throw new Exception("type error, type");
    }

    public override void Visit(CastNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(ForLoopNode node)
    {
        p.Vars.AllocateScope();
        node.Iterator.Visit(this);
        node.Expr.Visit(new SemanticVisitExpr(p, new BoolType()));
        node.Statements.ForEach(n => n.Visit(this));
        node.Inc.Visit(this);
        p.Vars.DeallocateScope();


    }

    public override void Visit(WhileLoopNode node)
    {
        node.Expression.Visit(new SemanticVisitExpr(p, new BoolType()));
        p.Vars.AllocateScope();
        node.StatementNodes.ForEach(n => n.Visit(this));
        p.Vars.DeallocateScope();
    }

    public override void Visit(IfNode node)
    {
        // Console.WriteLine(node.ToString());
        node.Expression.Visit(new SemanticVisitExpr(p, new BoolType()));
        p.Vars.AllocateScope();
        node.StatementNodes.ForEach(n => n.Visit(this));
        p.Vars.DeallocateScope();
        p.Vars.AllocateScope();
        node.ElseNode.StatementNodes.ForEach(n => n.Visit(this));
        p.Vars.DeallocateScope();
    }

    public override void Visit(StructNode node)
    {
        throw new NotImplementedException();
    }

    private LacusType tokenToLacusType(Tokens type)
    {
        return type.tokenType switch
        {
            TokenType.INT => new IntegerType(),
            TokenType.INT16 => new IntegerType(),
            TokenType.INT64 => new IntegerType(),
            TokenType.BOOL => new BoolType(),
            TokenType.FLOAT => new FloatType(),
            TokenType.CHAR => new CharType(),
            TokenType.VOID => new VoidType(),
            TokenType.STRING => new ArrayType(new CharType()),
            _ => throw new Exception("error")
        };
    }
}
