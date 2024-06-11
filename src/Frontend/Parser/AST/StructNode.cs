using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class StructNode : INode
{
    public List<VaraibleDeclarationNode> vars;
    public Tokens name;

    public T Visit<T>(ExpressionVisit<T> visit) => throw new NotImplementedException();
}
