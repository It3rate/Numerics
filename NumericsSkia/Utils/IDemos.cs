using NumericsSkia.Agent;
using NumericsSkia.Mappers;

namespace NumericsSkia.Utils;

public delegate SKMapper PageCreator();
public interface IDemos
{
    List<PageCreator> Pages { get; }
    SKMapper NextTest(MouseAgent agent);
    SKMapper PreviousTest(MouseAgent agent);
    SKMapper Reload(MouseAgent agent);

    SKMapper LoadTest(int index, MouseAgent mouseAgent);
}
