using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.Frontend.Parser.AST;

public class PerenNode(Dictionary<string, ModuleNode> modules) : StatementNode
{
    public Dictionary<string, ModuleNode> ModuleNodes { get; set; } = modules;

    public ModuleNode GetStart()
    {
        foreach (var module in ModuleNodes.Values)
        {
            foreach (var function in module.FunctionNodes)
            {
                if (function.Name.buffer == "main")
                {
                    return module;
                }
            }
        }

        return ModuleNodes.Values.ToList()[0];
    }

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}