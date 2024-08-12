using System.Text.RegularExpressions;
using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;

namespace PerenTests;

public class ParseTests
{
    private PerenNode p;

    public static string[] getFile(string code)
    {
        string pattern = @"([{};])";
        string[] parts = Regex.Split(code, pattern);
        return parts;
    }

    [SetUp]
    public void Setup()
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
            n := 10;
            return 0;
    
        }
    }";

        // p = new Parse(tokensList).ParseFile();
    }

    [Test]
    public void TestModuleParsing()
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
            f := 10;
            f := 10.2;
            return 0;
    
        }
    }";
        var list = new List<Tokens>();
        new LexTokens().LexList(getFile(code), list);

        var p = new Parse(list).ParseFile();
        p.ModuleNodes.Keys.ToList().ForEach(n => { Assert.That(n, Is.EqualTo("Factorial")); });
        Assert.Throws<TypeMisMatchException>(() => SemanticAnaylsis.init(p));
    }

    // [Test]
}