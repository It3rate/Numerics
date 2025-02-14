namespace NumericsCore.Motions;

public class Portion(INumberline numberline, Reference segment)
{
    public INumberline Numberline { get; } = numberline;
    public Reference Segment { get; } = segment;
}
