using LanguageExt;
using Microsoft.Z3;
using System.Reflection;
using static ScenarioModelling.Analysis.Tests.Z3.MicrosoftZ3Examples;

namespace ScenarioModelling.Analysis.Tests.Z3Examples;

/// <summary>
/// https://github.com/Z3Prover/z3
/// https://smt.st/SAT_SMT_by_example.pdf
/// https://github.com/Z3Prover/z3/wiki/Slides
/// https://fstar-lang.org/tutorial/book/under_the_hood/uth_smt.html#understanding-how-f-uses-z3
/// https://z3prover.github.io/papers/z3internals.html
/// https://z3prover.github.io/papers/programmingz3.html
/// </summary>
[TestClass]
public sealed class Z3Tests
{
    public void LinearAlgebra_TwoEquations(Context ctx)
    {
        IntExpr x = ctx.MkIntConst("x");
        IntExpr y = ctx.MkIntConst("y");

        Solver solver = ctx.MkSolver();

        // Constraints: x + y = 10, x - y = 2
        solver.Add(ctx.MkEq(ctx.MkAdd(x, y), ctx.MkInt(10)));
        solver.Add(ctx.MkEq(ctx.MkSub(x, y), ctx.MkInt(2)));

        int solutionCount = 0;

        while (solver.Check() == Status.SATISFIABLE)
        {
            Model model = solver.Model;
            int xVal = ((IntNum)model.Evaluate(x)).Int;
            int yVal = ((IntNum)model.Evaluate(y)).Int;

            // Print the found solution
            Console.WriteLine($"Solution {++solutionCount}: x = {xVal}, y = {yVal}");

            // Add a constraint to block this specific solution
            solver.Add(ctx.MkOr(ctx.MkNot(ctx.MkEq(x, ctx.MkInt(xVal))),
                                ctx.MkNot(ctx.MkEq(y, ctx.MkInt(yVal)))));
        }

        if (solutionCount == 0)
            Console.WriteLine("No solutions found.");

        Console.WriteLine(solver.Statistics);

    }

    public void SystemOfEquations_TwoEquations_Quadratic(Context _)
    {
        IntExpr x = _.MkIntConst("x");
        IntExpr y = _.MkIntConst("y");

        Solver solver = _.MkSolver();

        // Constraints: x² + y = 10, x - y = 2
        solver.Add(_.MkEq(_.MkAdd(_.MkMul(x, x), y), _.MkInt(10)));
        solver.Add(_.MkEq(_.MkSub(x, y), _.MkInt(2)));

        int solutionCount = 0;
        List<(int, int)> slns = new();

        while (solver.Check() == Status.SATISFIABLE)
        {
            Model model = solver.Model;
            int xVal = ((IntNum)model.Evaluate(x)).Int;
            int yVal = ((IntNum)model.Evaluate(y)).Int;

            // Print the found solution
            slns.Add((xVal, yVal));
            Console.WriteLine($"Solution {++solutionCount}: x = {xVal}, y = {yVal}");

            // Add a constraint to block this specific solution
            solver.Add(_.MkOr(_.MkNot(_.MkEq(x, _.MkInt(xVal))),
                                _.MkNot(_.MkEq(y, _.MkInt(yVal)))));
        }

        foreach((int xVal, int yVal) in slns)
        {
            Assert.AreEqual(10, xVal * xVal + yVal);
            Assert.AreEqual(2, xVal - yVal);
        }

        if (solutionCount == 0)
            Console.WriteLine("No solutions found.");

        Console.WriteLine(solver.Statistics);

    }

    //public void SystemOfEquations_TwoEquations_Quadratic_OverReals(Context ctx)
    //{
    //    RealExpr x = ctx.MkRealConst("x");
    //    RealExpr y = ctx.MkRealConst("y");

    //    Solver solver = ctx.MkSolver();

    //    // Constraints: x² + y = 10, x - y = 2
    //    solver.Add(ctx.MkEq(ctx.MkAdd(ctx.MkMul(x, x), y), ctx.MkReal(10, 11)));
    //    solver.Add(ctx.MkEq(ctx.MkSub(x, y), ctx.MkInt(2)));

    //    int solutionCount = 0;
    //    List<(double, double)> slns = new();

