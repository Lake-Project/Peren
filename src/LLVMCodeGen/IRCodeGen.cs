using System.Diagnostics;
using LLVMSharp;
using LLVMSharp.Interop;

public class IRCodeGen
{
    private static void LLVMCreateFile(LLVMModuleRef module, string file)
    {
        // Specify the directory path
        string directoryPath = "out";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (file == "")
        {
            file = "output.ll";
        }

        string filePath = Path.Combine(directoryPath, file);
        File.WriteAllText(filePath, module.ToString());
        Console.WriteLine("code successfully compiled");
        module.Dispose();
    }

    public static void LLVM_Gen(List<INode?> statements, CompileOptions compileOptions)
    {
        var module = LLVMModuleRef.CreateWithName("main");
        LLVMBuilderRef builder = module.Context.CreateBuilder();
        Context c = new();
        // Context context = new CO
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        foreach (INode? statement in statements)
            statement.CodeGen(new CodeGenVisitor(), builder, module, c);
        // string directoryPath = "out";
        // if (!Directory.Exists(directoryPath))
        // {
        //     Directory.CreateDirectory(directoryPath);
        // }

        // string filePath = Path.Combine(directoryPath, compileOptions.OutputFile);
        // if (compileOptions.IrFile)
        //     File.WriteAllText(filePath, module.ToString());
        // Console.WriteLine("code successfully compiled");
        // Console.WriteLine("IR code gen file path: " + filePath);

        //outputting directly to an object file
        //https://llvm.org/docs/tutorial/MyFirstLanguageFrontend/LangImpl08.html
        var targetTriple = LLVMTargetRef.DefaultTriple;
        var target = LLVMTargetRef.GetTargetFromTriple(targetTriple);
        var cpu = "generic";
        var features = "";
        var opt = compileOptions.OptLevel;
        var targetMachine = target.CreateTargetMachine(
            targetTriple,
            cpu,
            features,
            opt,
            LLVMRelocMode.LLVMRelocPIC,
            LLVMCodeModel.LLVMCodeModelLarge
        );
        var out_string = "";
        if (!Directory.Exists("lacus-bin"))
            Directory.CreateDirectory("lacus-bin");
        targetMachine.TryEmitToFile(
            module,
            "lacus-bin/a.o",
            LLVMCodeGenFileType.LLVMObjectFile,
            out out_string
        );
        Process link = new Process();
        link.StartInfo.FileName = "ld";
        link.StartInfo.Arguments = $"lacus-bin/a.o -o {compileOptions.OutputFile}";
        link.Start();
        link.WaitForExit();
        File.Delete("lacus-bin/a.o");
        Directory.Delete("lacus-bin");
        // builder.BuildFPToSI
        // builder.BuildTrunc()

        // new FloatNode().Accept<FloatNode>(new FloatExprVis(), builder, module);
        // LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, new LLVMTypeRef[0] { }, false);
        // LLVMValueRef function = module.AddFunction("main", funcType);
        // LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
        // builder.PositionAtEnd(entry);
        // builder.BuildRet(expr.Accept(builder, module));
        // Console.WriteLine(module.ToString());
    }
}
