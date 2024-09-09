using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Interfaces;

namespace NumericsCore.Expressions
{
    public interface IExpression
    {
        TileMode TileMode { get; }
        long Duration { get; }
        Number Calculate(Number input);
        Number CalculateAtT(Number input, double t);
    }
}