    //    while (solver.Check() == Status.SATISFIABLE)
    //    {
    //        Model model = solver.Model;
    //        double xVal = double.Parse(((AlgebraicNum)model.Evaluate(x)).ToDecimal(5));
    //        double yVal = double.Parse(((AlgebraicNum)model.Evaluate(y)).ToDecimal(5));

    //        // Print the found solution
    //        slns.Add((xVal, yVal));
    //        Console.WriteLine($"Solution {++solutionCount}: x = {xVal}, y = {yVal}");

    //        // Add a constraint to block this specific solution
    //        solver.Add(ctx.MkOr(ctx.MkNot(ctx.MkEq(x, ctx.MkReal(((AlgebraicNum)model.Evaluate(x)).ToDecimal(5)))),
    //                            ctx.MkNot(ctx.MkEq(y, ctx.MkReal(((AlgebraicNum)model.Evaluate(y)).ToDecimal(5))))));
    //    }

    //    foreach ((double xVal, double yVal) in slns)
    //    {
    //        Assert.AreEqual(10, xVal * xVal + yVal);
    //        Assert.AreEqual(2, xVal - yVal);
    //    }

    //    if (solutionCount == 0)
    //        Console.WriteLine("No solutions found.");
    //}

    /// <summary>
    /// From https://z3prover.github.io/papers/programmingz3.html#sec-logical-interface
    /// </summary>
    /// <param name="ctx"></param>
    public void ProgrammingZ3_3_1_2_EUF_Models(Context _)
    {
        Sort S = _.MkUninterpretedSort("S");

        Expr a = _.MkConst("a", S);
        Expr b = _.MkConst("b", S);
        Expr c = _.MkConst("c", S);
        Expr d = _.MkConst("d", S);
        Expr e = _.MkConst("e", S);
        Expr s = _.MkConst("s", S);
        Expr t = _.MkConst("t", S);

        FuncDecl f = _.MkFuncDecl("f", [S, S], S);
        FuncDecl g = _.MkFuncDecl("g", S, S);

        Solver solver = _.MkSolver();

        solver.Add(_.MkEq(a, b));
        solver.Add(_.MkEq(b, c));
        solver.Add(_.MkEq(d, e));
        solver.Add(_.MkEq(b, s));
        solver.Add(_.MkEq(d, t));
        solver.Add(_.MkNot(_.MkEq(f.Apply(a, g.Apply(d)), f.Apply(g.Apply(e), b))));

        Assert.AreEqual(Status.SATISFIABLE, solver.Check());
        
        Model model = solver.Model;

        // Get the result
        Console.WriteLine($"a = {model.Evaluate(a)}");
        Console.WriteLine($"b = {model.Evaluate(b)}");
        Console.WriteLine($"c = {model.Evaluate(c)}");
        Console.WriteLine($"d = {model.Evaluate(d)}");
        Console.WriteLine($"e = {model.Evaluate(e)}");
        Console.WriteLine($"s = {model.Evaluate(s)}");
        Console.WriteLine($"t = {model.Evaluate(t)}");
        Console.WriteLine($"f = {model.FuncInterp(f)}");
        Console.WriteLine($"g = {model.FuncInterp(g)}");
        Console.WriteLine($"S = {model.SortUniverse(S).Select(s => s.ToString()).CommaSeparatedList()}");

        Console.WriteLine(solver.Statistics);


    }

    /// <summary>
    /// From https://z3prover.github.io/papers/programmingz3.html#sec-logical-interface
    /// </summary>
    /// <param name="_"></param>
    public void ProgrammingZ3_3_5_Algebraic_Datatypes(Context _)
    {
        DatatypeSort tree = _.MkDatatypeSort("Tree",
                [
                    _.MkConstructor("Empty", "isEmpty"),
                    _.MkConstructor("Node", "isNode",
                        ["left", "data", "right"],
                        [null, _.IntSort, null],
                        [0, 0, 0]
                        )
                ]);

        Expr t = _.MkConst("t", tree);

        Solver solver = _.MkSolver();

        Expr empty = tree.Constructors[0].Apply();

        // It may produce the solution
        // solve(t != Tree.Empty)
        solver.Check(_.MkNot(_.MkEq(t, empty)));
        // [t = Node(Empty, 0, Empty)] ?
        Model model = solver.Model;
        Console.WriteLine($"t = {model.Evaluate(t)}"); // => t = (Node Empty 2 Empty)

        // Similarly, one can prove that a tree cannot be a part of itself.
        // prove(t != Tree.Node(t, 0, t))
        Expr data_zero = tree.Constructors[1].Apply([t, _.MkInt(0), t]);
        solver.Assert(_.MkNot(_.MkEq(t, empty)));

        Console.WriteLine(solver.Statistics);

    }

