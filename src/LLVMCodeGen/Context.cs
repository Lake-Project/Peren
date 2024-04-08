using System.Collections;
using Lexxer;
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
public sealed class Context
{
	private List<ScopeDimensions> ScopeDimension;
	private LLVMTypeRef _CurrentRetType;

	// Property with getter and setter
	public LLVMTypeRef CurrentRetType
	{
		get { return _CurrentRetType; }
		set { _CurrentRetType = value; }
	}
	public Context()
	{
		this.ScopeDimension = new List<ScopeDimensions>();
		AllocateScope();
	}
	public void AllocateScope()
	{
		ScopeDimensions s = new ScopeDimensions();
		ScopeDimension.Add(s);
	}
	public int ScopeSize()
	{
		return ScopeDimension.Count - 1;
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