using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Ops
{
    public interface OpInterface
    {
        Number Input { get; }
        Number Duration { get; }
        Number ValueAtT(Number t);
    }
}
