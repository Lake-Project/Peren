using System.Diagnostics;
using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LLVMSharp.Interop;

public class IRCodeGen
{
    public static void LLVM_Gen(List<StatementNode> statements, CompileOptions compileOptions)
    {
        LLVM.InitializeAllTargetInfos();
        LLVM.InitializeAllTargets();
        LLVM.InitializeAllTargetMCs();
        LLVM.InitializeAllAsmPrinters();
        LLVM.InitializeAllAsmParsers();

        var module = LLVMModuleRef.CreateWithName("main");
        LLVMBuilderRef builder = module.Context.CreateBuilder();
        LLVMStatementVisitor visit = new LLVMStatementVisitor(builder, module);
        statements.ForEach(n => n.Visit(visit));

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
            LLVMCodeModel.LLVMCodeModelMedium
        );
        if (!compileOptions.CompileOff)
        {
            if (!compileOptions.CompileOnly)
            {
                if (!Directory.Exists("lacus-bin"))
                    Directory.CreateDirectory("lacus-bin");
                var out_string = "";
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
            }
            else
            {
                var out_string = "";
                targetMachine.TryEmitToFile(
                    module,
                    $"{compileOptions.OutputFile}",
                    LLVMCodeGenFileType.LLVMObjectFile,
                    out out_string
                );
            }
        }

        //
        if (compileOptions.IrFile)
        {
            if (!Directory.Exists("lacus-IR"))
                Directory.CreateDirectory("lacus-IR");
            File.WriteAllText(
                "lacus-IR/" + Path.ChangeExtension(compileOptions.OutputFile, ".ll"),
                module.ToString()
            );
        }

        if (compileOptions.AssemblyFile)
        {
            if (!Directory.Exists("lacus-Assembly"))
                Directory.CreateDirectory("lacus-Assembly");
            var out_string = "";
            targetMachine.TryEmitToFile(
                module,
                $"lacus-assembly/{compileOptions.OutputFile}",
                LLVMCodeGenFileType.LLVMAssemblyFile,
                out out_string
            );
        }

        if (compileOptions.PrintIR)
            module.Dump();
        if (!compileOptions.CompileOff)
            if (compileOptions.CompileOnly)
                Console.WriteLine($"Object output path: {compileOptions.OutputFile} ");
            else
                Console.WriteLine($"executable output path: {compileOptions.OutputFile} ");
        if (compileOptions.IrFile)
            Console.WriteLine(
                $"LLVM-IR file path: lacus-IR/{Path.ChangeExtension(compileOptions.OutputFile, ".ll")}"
            );
        if (compileOptions.AssemblyFile)
            Console.WriteLine(
                $"Assembly file file path: lacus-Assembly/{Path.ChangeExtension(compileOptions.OutputFile, ".s")}"
            );

        Console.WriteLine("Compiled sucessfully");
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
