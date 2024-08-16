using System.Diagnostics;
using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LLVMSharp.Interop;


public class IRCodeGen
{
    public static void LLVM_Gen(PerenNode statements, CompileOptions compileOptions)
    {
        var asmOutDir = "peren-asm";
        var binOutDir = "peren-bin";
        var irOutDir = "peren-ir";

        LLVM.InitializeAllTargetInfos();
        LLVM.InitializeAllTargets();
        LLVM.InitializeAllTargetMCs();
        LLVM.InitializeAllAsmPrinters();
        LLVM.InitializeAllAsmParsers();

        var module = LLVMModuleRef.CreateWithName(Path.ChangeExtension(statements.GetStart().Name.buffer, ".ll"));
        LLVMBuilderRef builder = module.Context.CreateBuilder();
        var visit = new LLVMTopLevelVisitor(builder, module);
        statements.Visit(visit);

        //outputting directly to an object file
        //https://llvm.org/docs/tutorial/MyFirstLanguageFrontend/LangImpl08.html
        var targetTriple = LLVMTargetRef.DefaultTriple;
        var target = LLVMTargetRef.GetTargetFromTriple(targetTriple);
        var cpu = compileOptions.TargetArchitechure;
        var features = "";
        var opt = compileOptions.OptLevel switch
        {
            OptLevel.Level0 => LLVMCodeGenOptLevel.LLVMCodeGenLevelNone,
            OptLevel.Level1 => LLVMCodeGenOptLevel.LLVMCodeGenLevelLess,
            OptLevel.Level2 => LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault,
            OptLevel.Level3 => LLVMCodeGenOptLevel.LLVMCodeGenLevelAggressive,
            _ => throw new Exception("o.O")
        };
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
                if (!Directory.Exists(binOutDir))
                    Directory.CreateDirectory(binOutDir);
                var out_string = "";
                targetMachine.TryEmitToFile(
                    module,
                    $"{binOutDir}/a.o",
                    LLVMCodeGenFileType.LLVMObjectFile,
                    out out_string
                );
                Process link = new Process();
                link.StartInfo.FileName = "ld";
                link.StartInfo.Arguments = $"{binOutDir}/a.o -o {compileOptions.OutputFile}";
                link.Start();
                link.WaitForExit();

                File.Delete($"{binOutDir}/a.o");

                Directory.Delete(binOutDir);
            }
            else
            {
                var out_string = "";

                // targetMachine.TryToEmitTo
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
            if (!Directory.Exists(irOutDir))
                Directory.CreateDirectory(irOutDir);
            File.WriteAllText(
                $"{irOutDir}/{Path.ChangeExtension(compileOptions.OutputFile, ".ll")}",
                module.ToString()
            );
        }

        if (compileOptions.AssemblyFile)
        {
            if (!Directory.Exists(asmOutDir))
                Directory.CreateDirectory(asmOutDir);
            var out_string = "";
            targetMachine.TryEmitToFile(
                module,
                $"{asmOutDir}/{Path.ChangeExtension(compileOptions.OutputFile, ".s")}",
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
                $"LLVM-IR file path: {irOutDir}/{Path.ChangeExtension(compileOptions.OutputFile, ".ll")}"
            );
        if (compileOptions.AssemblyFile)
            Console.WriteLine(
                $"Assembly file file path: {asmOutDir}/{Path.ChangeExtension(compileOptions.OutputFile, ".s")}"
            );


        Console.WriteLine("Compiled successfully");
    }
}