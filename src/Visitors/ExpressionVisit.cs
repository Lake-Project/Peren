using LacusLLVM.Frontend.Parser.AST;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit; //.ss
//push :3
public abstract class ExpressionVisit<T>
{
    public abstract T Visit(IntegerNode node);
    public abstract T Visit(FloatNode node);
    public abstract T Visit(BoolNode node);
    public abstract T Visit(FunctionCallNode node);
    public abstract T Visit(OpNode node);
    public abstract T Visit(VaraibleReferenceNode node);
    public abstract T Visit(BooleanExprNode node);
    public abstract T Visit(CharNode node);
    public abstract T Visit(CastNode node);
    public abstract T Visit(StringNode node);

}