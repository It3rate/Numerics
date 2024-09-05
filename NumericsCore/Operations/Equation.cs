using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Structures;

/// <summary>
/// A series of operations that transform an input.
/// </summary>
public class Equation
{
    // akin to samplers, can be fixed data, looked up, random, or computed
    List<Number> Numbers { get; } = new List<Number>();
    List<EquationStep> EquationSteps { get; } = new List<EquationStep> { };
    private int CurrentIndex { get; } = 0;

    public Equation() { }

    public int AddNumber(Number number)
    {
        Numbers.Add(number);
        return Numbers.Count - 1;
    }
    public int AddEquationStep(EquationStep equationStep)
    {
        EquationSteps.Add(equationStep);
        return EquationSteps.Count - 1;
    }
}
public class EquationStep
{
    public int RightIndex { get; }
    public OperationsBase Operation { get; }
    public long Duration { get; }
    private List<Number> Numbers { get; } = new List<Number>();

    public EquationStep(List<Number> numbers, int rightIndex, OperationsBase operation, long duration)
    {
        Numbers = numbers;
        RightIndex = rightIndex;
        Operation = operation;
        Duration = duration;
    }
    public Number Calculate(Number left)
    {
        return Operation.Calculate(left, Numbers[RightIndex]);
    }
}
