using Numerics.Primitives;

namespace Numerics.CoreConcepts.Time;

public class MillisecondTimeDomain : Domain
{
    public static MillisecondTimeDomain Instance => MinMax;
    private MillisecondTimeDomain(TimeTrait trait, Focal basis, Focal minMax) : base(trait, basis, minMax, "timeMillisecond")
    {
    }
    public static MillisecondTimeDomain MinMax { get; } = new MillisecondTimeDomain(TimeTrait.Instance, new Focal(0, 1000), Focal.FullLimits);
}
