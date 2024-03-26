using System.Collections;
using LLVMSharp.Interop;

struct ScopeDimensions
{
	public Dictionary<string, LLVMValueRef> Vars;

	public ScopeDimensions()
	{
		Vars = new Dictionary<string, LLVMValueRef>();
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
	public void AddNewVar(string name, LLVMValueRef value)
	{
		for (int i = 0; i < ScopeDimension.Count; i++)
		{
			if (ScopeDimension[i].Vars.ContainsKey(name))
			{
				throw new VaraibleAlreadyDefinedException();
			}
		}
		ScopeDimension[ScopeDimension.Count - 1].Vars.Add(name, value);
	}
	public LLVMValueRef GetNewVar(string name)
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