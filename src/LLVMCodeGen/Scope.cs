using System.Collections;
using LLVMSharp.Interop;


public struct Var
{
	public LLVMValueRef valueRef;
	public LLVMTypeRef type;
	public Var(LLVMTypeRef typeRef, LLVMValueRef valueRef)
	{
		this.valueRef = valueRef;
		this.type = typeRef;
	}

}
struct ScopeDimensions
{
	public Dictionary<string, Var> Vars;

	public ScopeDimensions()
	{
		Vars = new Dictionary<string, Var>();
	}
}
public sealed class Scope
{
	private List<ScopeDimensions> ScopeDimension;
	private Scope()
	{
		this.ScopeDimension = new List<ScopeDimensions>();

	}
	private static Scope? instance = null;
	public static Scope Instance
	{
		get => instance ??= new Scope();

	}
	public void AllocateScope()
	{
		ScopeDimensions s = new ScopeDimensions();
		ScopeDimension.Add(s);
	}
	public void DeallocateScope()
	{
		int l = ScopeDimension.Count - 1;
		ScopeDimension.RemoveAt(l);
	}
	public void AddNewVar(LLVMTypeRef type, string name, LLVMValueRef value)
	{
		for (int i = 0; i < ScopeDimension.Count; i++)
		{
			if (ScopeDimension[i].Vars.ContainsKey(name))
			{
				throw new VaraibleAlreadyDefinedException();
			}
		}
		ScopeDimension[ScopeDimension.Count - 1].Vars.Add(name, new Var(type, value));
	}
	public Var GetNewVar(string name)
	{
		for (int i = 0; i < ScopeDimension.Count; i++)
		{
			if (ScopeDimension[i].Vars.ContainsKey(name))
			{
				return ScopeDimension[i].Vars[name];
			}
		}
		throw new VaraibleDoesntExistException();
	}
}
public class VaraibleDoesntExistException : Exception
{

}
public class VaraibleAlreadyDefinedException : Exception
{

}