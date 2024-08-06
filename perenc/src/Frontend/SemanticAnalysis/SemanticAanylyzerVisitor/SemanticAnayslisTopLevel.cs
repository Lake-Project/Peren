using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticAnayslisTopLevel : TopLevelVisitor
{
    public SemanticContext<SemanticVar> Vars { get; set; }
    public SemanticContext<SemanticTypes> Types { get; set; }
    public SemanticContext<SemanticFunction> Function { get; init; }
    public SemanticProgram Program { get; set; }

    public SemanticAnayslisTopLevel()
    {
        Function = new();

        Types = new();
        Vars = new();
        Function.AllocateScope();
        Types.AllocateScope();
        Vars.AllocateScope();
        Program = new(Vars, Function, Types);
    }

    public override void Visit(VaraibleDeclarationNode node)
    {
        var type = SemanticAnaylsis.tokenToLacusType(node.Type, node.AttributesTuple.isConst, Program);
        Program.AddVar(
            node.Name,
            new SemanticVar(type, Program.Vars.GetSize(),
                node.AttributesTuple)
        );
        if (node.Expression != null)
        {
            // type.IsConst = true;
            LacusType t = node.Expression.Visit(
                new SemanticVisitExpr(Program, type)
            );
            if (!type.CanAccept(t))
                throw new TypeMisMatchException(
                    $"type {t} cant fit "
                    + $"{type} on line {node.Type.GetLine()}"
                );
            // type.IsConst = false;
        }
    }

    public override void Visit(FunctionNode node)
    {
        var f = new SemanticFunction(
            SemanticAnaylsis.tokenToLacusType(node.RetType.Name, node.RetType.tuple.isConst, Program),
            node.Parameters
                .Select(n =>
                    SemanticAnaylsis.tokenToLacusType(n.Type, n.AttributesTuple.isConst, Program)) //grab all params
                .ToList() // to list of lacus type
        );
        Program.Functions.AddValue(node.Name, f);
    }

    public override void Visit(ModuleNode node)
    {
        node.FunctionNodes.ForEach(n => n.Visit(this));
        node.StructNodes.ForEach(n => n.Visit(this));
        node.VaraibleDeclarationNodes.ForEach(n => n.Visit(this));
        node.FunctionNodes.ForEach(n => n.Visit(new SemanticVisitStatement(Program)));
    }

    public override void Visit(TopLevelStatement node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(StructNode node)
    {
        Program.Types.AddValue(node.Name, new SemanticTypes(
            new StructType(node
                    .Name
                    .buffer,
                node.Vars.ToDictionary(
                    n => n.Name.buffer, //name
                    n
                        => SemanticAnaylsis.tokenToLacusType(n.Type, n.AttributesTuple.isConst, Program) //type
                ), false)));
    }
}