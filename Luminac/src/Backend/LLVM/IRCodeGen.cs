using System.Diagnostics;
using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LLVMSharp.Interop;

public class IRCodeGen
{
    public static void LLVM_Gen(List<StatementNode> statements, CompileOptions compileOptions)
    {
        var lakeAsmDir = "lumina-asm";
        var lakeBinDir = "lumina-bin";
        var lakeIrDir = "lumina-ir";

        LLVM.InitializeAllTargetInfos();
        LLVM.InitializeAllTargets();
        LLVM.InitializeAllTargetMCs();
        LLVM.InitializeAllAsmPrinters();
        LLVM.InitializeAllAsmParsers();

        var module = LLVMModuleRef.CreateWithName(Path.ChangeExtension(compileOptions.OutputFile, ".ll"));
        LLVMBuilderRef builder = module.Context.CreateBuilder();
        LLVMStatementVisitor visit = new LLVMStatementVisitor(builder, module);
        statements.ForEach(n => n.Visit(visit));

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
            OptLevel.Level3 => LLVMCodeGenOptLevel.LLVMCodeGenLevelAggressive
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
                Console.WriteLine("a.o");
                Console.WriteLine("");
                // LacusLLVM.SemanticAanylyzerVisitor.Linker.Link.LinkCode($"{lakeBinDir}/a.o");
                File.Delete($"{lakeBinDir}/a.o");
                Console.WriteLine("a.exe");
                Console.WriteLine("");
                // LacusLLVM.SemanticAanylyzerVisitor.Linker.Link.LinkCode(compileOptions.OutputFile);

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

        Console.WriteLine("Compiled successfully");
    }
}