using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor.Backend;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class LLVMTopLevelVisitor(LLVMBuilderRef builderRef, LLVMModuleRef moduleRef) : StatementVisit
{
    private LLVMContext Context { get; set; }

    public Dictionary<string, CompilerModule> Modules { get; } = new();

    public CompilerModule CurrentModule = new();

    public override void Visit(VaraibleDeclarationNode node)
    {
        // Context.CurrentModule = CurrentModule;
        var type = Compile.ToLLVMType(node.Type, Context);
        if (node is ArrayNode n)
        {
            // var size = n.Size.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef));
            // var value = moduleRef.AddGlobal(type, node.Name.buffer);
            // Context.vars.Add(node.Name.buffer, new LLVMVar(value, type));
            // value.Initializer = 
            ///TODO: come up with a plan for arrays
            // throw new NotImplementedException();
            var value = moduleRef.AddGlobal(type, node.Name.buffer);

            value.Initializer = builderRef.BuildArrayAlloca(type,
                n.Size.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef)),
                "test");
            // LLVMTypeRef.CreateArray()
            // Context.globalVars.Add(node.Name.buffer, new LLVMVar(value, type));
            CurrentModule.Varaibles.Add(node.Name.buffer, new LLVMVar(value, type));

            // LLVMTypeRef.CreateArray
            throw new NotImplementedException();
        }
        else
        {
            var value = moduleRef.AddGlobal(type, node.Name.buffer);
            // var value = builderRef.BuildAlloca(type, node.Name.buffer);
            if (node.AttributesTuple.isExtern)
                value.Linkage = LLVMLinkage.LLVMExternalLinkage;
            CurrentModule.Varaibles.Add(node.Name.buffer, new LLVMVar(value, type));
            {
                if (node.Expression != null)
                {
                    LLVMValueRef eq = node.Expression.Visit(
                        new LLVMExprVisitor(Context, builderRef, moduleRef)
                    );
                    value.Initializer = eq;
                }
                else
                {
                    if (!node.AttributesTuple.isExtern)
                        value.Initializer = LLVMValueRef.CreateConstNull(type);
                }
            }
            // Context.globalVars.Add(node.Name.buffer, new LLVMVar(value, type));
            // {
            //     if (node.Expression != null)
            // {
            //     LLVMValueRef eq = node.Expression.Visit(
            //         new LLVMExprVisitor(Context, builderRef, moduleRef)
            //     );
            //     value.Initializer = eq;
            // }
            // else
            // {
            //     if (!node.AttributesTuple.isExtern)
            //         value.Initializer = LLVMValueRef.CreateConstNull(type);
            // }
            // }
        }
    }

    public override void Visit(FunctionNode node)
    {
        LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(
            Compile.ToLLVMType(node.RetType.Name, Context),
            node.Parameters //params
                .Select(n => Compile.ToLLVMType(n.Type, Context)) //converts param types
                .ToArray(), //to an array
            false
        );
        // node.Parameters.ForEach(n => n.Visit(this));
        LLVMValueRef function = moduleRef.AddFunction(node.Name.buffer, funcType);
        // Context.functions.Add(node.Name.buffer,
        // new LLVMFunction(funcType, Compile.ToLLVMType(node.RetType.Name, Context), function));
        function.Linkage = LLVMLinkage.LLVMExternalLinkage;
        CurrentModule.Functions.Add(node.Name.buffer,
            new LLVMFunction(funcType, Compile.ToLLVMType(node.RetType.Name, Context), function));
    }

    public override void Visit(ModuleNode node)
    {
        // CurrentModule.imports = node.Imports.Select(n => n.buffer).ToList();
        // CurrentModule.imports.Join(node.Imports => node.Imports.buffer);
        CurrentModule.ModuleNode = node;
        // Context.CurrentModule = 
        // Context.CurrentModule = CurrentModule;
        node.FunctionNodes.ForEach(n => n.Visit(this));
        node.StructNodes.ForEach(n => n.Visit(this));
        node.VaraibleDeclarationNodes.ForEach(n => n.Visit(this));
        // Console.WriteLine(Con);
        // node.StatementNodes.ForEach(n => n.Visit(this));
    }

    public override void Visit(PerenNode node)
    {
        // node.ModuleNodes.Values.ToList().ForEach(n =>
        // {
        //     n.Visit(this);
        //     
        //
        // });

        node.ModuleNodes.Values.ToList().ForEach(n =>
        {
            CurrentModule = new();
            CurrentModule.ModuleNode = n;
            Modules.Add(n.Name.buffer, CurrentModule);
        });

        Modules.Values.ToList()
            .ForEach(n =>
                n.imports = n.ModuleNode.Imports.Select(n1 => { return Modules[n1.buffer]; }).ToList());
        Context = new(Modules);
        node.Visit(new LLVMTypes(builderRef, moduleRef, Context));
        node.Visit(new LLVMFunctions(builderRef, moduleRef, Context));
        node.Visit(new LLVMVaraibles(builderRef, moduleRef, Context));
        // Modules.Values.ToList().SelectMany(n =>
        // {
        //     
        //     // n.imports.Select(n => n.buffer).ToList();
        // })

        node.ModuleNodes.Values.ToList()
            .ForEach(n => { n.Visit(new LLVMStatementVisitor(Context, builderRef, moduleRef)); });
        // base.Visit(node);
    }

    // public override void Visit(StructNode node)
    // {
    //     var llvmstruct = moduleRef.Context.CreateNamedStruct(node.Name.buffer);
    //     // Context.types.Add(node.Name.buffer, new LLVMType(
    //     //     llvmstruct, node.Vars));
    //     CurrentModule.Types.Add(node.Name.buffer, new LLVMType(
    //         llvmstruct, node.Vars));
    // }
}

