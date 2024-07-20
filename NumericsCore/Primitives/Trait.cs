
namespace Numerics.Primitives;

/// <summary>
/// Traits are measurable properties on objects. They can be composed of multiple domains (like mph or dollars/day or lwh) but do not need to be.
/// </summary>
public class Trait
{
    public virtual string Name { get; private set; }
    public Trait(string name)
    {
        Name = name;
    }
    public virtual Trait Clone() => new Trait(Name);
}
