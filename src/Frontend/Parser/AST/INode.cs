using LLVMSharp.Interop;

public interface INode
{
    public void Transform(IOptimize optimizer, Context context);
    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    );
}
