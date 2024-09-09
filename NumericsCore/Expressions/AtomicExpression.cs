using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Interfaces;
using NumericsCore.Expressions;

namespace NumericsCore.Expressions
{
    public class AtomicExpression : IExpression
    {
        public TileMode TileMode { get; } = TileMode.Invert;
        public long Duration { get; } = 1;
        public int RightIndex { get; }
        public OperationBase Operation { get; }
        private List<Number> Numbers { get; }
        private bool PreserveResults { get; } = true;


        public AtomicExpression(List<Number> numbers, int rightIndex, OperationBase operation, long duration)
        {
            Numbers = numbers;
            RightIndex = rightIndex;
            Operation = operation;
            Duration = duration;
        }
        public AtomicExpression(OperationBase operation, long duration)
        {
            Numbers = new List<Number> { };
            Operation = operation;
            Duration = duration;
        }
        public Number Calculate(Number input)
        {
            var result = input;
            if (Operation is BinaryOperationsBase binary)
            {
                var index = RightIndex >= 0 ? RightIndex : Numbers.Count + RightIndex; // this needs to be relative to the current index, which is in the expression
                result = binary.Calculate(input, Numbers[index]);
            }
            else
            {
                result = Operation.Calculate(input);
            }

            if(PreserveResults && Numbers != null)
            {
                Numbers.Add(result);
            }
            return result;
        }
        public Number CalculateAtT(Number input, double t)
        {
            throw new NotImplementedException();
        }
    }
}
