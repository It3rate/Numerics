using Numerics.Primitives;

namespace Numerics.CoreConcepts.Time;

public class TimeTrait : Trait
{
    private static readonly TimeTrait _instance = new TimeTrait();
    public static TimeTrait Instance => _instance;

    private TimeTrait() : base("Time") { }


}
