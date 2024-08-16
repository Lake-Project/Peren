using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LLVMLake.Frontend;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticAnayslisTopLevel : StatementVisit
{
    public SemanticContext<SemanticVar> Vars { get; set; }
    public SemanticContext<SemanticTypes> Types { get; set; }
    public SemanticContext<SemanticFunction> Function { get; init; }

    public SemanticProgram Program { get; set; }
    public Dictionary<string, ModuleNode> AvaibleModules;

    public SemanticAnayslisTopLevel()
    {
        Function = new();
        Types = new();
        Vars = new();
        Program = new(Vars, Function, Types);
    }

    public override void Visit(VaraibleDeclarationNode node)
    {
        if (node is ArrayNode n)
        {
            n.Size.Visit(new SemanticVisitExpr(Program, new IntegerType(true)));
        }

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
    }

    public override void Visit(FunctionNode node)
    {
        var f = new SemanticFunction(
            SemanticAnaylsis.tokenToLacusType(node.RetType.Name, node.RetType.tuple.isConst, Program),
            node.Parameters
                .Select(n =>
                    SemanticAnaylsis.tokenToLacusType(n.Type, n.AttributesTuple.isConst, Program)) //grab all params
                .ToList(),
            node.AttributesTuple.isPub // to list of lacus type
        );
        Program.Functions.AddValue(node.Name, f);
    }

    public override void Visit(ModuleNode node)
    {
        node.Imports.ForEach(n =>
        {
            if (!AvaibleModules.ContainsKey(n.buffer))
                throw new ModuleException(
                    $"module {node.Name.buffer} Cant import Module {n.buffer} on line {node.Name.GetLine()}");
            if (n.buffer == node.Name.buffer)
                throw new ModuleException(
                    $"recursive import detected in module {node.Name.buffer} on line {node.Name.GetLine()}");

            AvaibleModules[n.buffer].StructNodes.Where(n => n.AttributesTuple.isPub).ToList()
                .ForEach(n => n.Visit(this));
            AvaibleModules[n.buffer].FunctionNodes.Where(n => n.AttributesTuple.isPub).ToList()
                .ForEach(n => n.Visit(this));
            AvaibleModules[n.buffer].VaraibleDeclarationNodes.Where(n => n.AttributesTuple.isPub).ToList()
                .ForEach(n => n.Visit(this));
        });
        node.StructNodes.ForEach(n => n.Visit(this));
        node.FunctionNodes.ForEach(n => n.Visit(this));
        node.VaraibleDeclarationNodes.ForEach(n => n.Visit(this));
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

    public override void Visit(PerenNode node)
    {
        AvaibleModules = node.ModuleNodes;
        node.ModuleNodes.Values.ToList()
            .ForEach(module =>
            {
                Program.Functions.AllocateScope();
                Program.Types.AllocateScope();
                Program.Vars.AllocateScope();
                module.Visit(this);
                module.Visit(new SemanticVisitStatement(Program));
                Program.Functions.DeallocateScope();
                Program.Vars.DeallocateScope();
                Program.Types.DeallocateScope();
            });
    }
}