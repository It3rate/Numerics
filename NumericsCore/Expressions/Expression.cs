using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Interfaces;

namespace NumericsCore.Expressions;

/// <summary>
/// A series of operations that transform an input.
/// </summary>
public class Expression : IExpression
{
    public TileMode TileMode { get; } = TileMode.Invert;
    public long Duration { get; } = 1; // can either be based on length, or hard coded. In either case is multiplied by some basis (speed of time, allowing slowing or reversing effect)
    public Number? CurrentResult { get; private set; }
    // akin to samplers, can be fixed data, looked up, random, or computed
    List<Number> Numbers { get; }
    List<IExpression> ExpressionChain { get; } = new List<IExpression> { }; // equations can be part of equations
    public int CurrentIndex { get; private set; } = 0; // current index will be a number. Eg if equation is *3, then -7i+9 is the value of that over the duration of 7->9.
    public bool IsComplete => TileMode != TileMode.Ignore && CurrentIndex >= ExpressionChain.Count;

    public Expression()
    {
        Numbers = new List<Number>();
    }
    public Expression(List<Number> numbers)
    {
        Numbers = numbers;
    }
    public Expression(List<Number> numbers, long duration, TileMode tileMode) : this(numbers)
    {
        Duration = duration;
        TileMode = tileMode;
    }

    public int AddNumber(Number number)
    {
        Numbers.Add(number);
        return Numbers.Count - 1;
    }
    public int AddEquationSteps(params AtomicExpression[] equationSteps)
    {
        ExpressionChain.AddRange(equationSteps);
        return ExpressionChain.Count - 1;
    }
    public void SetInput(Number input)
    {
        CurrentResult = input;
        CurrentIndex = 0;
    }
    public Number? Next()
    {
        if (!IsComplete && CurrentResult != null) // todo: check and implement duration, bounce mode etc.
        {
            var index = CurrentIndex >= ExpressionChain.Count ? ExpressionChain.Count - 1 : CurrentIndex;
            CurrentResult = ExpressionChain[index].Calculate(CurrentResult);
            CurrentIndex++;
        }
        return CurrentResult;
    }
    public Number Calculate(Number input)
    {
        SetInput(input);
        while (!IsComplete)
        {
            Next();
        }
        return CurrentResult!;
    }
    public Number CalculateAtT(Number input, double t)
    {
        throw new NotImplementedException();
    }
}

