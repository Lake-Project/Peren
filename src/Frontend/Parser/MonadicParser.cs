using LacusLLVM.Frontend.Parser.AST;
using LLVMSharp;
using Microsoft.CSharp.RuntimeBinder;
using Type = LLVMSharp.Type;

namespace Lexxer.Parser;

public class MonadicParser(List<Tokens> tokensList)
{
    public Maybe<INode> Factor()
    {
        return new Maybe<INode>(tokensList[0])
            .Bind(
                TokenType.NUMBER,
                tokensList,
                (type, m) =>
                {
                    Console.WriteLine("Factor.Number");
                    if (type.buffer.Contains("."))
                        return new FloatNode(type);
                    return new IntegerNode(type);
                }
            )
            .Bind(
                TokenType.WORD,
                tokensList,
                (Type, m) =>
                {
                    if (m.LookAhead(TokenType.OP_PAREN, tokensList))
                    {
                        return ParseFuncCall(Type).Value;
                    }

                    return new VaraibleReferenceNode(Type);
                }
            )
            .Bind(
                TokenType.OP_PAREN,
                tokensList,
                (type, m) =>
                {
                    Maybe<INode> ex = Expression().RemoveList(TokenType.CL_PAREN, tokensList);
                    return ex.Value;
                }
            );
    }

    public Maybe<INode> BoolExpr()
    {
        Func<Maybe<INode>, INode> nothing = (m) => m.Value;
        return Factor()
            .Bind(
                TokenType.GT,
                tokensList,
                ((tokens, maybe) => new BooleanExprNode(maybe.Value, Factor().Value, tokens))
            )
            .Bind(
                TokenType.LT,
                tokensList,
                ((tokens, maybe) => new BooleanExprNode(maybe.Value, Factor().Value, tokens))
            )
            .Bind(
                TokenType.GTE,
                tokensList,
                ((tokens, maybe) => new BooleanExprNode(maybe.Value, Factor().Value, tokens))
            )
            .Bind(
                TokenType.LTE,
                tokensList,
                ((tokens, maybe) => new BooleanExprNode(maybe.Value, Factor().Value, tokens))
            )
            .Bind(
                TokenType.BOOL_EQ,
                tokensList,
                ((tokens, maybe) => new BooleanExprNode(maybe.Value, Factor().Value, tokens)),
                nothing
            );
    }

    public Maybe<INode> Term()
    {
        Func<Maybe<INode>, INode> nothing = (m) => m.Value;
        return BoolExpr()
            .Bind(
                TokenType.MULTIPLICATION,
                tokensList,
                (type, m) => new OpNode(m.Value, Term().Value, type)
            )
            .Bind(
                TokenType.DIVISION,
                tokensList,
                (type, m) => new OpNode(m.Value, Term().Value, type)
            )
            .Bind(
                TokenType.MODULAS,
                tokensList,
                (type, m) => new OpNode(m.Value, Term().Value, type)
            )
            .Bind(
                TokenType.L_SHIFT,
                tokensList,
                (type, m) => new OpNode(m.Value, Term().Value, type)
            )
            .Bind(
                TokenType.R_SHIFT,
                tokensList,
                (type, m) => new OpNode(m.Value, Term().Value, type)
            )
            .Bind(TokenType.AND, tokensList, (type, m) => new OpNode(m.Value, Term().Value, type))
            .Bind(
                TokenType.OR,
                tokensList,
                (type, m) => new OpNode(m.Value, Term().Value, type),
                nothing
            );
    }

    public Maybe<INode> Expression()
    {
        Func<Maybe<INode>, INode> nothing = (m) => m.Value;
        return Term()
            .Bind(
                TokenType.ADDITION,
                tokensList,
                (type, parser) => new OpNode(parser.Value, Expression().Value, type)
            )
            .Bind(
                TokenType.SUBTRACTION,
                tokensList,
                (type, parser) => new OpNode(parser.Value, Expression().Value, type),
                nothing
            );
    }

    public Maybe<VaraibleDeclarationNode> ParseVar(Tokens type)
    {
        return new Maybe<VaraibleDeclarationNode>(tokensList[0]).Bind(
            TokenType.WORD,
            tokensList,
            (tokens, maybe) =>
            {
                return new VaraibleDeclarationNode(
                    type,
                    tokens,
                    ParseExpression(TokenType.EQUALS).Value,
                    false
                );
            }
        );
    }

    public Maybe<bool> isExtern()
    {
        return new Maybe<bool>(tokensList[0]).Bind(
            TokenType.EXTERN,
            tokensList,
            (tokens, maybe) => true,
            (tokens) => false
        );
    }

    private Maybe<Tokens> ParseReturn()
    {
        return new Maybe<Tokens>(tokensList[0]).Bind(
            TokenType.RETURNS,
            tokensList,
            (tokens, maybe) => ParseType().Value,
            (tokens) => new Tokens(TokenType.VOID)
        );
    }

