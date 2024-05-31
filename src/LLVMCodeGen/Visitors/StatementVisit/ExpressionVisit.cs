using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public abstract class ExpressionVisit
{
    public abstract LLVMValueRef Visit(IntegerNode node);
    public abstract LLVMValueRef Visit(FloatNode node);
    public abstract LLVMValueRef Visit(BoolNode node);
    public abstract LLVMValueRef Visit(FunctionCallNode node);
    public abstract LLVMValueRef Visit(OpNode node);
    public abstract LLVMValueRef Visit(VaraibleReferenceNode node);
}
