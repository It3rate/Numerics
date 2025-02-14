using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericsCore.Motions;

public class ExprStack
{
	public Reference Left { get; }
	public Reference Right { get; }
	public Reference Output { get; private set; }
	public List<StackOp> Ops { get; } = new List<StackOp>();

	private Stack<double> Stack { get; } = new Stack<double>();

	public ExprStack(Reference left, Reference right, params StackOp[] ops)
	{
		Left = left;
		Right = right;
		Output = Left;
		Ops.AddRange(ops);
	}
	public Reference Run()
	{
		foreach (StackOp op in Ops)
		{
			// ops come from the stack or input.
			var values = op.Mask == BitMask.None ? PopTwo() : op.Mask.GetValues(Left, Right);
			var result = op.Mask.CombineValues(op.Operation, values);
			Push(result);
		}
		Output = new ReferenceByRatio(Left.Source, Pop(), Pop());
		return Output;
	}
	// mult: push A0B0, push A1B1, mult, push A0B1, push A1B0, mult, add
	// mult2: stack AB as N, mult BC, mult AD, add
	// op,op,mult, op,op,mult, createNumber(ni+n)
	public void Push(double item) => Stack.Push(item);
	public double Pop() => Stack.Pop();
	public double Peek(int position) => Stack.Peek();
	public double[] PopTwo() => [Pop(), Pop()];
}
public class StackOp
{
	public BitMask Mask { get; }
	public Ops Operation { get; }
	public StackOp(BitMask mask, Ops operation)
	{
		Mask = mask;
		Operation = operation;
	}	
}
