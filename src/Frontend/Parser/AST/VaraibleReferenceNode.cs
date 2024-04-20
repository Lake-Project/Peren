using System.Linq.Expressions;
using LLVMSharp.Interop;


public class VaraibleReferenceNode : INode
{
	public string name;
	public VaraibleReferenceNode(string varName)
	{
		name = varName;
	}
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
	{
		// throw new NotImplementedException();
		return visitor.Visit(this, builder, module, context);
	}
    public override string ToString()
    {
        return name;
    }
}