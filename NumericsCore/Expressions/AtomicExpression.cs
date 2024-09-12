using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Interfaces;
using NumericsCore.Expressions;
using System.Linq.Expressions;
using System.Reflection;

namespace NumericsCore.Expressions
{
    public class AtomicExpression : IExpression
    {
        public TileMode TileMode { get; } = TileMode.Invert;
        public long Duration { get; } = 1;
        public int RightIndex { get; } // use previous value expression?
        public OperationBase Operation { get; }
        public Expression? Parent { get; set; }
        public IExpression? RightExpression { get; set; }


        public AtomicExpression(OperationBase operation, long duration)
        {
            Operation = operation;
            Duration = duration;
        }
        public AtomicExpression(int rightIndex, OperationBase operation, long duration)
        {
            RightIndex = rightIndex;
            Operation = operation;
            Duration = duration;
        }
        public AtomicExpression(IExpression rightExpression, OperationBase operation, long duration)
        {
            RightExpression = rightExpression;
            Operation = operation;
            Duration = duration;
        }
        public Number Calculate(Number input)
        {
            var result = input;
            if (Operation is BinaryOperationsBase binary)
            {
                if(Parent != null) // if there's no parent, just return the input.
                {
                    var index = RightIndex >= 0 ? RightIndex : Parent.Results.Count + RightIndex; // this needs to be relative to the current index, which is in the expression
                    binary.SetRightSide(Parent.Results[index]);
                    result = binary.Calculate(input);
                }
            }
            else
            {
                result = Operation.Calculate(input);
            }
            return result;
        }
        //public Number Calculate(Number left, Number right)
        //{
        //    var result = left;
        //    if (Operation is BinaryOperationsBase binary)
        //    {
        //        binary.SetRightSide(right);
        //        result = binary.Calculate(left);
        //    }
        //    return result;
        //}
         public Number CalculateAtT(Number input, double t)
        {
            throw new NotImplementedException();
        }
    }
}