    /// <summary>
    /// From https://z3prover.github.io/papers/programmingz3.html#sec-logical-interface
    /// </summary>
    /// <param name="_"></param>
    public void ProgrammingZ3_3_6_Sequences_And_Strings(Context _)
    {
        var s = _.MkString("s");
        var t = _.MkString("t");
        var u = _.MkString("u");

        Solver solver = _.MkSolver();
        
        // If the lengths of a prefix and suffix of a string add up to the length of the string,
        // the string itself must be the concatenation of the prefix and suffix
        solver.Assert(_.MkImplies(_.MkAnd(_.MkPrefixOf(s, t), _.MkSuffixOf(u, t), _.MkEq(_.MkLength(t), _.MkAdd(_.MkLength(s), _.MkLength(u)))),
                                    _.MkEq(t, _.MkConcat(s, u))));

        // One can concatenate single elements to a sequence as units
        var x = (SeqExpr)_.MkConst("x", _.MkSeqSort(_.IntSort));
        var y = (SeqExpr)_.MkConst("y", _.MkSeqSort(_.IntSort));

        solver.Add(_.MkEq(_.MkConcat(x, _.MkUnit(_.MkInt(2))), _.MkConcat(_.MkUnit(_.MkInt(1)), y)));
        solver.Add(_.MkNot(_.MkEq(_.MkLength(x), _.MkInt(0))));
        solver.Add(_.MkNot(_.MkEq(_.MkLength(y), _.MkInt(0))));
        var result = solver.Check();

        Assert.AreEqual(Status.SATISFIABLE, result);

        int solutionCount = 0;
        List<(SeqExpr, SeqExpr)> slns = new();

        while (solver.Check() == Status.SATISFIABLE && solutionCount < 20)
        {
            Model model = solver.Model;
            SeqExpr xVal = ((SeqExpr)model.Evaluate(x));
            SeqExpr yVal = ((SeqExpr)model.Evaluate(y));

            // Print the found solution
            slns.Add((xVal, yVal));
            Console.WriteLine($"Solution {++solutionCount}: x = {xVal}, y = {yVal}");

            // Add a constraint to block this specific solution
            solver.Add(_.MkOr(_.MkNot(_.MkEq(x, xVal)),
                                _.MkNot(_.MkEq(y, yVal))));
        }

        Console.WriteLine(solver.Statistics);

        if (solutionCount == 0)
            Console.WriteLine("No solutions found.");

    }

    public static IEnumerable<object[]> TestMethods
        => [
            [nameof(LinearAlgebra_TwoEquations)],
            [nameof(SystemOfEquations_TwoEquations_Quadratic)],
            //[nameof(SystemOfEquations_TwoEquations_Quadratic_OverReals)],
            [nameof(ProgrammingZ3_3_1_2_EUF_Models)],
            [nameof(ProgrammingZ3_3_5_Algebraic_Datatypes)],
            [nameof(ProgrammingZ3_3_6_Sequences_And_Strings)]
        ];

    [DataTestMethod]
    [TestCategory("Z3")]
    [DynamicData(nameof(TestMethods), DynamicDataSourceType.Property)]
    [DoNotParallelize] 
    public void ComprehesionTests(string methodName)
    {
        try
        {
            Global.ToggleWarningMessages(true);
            Log.Open("test.log");

            MethodInfo? method = typeof(Z3Tests).GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new Exception("Method not found");

            // These examples need model generation turned on.
            using (Context ctx = new Context())
            {
                method.Invoke(this, [ctx]);
            }

            Log.Close();
            if (Log.isOpen())
                Console.WriteLine("Log is still open!");
        }
        catch (Z3Exception ex)
        {
            Console.WriteLine("Z3 Managed Exception: " + ex.Message);
            Console.WriteLine("Stack trace: " + ex.StackTrace);
        }
        catch (TestFailedException ex)
        {
            Console.WriteLine("TEST CASE FAILED: " + ex.Message);
        }
    }
}
