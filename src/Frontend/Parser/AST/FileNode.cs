namespace LacusLLVM.Frontend.Parser.AST;

public class FileNode
{
    public Dictionary<string, FunctionNode> FunctionNodes = new();
    public Dictionary<string, StructNode> StructNodes = new();
    
}