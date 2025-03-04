using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.CoreConcepts.Time;
using NumericsCore.Interfaces;

namespace NumericsCore.Motions;

/// <summary>
/// Operations that have a duration and wrap mode
/// </summary>
public class Motion
{
	public ExprStack Stack { get; }
	public MillisecondNumber? Duration { get; } 

	public Reference Left => Stack.Left; // these need to potentially be exprStacks, so some interface (or exprStack is a reference?)
	public Reference Right => Stack.Right;
	public Reference Output => Stack.Output; // is a ref from this 'numberline'?

	// internally track previous, current, predicted, history, algortihm, error, expected error/certainty, surprise, working resolution
	// there can be more than two inputs, as in RGB (input) computing saturation (output)

	public Motion(ExprStack stack)
	{
		Stack = stack;
	}
	public Reference Run() => Stack.Run();

	public float InterpolationT { get; } // just do linear for now

	public TileMode TileMode { get; } = TileMode.OneShot;
	public bool IsDynamic { get; set; } = false;

	public int RepeatCount { get; } = -1; // maybe based on max len?
	public int _repeatIndex = 0;
	public bool IsRepeatsComplete => TileMode != TileMode.Continue && _repeatIndex >= RepeatCount;

}
