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
    public LacusType RetType { get; set; }
    public List<LacusType> ParamTypes { get; set; }

    public SemanticFunction(LacusType type, List<LacusType> paramTypes)
    {
        RetType = type;
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

public class SemanticVisitStatement(SemanticProgram program) : StatementVisit
{
    public SemanticContext<SemanticVar> Vars { get; set; }
    public SemanticContext<SemanticTypes> Types { get; set; }

    public SemanticContext<SemanticFunction> Function { get; init; }

    public SemanticProgram Program { get; set; } = program;

    // public SemanticVisitStatement()
    // {
    //     Function = new();
    //     Types = new();
    //     Vars = new();
    //     Function.AllocateScope();
    //     Types.AllocateScope();
    //
    //     // Program = new(Vars, Function, Types);
    // }

    private SemanticFunction function;

    public override void Visit(VaraibleDeclarationNode node)
    {
        if (node is ArrayNode n)
        {
            n.Size.Visit(new SemanticVisitExpr(Program, new IntegerType(true)));
        }

        // if (node is ArrayNode)
        // {
        //     p.AddVar(node.Name,
        //         new SemanticVar(new ArrayType(tokenToLacusType(node.Type, false), node.AttributesTuple.isConst),
        //             p.Vars.GetSize(),
        //             node.AttributesTuple));
        // }
        // else
        // {
        var type = SemanticAnaylsis.tokenToLacusType(node.Type, node.AttributesTuple.isConst, Program);
        Program.AddVar(
            node.Name,
            new SemanticVar(type, Program.Vars.GetSize(),
                node.AttributesTuple)
        );
        if (node.Expression != null)
        {
            LacusType t = node.Expression.Visit(
                new SemanticVisitExpr(Program, type)
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
        SemanticVar v = Program.GetVar(node.Name);
        if (node is ArrayRefStatementNode arr)
            arr.Element.Visit(new SemanticVisitExpr(Program, new IntegerType(false)));
        LacusType l = node.Expression.Visit(new SemanticVisitExpr(Program, v.VarType));
        if (v.AttributesTupe.isConst)
            throw new Exception(
                $"type const {v.VarType} cant fit into {l} on line {node.Name.GetLine()}"
            );
        if (!v.VarType.CanAccept(l))
            throw new TypeMisMatchException(
                $"type {l} cant fit " + $"{v.VarType} on line {node.Name.GetLine()}"
            );
    }

    public override void Visit(FunctionCallNode node)
    {
        SemanticFunction f = Program.Functions.GetValue(node.Name);

        if (node.ParamValues.Count != f.ParamTypes.Count)
            throw new Exception("no matching type");
        for (int i = 0; i < f.ParamTypes.Count; i++)
        {
            LacusType t = node.ParamValues[i].Visit(new SemanticVisitExpr(Program, f.ParamTypes[i]));
            if (!f.ParamTypes[i].CanAccept(t))
                throw new Exception("error");
        }
    }

    public override void Visit(FunctionNode node)
    {
        Program.Vars.AllocateScope();
        // var f = new SemanticFunction(
        //     tokenToLacusType(node.RetType.Name, node.RetType.tuple.isConst),
        //     node.Parameters
        //         .Select(n =>
        //             tokenToLacusType(n.Type, n.AttributesTuple.isConst)) //grab all params
        //         .ToList() // to list of lacus type
        // );
        // Program.Functions.AddValue(node.Name, f);
        this.function = Program.GetFunction(node.Name);
        node.Parameters.ForEach(n => n.Visit(this));
        node.Statements.ForEach(n => n.Visit(this));
        Program.Vars.DeallocateScope();
    }

    public override void Visit(ReturnNode node)
    {
        LacusType t = new VoidType();
        if (node.Expression != null)
        {
            t = node.Expression.Visit(new SemanticVisitExpr(Program, function.RetType));
        }

        if (!function.RetType.CanAccept(t))
            throw new Exception($"type error, type {t} cant accept {function.RetType}");
    }


    public override void Visit(ForLoopNode node)
    {
        Program.Vars.AllocateScope();
        node.Iterator.Visit(this);
        node.Expr.Visit(new SemanticVisitExpr(Program, new BoolType(false)));
        node.Statements.ForEach(n => n.Visit(this));
        node.Inc.Visit(this);
        Program.Vars.DeallocateScope();
    }

    public override void Visit(WhileLoopNode node)
    {
        node.Expression.Visit(new SemanticVisitExpr(Program, new BoolType(false)));
        Program.Vars.AllocateScope();
        node.StatementNodes.ForEach(n => n.Visit(this));
        Program.Vars.DeallocateScope();
    }

    public override void Visit(IfNode node)
    {
        node.Expression.Visit(new SemanticVisitExpr(Program, new BoolType(false)));
        Program.Vars.AllocateScope();
        node.StatementNodes.ForEach(n => n.Visit(this));
        Program.Vars.DeallocateScope();
        Program.Vars.AllocateScope();
        node.ElseNode.StatementNodes.ForEach(n => n.Visit(this));
        Program.Vars.DeallocateScope();
    }

    // public override void Visit(StructNode node)
    // {
    //     Program.Types.AddValue(node.Name, new SemanticTypes(
    //         new StructType(node
    //                 .Name
    //                 .buffer,
    //             node.Vars.ToDictionary(
    //                 n => n.Name.buffer, //name
    //                 n
    //                     => SemanticAnaylsis.tokenToLacusType(n.Type, n.AttributesTuple.isConst, Program) //type
    //             ), false)));
    // }
}