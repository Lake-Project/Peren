using System.Diagnostics;
using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LLVMSharp.Interop;

public class IRCodeGen
{
    public static void LLVM_Gen(List<StatementNode> statements, CompileOptions compileOptions)
    {

        var lakeAsmDir = "lake-asm";
        var lakeBinDir = "lake-bin";
        var lakeIrDir = "lake-ir";

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
        var cpu = compileOptions.targetArchitechure;
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
                if (!Directory.Exists(lakeBinDir))
                    Directory.CreateDirectory(lakeBinDir);
                var out_string = "";
                targetMachine.TryEmitToFile(
                    module,
                    $"{lakeBinDir}/a.o",
                    LLVMCodeGenFileType.LLVMObjectFile,
                    out out_string
                );
                Process link = new Process();
                link.StartInfo.FileName = "ld";
                link.StartInfo.Arguments = $"{lakeBinDir}/a.o -o {compileOptions.OutputFile}";
                link.Start();
                link.WaitForExit();
                File.Delete($"{lakeBinDir}/a.o");
                Directory.Delete(lakeBinDir);
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
            if (!Directory.Exists(lakeIrDir))
                Directory.CreateDirectory(lakeIrDir);
            File.WriteAllText(
                $"{lakeIrDir}/{Path.ChangeExtension(compileOptions.OutputFile, ".ll")}",
                module.ToString()
            );
        }

        if (compileOptions.AssemblyFile)
        {
            if (!Directory.Exists(lakeAsmDir))
                Directory.CreateDirectory(lakeAsmDir);
            var out_string = "";
            targetMachine.TryEmitToFile(
                module,
                $"{lakeAsmDir}/{Path.ChangeExtension(compileOptions.OutputFile, ".s")}",
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
                $"LLVM-IR file path: {lakeIrDir}/{Path.ChangeExtension(compileOptions.OutputFile, ".ll")}"
            );
        if (compileOptions.AssemblyFile)
            Console.WriteLine(
                $"Assembly file file path: {lakeAsmDir}/{Path.ChangeExtension(compileOptions.OutputFile, ".s")}"
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
