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

public struct SemanticTypes(LacusType type)
{
    public LacusType Type { get; set; } = type;
}

public struct SemanticProgram
{
    public SemanticContext<SemanticVar> Vars { get; set; }
    public SemanticContext<SemanticFunction> Functions { get; set; }
    public SemanticContext<SemanticTypes> Types { get; set; }

    public SemanticProgram(
        SemanticContext<SemanticVar> _Vars,
        SemanticContext<SemanticFunction> _functions,
        SemanticContext<SemanticTypes> _types)
    {
        Vars = _Vars;
        Functions = _functions;
        Types = _types;
        Vars.AllocateScope();
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
    public SemanticContext<SemanticVar> Vars { get; set; }
    public SemanticContext<SemanticTypes> Types { get; set; }

    public SemanticContext<SemanticFunction> Function { get; init; }

    public SemanticProgram p { get; set; }

    public SemanticVisitStatement()
    {
        Function = new();
        Types = new();
        Vars = new();
        Function.AllocateScope();
        Types.AllocateScope();

        p = new(Vars, Function, Types);
    }

    private SemanticFunction function;

    public override void Visit(VaraibleDeclarationNode node)
    {
        // if (node is ArrayNode)
        // {
        //     p.AddVar(node.Name,
        //         new SemanticVar(new ArrayType(tokenToLacusType(node.Type, false), node.AttributesTuple.isConst),
        //             p.Vars.GetSize(),
        //             node.AttributesTuple));
        // }
        // else
        // {
            var type = tokenToLacusType(node.Type, node.AttributesTuple.isConst);
            p.AddVar(
                node.Name,
                new SemanticVar(type, p.Vars.GetSize(),
                    node.AttributesTuple)
            );
            if (node.ExpressionNode != null)
            {
                LacusType t = node.ExpressionNode.Visit(
                    new SemanticVisitExpr(p, type)
                );
                if (!type.CanAccept(t))
                    throw new TypeMisMatchException(
                        $"type {t} cant fit "
                        + $"{type} on line {node.Type.GetLine()}"
                    );
            }
        // }
    }

    public override void Visit(VaraibleReferenceStatementNode node)
    {
        SemanticVar v = p.GetVar(node.Name);
        if (node is ArrayRefStatementNode arr)
            arr.Element.Visit(new SemanticVisitExpr(p, new IntegerType(false)));
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
            tokenToLacusType(node.RetType, false),
            node.Parameters.Select(n => tokenToLacusType(n.Type, false)) //grab all params
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
            throw new Exception($"type error, type {t} cant accept {function.retType}");
    }


    public override void Visit(ForLoopNode node)
    {
        p.Vars.AllocateScope();
        node.Iterator.Visit(this);
        node.Expr.Visit(new SemanticVisitExpr(p, new BoolType(false)));
        node.Statements.ForEach(n => n.Visit(this));
        node.Inc.Visit(this);
        p.Vars.DeallocateScope();
    }

    public override void Visit(WhileLoopNode node)
    {
        node.Expression.Visit(new SemanticVisitExpr(p, new BoolType(false)));
        p.Vars.AllocateScope();
        node.StatementNodes.ForEach(n => n.Visit(this));
        p.Vars.DeallocateScope();
    }

    public override void Visit(IfNode node)
    {
        node.Expression.Visit(new SemanticVisitExpr(p, new BoolType(false)));
        p.Vars.AllocateScope();
        node.StatementNodes.ForEach(n => n.Visit(this));
        p.Vars.DeallocateScope();
        p.Vars.AllocateScope();
        node.ElseNode.StatementNodes.ForEach(n => n.Visit(this));
        p.Vars.DeallocateScope();
    }

    public override void Visit(StructNode node)
    {
        Console.WriteLine(node.Name);
        p.Types.AddValue(node.Name, new SemanticTypes(
            new StructType(node
                    .Name
                    .buffer,
                node.Vars.ToDictionary(
                    n => n.Name.buffer, //name
                    n
                        => tokenToLacusType(n.Type, false) //type
                ), false)));
    }

    private LacusType tokenToLacusType(Tokens type, bool isConst)
    {
        if (type.tokenType == TokenType.WORD)
        {
            return p.Types.GetValue(type).Type;
        }

        return type.tokenType switch
        {
            TokenType.INT => new IntegerType(isConst),
            TokenType.INT16 => new IntegerType(isConst),
            TokenType.INT64 => new IntegerType(isConst),
            TokenType.BOOL => new BoolType(isConst),
            TokenType.FLOAT => new FloatType(isConst),
            TokenType.CHAR => new CharType(isConst),
            TokenType.VOID => new VoidType(isConst),
            TokenType.STRING => new ArrayType(new CharType(false), isConst),
            _ => throw new Exception($"type{type.ToString()} doesnt exist")
        };
    }
}