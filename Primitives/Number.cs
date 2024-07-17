using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.Primitives;

public class Number :
    IMeasurable,
    Measurable<Number>
//IAdditionOperators<Number, Number, Number>,
//ISubtractionOperators<Number, Number, Number>,
//IAdditiveIdentity<Number, Number>,
//IMultiplyOperators<Number, Number, Number>,
//IDivisionOperators<Number, Number, Number>,
//IMultiplicativeIdentity<Number, Number>,
//IIncrementOperators<Number>,
//IDecrementOperators<Number>,
//IUnaryNegationOperators<Number, Number>,
//IUnaryPlusOperators<Number, Number>,
//IMinMaxValue<Number>
{
    public Focal Focal => _focal;
    private Focal _focal;
    public Domain Domain => _domain;
    private Domain _domain;
    public long StartTick { get => _focal.StartTick; set => _focal.StartTick = value; }
    public long EndTick { get => _focal.EndTick; set => _focal.EndTick = value; }

    public long TickLength => _focal.TickLength;

    public long AbsTickLength => _focal.AbsTickLength;

    public Number(Domain domain, Focal focal)
    {
        _domain = domain;
        _focal = focal;
    }
}
