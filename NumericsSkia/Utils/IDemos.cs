using NumericsSkia.Agent;
using NumericsSkia.Mappers;

namespace NumericsSkia.Utils;

public delegate SKMapper PageCreator();
public interface IDemos
{
    List<PageCreator> Pages { get; }
    SKMapper NextTest(RenderAgent agent);
    SKMapper PreviousTest(RenderAgent agent);
    SKMapper Reload(RenderAgent agent);

    SKMapper LoadTest(int index, RenderAgent mouseAgent);
}
