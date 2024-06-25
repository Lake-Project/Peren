using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.Frontend.Parser.AST;

public class CharNode(char value) : INode
{
    public char Value { get; set; } = value;

    public T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}
