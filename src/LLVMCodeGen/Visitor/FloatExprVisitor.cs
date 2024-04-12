using LLVMSharp.Interop;

public class FloatExprVisitor : IVisitor
{
    public LLVMValueRef Visit(IntegerNode node, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
    {
        throw new NotImplementedException();
    }

    public LLVMValueRef Visit(FloatNode node, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
    {
        throw new NotImplementedException();
    }

    public LLVMValueRef Visit(OpNode node, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
    {
        throw new NotImplementedException();
    }
    public LLVMValueRef Visit(VaraibleReferenceNode node, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
    {
        throw new NotImplementedException();
    }
}