using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LLVMSharp.Interop;

namespace LacusLLVM.SemanticAanylyzerVisitor.Backend;

public class CompilerModule
{
    public Dictionary<string, LLVMFunction> Functions { get; set; } = new();
    public Dictionary<string, LLVMType> Types { get; set; } = new();
    public Dictionary<string, LLVMVar> Varaibles { get; set; } = new();
    public ModuleNode ModuleNode { get; set; }
    public List<CompilerModule> imports = new();
    
}