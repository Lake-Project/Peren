using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.Frontend.Parser.AST;

public class CharNode(char value) : ExpressionNode
{
    public char Value { get; set; } = value;

    public override T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}