    private Maybe<Tokens> ParseType()
    {
        return new Maybe<Tokens>(tokensList[0])
            .Bind(TokenType.INT, tokensList, (tokens, maybe) => tokens)
            .Bind(TokenType.FLOAT, tokensList, (tokens, maybe) => tokens)
            .Bind(TokenType.WORD, tokensList, (tokens, maybe) => tokens)
            .Bind(TokenType.CHAR, tokensList, (tokens, maybe) => tokens)
            .Bind(TokenType.BOOL, tokensList, (tokens, maybe) => tokens);
    }

    private Maybe<List<VaraibleDeclarationNode>> ParseParameters()
    {
        return new Maybe<List<VaraibleDeclarationNode>>(tokensList[0]).Bind(
            TokenType.OP_PAREN,
            tokensList,
            (tokens1, maybe1) =>
            {
                List<VaraibleDeclarationNode> v = new();
                return v;
            }
        );
    }

    private Maybe<FunctionNode> ParseFunctions()
    {
        return new Maybe<FunctionNode>(tokensList[0]).Bind(
            TokenType.WORD,
            tokensList,
            (
                (tokens, maybe) =>
                {
                    bool isExtern = this.isExtern().Value;
                    Tokens name = tokens;
                    List<VaraibleDeclarationNode> v = ParseParameters().Value;
                    Tokens retType = ParseReturn().Value;
                    // return new FunctionNode(name,
                    //     ParseParameters().Value,);
                    throw new NotImplementedException();
                }
            )
        );
    }

    public Maybe<FunctionCallNode> ParseFuncCall(Tokens name)
    {
        throw new NotImplementedException();
    }

    public Maybe<VaraibleReferenceStatementNode> ParseVarRef(Tokens name)
    {
        throw new NotImplementedException();
    }

    public Maybe<INode> ParseExpression(TokenType ExprIdentfier)
    {
        return new Maybe<INode>(tokensList[0]).Bind(
            ExprIdentfier,
            tokensList,
            (tokens, maybe) => Expression().Value
        );
    }

    // private StatementNode ParseFunctions()
    // {
    //     Maybe<Tokens> name = new Maybe<Tokens>(tokensList[0]).Bind(
    //         TokenType.WORD,
    //         tokensList,
    //         (tokens, maybe) => tokens,
    //         (type) => throw new Exception("function needs name")
    //     );
    //
    //     Maybe<List<VaraibleDeclarationNode>> Params = new Maybe<List<VaraibleDeclarationNode>>(
    //         tokensList[0]
    //     ).Bind(
    //         TokenType.OP_PAREN,
    //         tokensList,
    //         (tokens, maybe) =>
    //         {
    //             List<VaraibleDeclarationNode> n = new();
    //
    //             while (
    //                 new Maybe<bool>(tokensList[0])
    //                     .Bind(
    //                         TokenType.CL_PAREN,
    //                         tokensList,
    //                         (tokens1, maybe1) => true,
    //                         maybe1 => false
    //                     )
    //                     .Value
    //             )
    //             {
    //                 n.Add(
    //                     new Maybe<VaraibleDeclarationNode>(tokensList[0])
    //                         .Bind(TokenType.INT, tokensList, (tokens1, maybe1) => ParseVar(tokens1))
    //                         .Bind(
    //                             TokenType.FLOAT,
    //                             tokensList,
    //                             (tokens1, maybe1) => ParseVar(tokens1)
    //                         )
    //                         .Bind(
    //                             TokenType.CHAR,
    //                             tokensList,
    //                             (tokens1, maybe1) => ParseVar(tokens1)
    //                         )
    //                         .Bind(
    //                             TokenType.BOOL,
    //                             tokensList,
    //                             (tokens1, maybe1) => ParseVar(tokens1)
    //                         )
    //                         .Bind(
    //                             TokenType.WORD,
    //                             tokensList,
    //                             (tokens1, maybe1) => ParseVar(tokens1)
    //                         )
    //                         .Value
    //                 );
    //             }
    //
    //             return n;
    //         }
    //     );
    //     // return new FunctionNode(name.Value, Params.Value);
    //     throw new NotImplementedException();
    // }

    public Maybe<StatementNode> ParseGlobals()
    {
        return new Maybe<StatementNode>(tokensList[0]).Bind(
            TokenType.FUNCTION,
            tokensList,
            (tokens, maybe) => ParseFunctions().Value
        );
    }

    public Maybe<StatementNode> ParseStatements()
    {
        return new Maybe<StatementNode>(tokensList[0]).Bind(
            TokenType.WORD,
            tokensList,
            (tokens, maybe) =>
            {
                if (maybe.LookAhead(TokenType.WORD, tokensList))
                {
                    return ParseVarRef(tokens).Value;
                }
                else
                {
                    ParseFunctions();
                }

                return null;
            }
        );
    }

    public Maybe<INode> Parse()
    {
        return Expression();
    }
}
