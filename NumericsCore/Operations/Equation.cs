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

/// <summary>
/// A series of operations that transform an input.
/// </summary>
public class Equation
{
    // akin to samplers, can be fixed data, looked up, random, or computed
    List<Number> Numbers { get; }
    List<EquationStep> EquationSteps { get; } = new List<EquationStep> { };
    private int CurrentIndex { get; } = 0;

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
        EquationSteps.AddRange(equationSteps);
        return EquationSteps.Count - 1;
    }
    public Number Calculate(Number input)
    {
        var result = input;
        foreach (var step in EquationSteps)
        {
            result = step.Calculate(result);
        }
        return result;
    }
}
public class EquationStep
{
    public int RightIndex { get; }
    public OperationBase Operation { get; }
    public long Duration { get; }
    private List<Number> Numbers { get; }
    private TileMode TileMode { get; } = TileMode.Invert;

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
        Number result;
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
}
