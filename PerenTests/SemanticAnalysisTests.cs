using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;

namespace PerenTests;

public class SemanticAnalysisTests
{
    /// <summary>
    /// Tests for forbidden assignments
    /// </summary>
    [Test]
    public void TestForbiddenAssignment()
    {
        string code = @"
       
        module Factorial{
        fn Factorial(int n) returns int
        {
            if (n == 1) {
                return 1;
            }
  
            return n * Factorial(n - 1);
        }
        fn main() returns int{
            int f := Factorial(10);
            f := 10.2;
            return 0;
    
        }
    }";


        var list = new List<Tokens>();
        new LexTokens().LexList(ParseTests.getFile(code), list);

        var p = new Parse(list).ParseFile();
        Assert.Throws<TypeMisMatchException>(() => SemanticAnaylsis.init(p));
    }

    /// <summary>
    /// Tests to see if range works
    /// </summary>
    [Test]
    public void TestIntegerRange()
    {
        string code = @"
       
        module TestIntegerRange{
        fn main() returns int{
            sbyte a := 1;
            byte b := 1;
            int16 c := 1;
            uint16 d := 1;
            int e := 1;
            uint f := 1;
            int64 g := 1;
            uint64 h := 1;     
        }
    }";


        var list = new List<Tokens>();
        new LexTokens().LexList(ParseTests.getFile(code), list);

        var p = new Parse(list).ParseFile();
        SemanticAnaylsis.init(p);

        var exprs = p.ModuleNodes["TestIntegerRange"].FunctionNodes[0].Statements
            .Select(n => (IntegerNode)((VaraibleDeclarationNode)n).Expression!)
            .ToList();
        Range[] range =
        [
            Range.EightBit, Range.EightBit, Range.SixteenBit, Range.SixteenBit, Range.ThirtyTwoBit, Range.ThirtyTwoBit,
            Range.SixtyFourBit, Range.SixtyFourBit
        ];
        exprs
            .Zip(range, (expr, range) => new { Expr = expr, Range = range })
            .ToList()
            .ForEach(n =>
            {
                Assert.That(n.Range, Is.EqualTo(n.Expr.Range));
                // Assert.Equals(n.Expr.Range, range);
            });
    }

    /// <summary>
    /// Tests for forbidden casts
    /// </summary>
    [Test]
    public void TestForbiddenCasts()
    {
        string code = @"
       
        module TestCasts{
        fn main() returns int{
            int a := (float)1;  
        }
    }";
        var list = new List<Tokens>();
        new LexTokens().LexList(ParseTests.getFile(code), list);

        var p = new Parse(list).ParseFile();
        Assert.Throws<TypeMisMatchException>(() => SemanticAnaylsis.init(p));
    }

    [Test]
    public void TestsCasts()
    {
        string code = @"
       
        module TestCasts{
        fn main() returns int{
            int a := 1;
            byte b := (byte) a;  
            int d := (int) b;
            float e := (float) a;
            float f := (int) e;
               
        }
    }";
        var list = new List<Tokens>();
        new LexTokens().LexList(ParseTests.getFile(code), list);

        var p = new Parse(list).ParseFile();
        var exprs = p.ModuleNodes["TestCasts"].FunctionNodes[0].Statements
            .Select(n => ((VaraibleDeclarationNode)n).Expression!)
            .ToList();
        SemanticAnaylsis.init(p);
        CastNode e = (CastNode)exprs[1];
        Assert.That(e.inferredtype, Is.EqualTo(CastType.TRUNCATE));
        CastNode f = (CastNode)exprs[2];
        Assert.That(f.inferredtype, Is.EqualTo(CastType.SEXT));
        CastNode g = (CastNode)exprs[3];
        Assert.That(g.inferredtype, Is.EqualTo(CastType.INT));
        CastNode h = (CastNode)exprs[4];
        Assert.That(h.inferredtype, Is.EqualTo(CastType.FLOAT));
    }
}