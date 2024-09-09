using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Interfaces;

namespace NumericsCore.Structures;

public interface IEquationElement
{
    TileMode TileMode { get; }
    long Duration { get; }
    Number Calculate(Number input);
    Number CalculateAtT(Number input, double t);
}
/// <summary>
/// A series of operations that transform an input.
/// </summary>
public class Equation : IEquationElement
{
    public TileMode TileMode { get; } = TileMode.Invert;
    public long Duration { get; } = 1; // can either be based on length, or hard coded. In either case is multiplied by some basis (speed of time, allowing slowing or reversing effect)
    public Number? CurrentResult { get; private set; }
    // akin to samplers, can be fixed data, looked up, random, or computed
    List<Number> Numbers { get; }
    List<IEquationElement> EquationChain { get; } = new List<IEquationElement> { }; // equations can be part of equations
    public int CurrentIndex { get; private set; } = 0; // current index will be a number. Eg if equation is *3, then -7i+9 is the value of that over the duration of 7->9.
    public bool IsComplete => CurrentIndex >= EquationChain.Count;

    public Equation()
    {
        Numbers = new List<Number>();
    }
    public Equation(List<Number> numbers) 
    {
        Numbers = numbers;
    }

    public int AddNumber(Number number)
    {
        Numbers.Add(number);
        return Numbers.Count - 1;
    }
    public int AddEquationSteps(params EquationStep[] equationSteps)
    {
        EquationChain.AddRange(equationSteps);
        return EquationChain.Count - 1;
    }
    public void SetInput(Number input)
    {
        CurrentResult = input;
        CurrentIndex = 0;
    }
    public Number Next()
    {
        if(!IsComplete) // todo: check and implement duration, bounce mode etc.
        {
            CurrentResult = EquationChain[CurrentIndex].Calculate(CurrentResult);
            CurrentIndex++;
        }
        return CurrentResult!;
    }
    public Number Calculate(Number input)
    {
        SetInput(input);
        while(!IsComplete)
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

public class EquationStep : IEquationElement
{
    public TileMode TileMode { get; } = TileMode.Invert;
    public long Duration { get; } = 1;
    public int RightIndex { get; }
    public OperationBase Operation { get; }
    private List<Number> Numbers { get; }


    public EquationStep(List<Number> numbers, int rightIndex, OperationBase operation, long duration)
    {
        Numbers = numbers;
        RightIndex = rightIndex;
        Operation = operation;
        Duration = duration;
    }
    public EquationStep(OperationBase operation, long duration)
    {
        Numbers = new List<Number> { };
        Operation = operation;
        Duration = duration;
    }
    public Number Calculate(Number input)
    {
        var result = input;
        if(Operation is BinaryOperationsBase binary)
        {
            result = binary.Calculate(input, Numbers[RightIndex]);
        }
        else
        {
            result = Operation.Calculate(input);
        }
        return result;
    }
    public Number CalculateAtT(Number input, double t)
    {
        throw new NotImplementedException();
    }
}
