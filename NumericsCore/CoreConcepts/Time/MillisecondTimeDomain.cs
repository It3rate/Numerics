using Numerics.Primitives;

namespace Numerics.CoreConcepts.Time;

public class MillisecondTimeDomain : Domain
{
    public static MillisecondTimeDomain Instance => MinMax;
    private MillisecondTimeDomain(TimeTrait trait, Focal basis, Focal minMax) : base(trait, basis, minMax, "timeMillisecond")
    {
    }
    private static MillisecondTimeDomain? _minMax;
    public static MillisecondTimeDomain MinMax {
        get
        {
            if(_minMax == null)
            {
                _minMax =  new MillisecondTimeDomain(TimeTrait.Instance, new Focal(0, 1000), Focal.FullLimits);
            }
            return _minMax!;
        }
    }
}
