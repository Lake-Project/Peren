using LLVMSharp.Interop;

public class WhileLoopNode : INode
{
    public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
    {
        throw new NotImplementedException();
    }

    public void Transform(IOptimize optimizer, Context context)
    {
        throw new NotImplementedException();
    }
}
