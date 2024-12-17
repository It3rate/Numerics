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
    public TileMode TileMode { get; } = TileMode.OneShot;
    public long Duration { get; } = 1; // can either be based on length, or hard coded. In either case is multiplied by some basis (speed of time, allowing slowing or reversing effect)
    public Number? CurrentResult { get; private set; }
    // akin to samplers, can be fixed data, looked up, random, or computed
    public List<Number> Results { get; } = new List<Number>();
    private List<IExpression> ExpressionChain { get; } = new List<IExpression> { }; // equations can be part of equations
    public int RepeatCount = 1;
    public int RepeatIndex = 0;
    public bool IsRepeatsComplete => TileMode != TileMode.Continue && RepeatIndex >= RepeatCount;

    public int CurrentIndex { get; private set; } = 0; // current index will be a number. Eg if equation is *3, then -7i+9 is the value of that over the duration of 7->9.
    public bool IsCycleComplete => CurrentIndex >= ExpressionChain.Count;
    public bool PreserveResults { get; private set; } = false;

    private bool _isInverted = false;
    private Number[] _initialNumbers;
    public Expression()
    {
    }
    public Expression(params Number[] numbers)
    {
        _initialNumbers = numbers;
        Reset();
    }
    public Expression(IEnumerable<Number> numbers, long duration, TileMode tileMode, bool preserveResults, int repeatCount = 1) : this(numbers.ToArray())
    {
        Duration = duration;
        TileMode = tileMode;
        PreserveResults = preserveResults;
        RepeatCount = repeatCount;
    }

    public void Reset()
    {
        CurrentResult = Number.SCALAR_ZERO;
        _isInverted = false;
        CurrentIndex = 0;
        RepeatIndex = 0;
        Results.Clear();
        AddNumbers(_initialNumbers);
    }

    public int AddNumbers(params Number[] numbers)
    {
        foreach (var number in numbers)
        {
            Results.Add(number);
            //CurrentResult = number;
        }
        return Results.Count - 1;
    }
    public int AddAtomicExpression(params AtomicExpression[] atomics)
    {
        foreach (var atomic in atomics) 
        {
            atomic.Parent = this;
            ExpressionChain.Add(atomic);
        }
        return ExpressionChain.Count - 1;
    }
    public void SetInput(Number input)
    {
        CurrentResult = input;
    }
    public Number? Next()
    {
        if (!IsRepeatsComplete && CurrentResult != null) // todo: check and implement duration, bounce mode etc.
        {
            var index = CurrentIndex >= ExpressionChain.Count ? ExpressionChain.Count - 1 : CurrentIndex;
            var expr = ExpressionChain[index];
            CurrentResult = _isInverted ? expr.CalculateInverse(CurrentResult) : expr.Calculate(CurrentResult);
            CurrentIndex++;

            if (PreserveResults && Results != null)
            {
                Results.Add(CurrentResult);
            }

            if(IsCycleComplete)
            {
                RepeatIndex += 1;
            }

            if(IsRepeatsComplete)
            {
                switch (TileMode)
                {
                    case TileMode.Bounce:
                        _isInverted = !_isInverted;
                        CurrentIndex = 0;
                        RepeatIndex = 0;
                        break;
                    case TileMode.Loop:
                        CurrentIndex = 0;
                        RepeatIndex = 0;
                        break;
                }
            }
        }
        return CurrentResult;
    }
    public Number Calculate(Number input)
    {
        SetInput(input);
        while (!IsCycleComplete && !IsRepeatsComplete)
        {
            Next();
        }
        return CurrentResult!;
    }
    public Number CalculateInverse(Number input)
    {
        SetInput(input);
        _isInverted = true;
        while (!IsCycleComplete && !IsRepeatsComplete)
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






public interface IScheduleable { }
// these go in the sequencer. It holds a list of expressions with durations, which can split, choose, and merge. The primitive is add/remove expressions, not expressions themselves.
public class SplitExpression : IScheduleable
{
    public TileMode TileMode => TileMode.Continue;
    public long Duration => 0;
    // add, remove, merge, split etc These form the letter shape type paths.
}
public class MergeExpression : IScheduleable
{
    public TileMode TileMode => TileMode.Continue;
    public long Duration => 0;
}