// public class LLVMVaraibles(LLVMBuilderRef builderRef, LLVMModuleRef moduleRef, LLVMContext context) : StatementVisit
// {
//     public CompilerModule CurrentModule = new();
//
//     public override void Visit(VaraibleDeclarationNode node)
//     {
//         var type = Compile.ToLLVMType(node.Type, context);
//         if (node is ArrayNode n)
//         {
//             // var size = n.Size.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef));
//             // var value = moduleRef.AddGlobal(type, node.Name.buffer);
//             // Context.vars.Add(node.Name.buffer, new LLVMVar(value, type));
//             // value.Initializer = 
//             ///TODO: come up with a plan for arrays
//             // throw new NotImplementedException();
//             var value = moduleRef.AddGlobal(type, node.Name.buffer);
//
//             value.Initializer = builderRef.BuildArrayAlloca(type,
//                 n.Size.Visit(new LLVMExprVisitor(context, builderRef, moduleRef)),
//                 "test");
//             // LLVMTypeRef.CreateArray()
//             // Context.globalVars.Add(node.Name.buffer, new LLVMVar(value, type));
//             CurrentModule.Varaibles.Add(node.Name.buffer, new LLVMVar(value, type));
//
//             // LLVMTypeRef.CreateArray
//             throw new NotImplementedException();
//         }
//         else
//         {
//             var value = moduleRef.AddGlobal(type, node.Name.buffer);
//             // var value = builderRef.BuildAlloca(type, node.Name.buffer);
//             if (node.AttributesTuple.isExtern)
//                 value.Linkage = LLVMLinkage.LLVMExternalLinkage;
//             CurrentModule.Varaibles.Add(node.Name.buffer, new LLVMVar(value, type));
//             {
//                 if (node.Expression != null)
//                 {
//                     LLVMValueRef eq = node.Expression.Visit(
//                         new LLVMExprVisitor(context, builderRef, moduleRef)
//                     );
//                     value.Initializer = eq;
//                 }
//                 else
//                 {
//                     if (!node.AttributesTuple.isExtern)
//                         value.Initializer = LLVMValueRef.CreateConstNull(type);
//                 }
//             }
//         }
//     }

//     public override void Visit(ModuleNode moduleNode)
//     {
//         moduleNode.VaraibleDeclarationNodes.ForEach(n => n.Visit(this));
//     }
//
//     public override void Visit(PerenNode node)
//     {
//         node.ModuleNodes.Values.ToList().ForEach(n =>
//         {
//             CurrentModule = context.SetCurrent(n.Name.buffer);
//             n.Visit(this);
//         });
//     }
// }