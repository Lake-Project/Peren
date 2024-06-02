using LLVMSharp.Interop;

public class CodeGenVisitor : IVisitor
{
    public LLVMValueRef Visit(
        IntegerNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        throw new NotImplementedException();
    }

    public LLVMValueRef Visit(
        FloatNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        throw new NotImplementedException();
    }

    public LLVMValueRef Visit(
        OpNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        throw new NotImplementedException();
    }

    public LLVMValueRef Visit(
        VaraibleReferenceNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        throw new NotImplementedException();
    }

    public LLVMValueRef Visit(
        FunctionCallNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        Function fun = context.GetFunction(node.Name);
        return builder.BuildCall2(fun.type, fun.ValueRef, node.Values, fun.f.name.buffer);
    }
}
