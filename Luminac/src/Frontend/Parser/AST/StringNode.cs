using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class StringNode(Tokens value) : ExpressionNode
{
    public Tokens Token { get; set; } = value;
    public string Value { get; set; } = value.buffer;
    public override T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}