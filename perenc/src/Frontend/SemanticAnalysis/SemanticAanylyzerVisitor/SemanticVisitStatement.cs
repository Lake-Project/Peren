using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public struct SemanticVar
{
    public PerenType VarType { get; set; }
    public int ScopeLocation { get; set; }

    public AttributesTuple AttributesTupe { get; set; }

    public SemanticVar(PerenType type, int scopeLocation, AttributesTuple attributesTuple)
    {
        VarType = type;
        ScopeLocation = scopeLocation;
        AttributesTupe = attributesTuple;
    }
}

public struct SemanticFunction
{
    public PerenType RetType { get; set; }
    public List<PerenType> ParamTypes { get; set; }

    public SemanticFunction(PerenType type, List<PerenType> paramTypes, bool isPub)
    {
        RetType = type;
        ParamTypes = paramTypes;
    }
}

public struct SemanticTypes(PerenType type)
{
    public PerenType Type { get; set; } = type;
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
            n.Size.Visit(new SemanticVisitExpr(Program, new IntegerType(false)));
        }

        if (node is ArrayNode)
        {
            Program.AddVar(node.Name,
                new SemanticVar(new 
                        ArrayType(SemanticAnaylsis.TokenToPerenType(node.Type, false, Program), node.AttributesTuple.isConst),
                    Program.Vars.GetSize(),
                    node.AttributesTuple));
        }
        else
        {
            var type = SemanticAnaylsis.TokenToPerenType(node.Type, node.AttributesTuple.isConst, Program);
            Program.AddVar(
                node.Name,
                new SemanticVar(type, Program.Vars.GetSize(),
                    node.AttributesTuple)
            );
            if (node.Expression != null)
            {
                PerenType t = node.Expression.Visit(
                    new SemanticVisitExpr(Program, type)
                );
                if (!type.CanAccept(t))
                    throw new TypeMisMatchException(
                        $"type {t} cant fit "
                        + $"{type} on line {node.Type.GetLine()}"
                    );
            }
        }
    }

    public override void Visit(VaraibleReferenceStatementNode node)
    {
        SemanticVar v = Program.GetVar(node.Name);
        if (node is ArrayRefStatementNode arr)
        {
            PerenType varTypes = node.Expression.Visit(new SemanticVisitExpr(Program, v.VarType.simplerType));
            PerenType l2 = arr.Element.Visit(new SemanticVisitExpr(Program, new IntegerType(false)));
            if (!v.VarType.simplerType.CanAccept(varTypes))
                throw new TypeMisMatchException(
                    $"type {l2} cant fit " + $"{v.VarType} on line {node.Name.GetLine()}"
                );
            if (v.AttributesTupe.isConst)
                throw new Exception(
                    $"type const {v.VarType} cant fit into {l2} on line {node.Name.GetLine()}"
                );
            

        }
        else
        {
            PerenType l = node.Expression.Visit(new SemanticVisitExpr(Program, v.VarType));
            if (!v.VarType.CanAccept(l))
                throw new TypeMisMatchException(
                    $"type {l} cant fit " + $"{v.VarType} on line {node.Name.GetLine()}"
                );
            if (v.AttributesTupe.isConst)
                throw new Exception(
                    $"type const {v.VarType} cant fit into {l} on line {node.Name.GetLine()}"
                );
        }
        // if (node is ArrayRefStatementNode arr)
        //     arr.Element.Visit(new SemanticVisitExpr(Program, new IntegerType(false)));
        
        
    }

    public override void Visit(FunctionCallNode node)
    {
        SemanticFunction f = Program.Functions.GetValue(node.Name);

        if (node.ParamValues.Count != f.ParamTypes.Count)
            throw new Exception("no matching type");
        for (int i = 0; i < f.ParamTypes.Count; i++)
        {
            PerenType t = node.ParamValues[i].Visit(new SemanticVisitExpr(Program, f.ParamTypes[i]));
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
        PerenType t = new VoidType();
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

    public override void Visit(ModuleNode moduleNode)
    {
        moduleNode
            .FunctionNodes.ForEach(n => n.Visit(this));
    }
}