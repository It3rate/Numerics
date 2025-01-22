namespace MathDemo;

using NumericsSkia.Utils;
using NumericsSkia.Agent;
using NumericsSkia.Mappers;
using System.Collections.Generic;

public abstract class DemoBase : IDemos
{
    public List<PageCreator> Pages { get; } = new List<PageCreator>();
    protected int Count => Pages.Count;
    protected int _testIndex = 0;
    protected RenderAgent _currentAgent;
    //protected List<int> _tests{get;} = new List<int>();

    public SKMapper PreviousTest(RenderAgent mouseAgent)
    {
        int index = _testIndex >= 1 ? _testIndex - 1 : Pages.Count - 1;
        return LoadTest(index, mouseAgent);
    }
    public SKMapper Reload(RenderAgent mouseAgent)
    {
        return LoadTest(_testIndex, mouseAgent);
    }
    public SKMapper NextTest(RenderAgent mouseAgent)
    {
        int index = _testIndex >= Pages.Count - 1 ? 0 : _testIndex + 1;
        return LoadTest(index, mouseAgent);
    }

    public SKMapper LoadTest(int index, RenderAgent mouseAgent)
    {
        _testIndex = index;
        _currentAgent = mouseAgent;
        _currentAgent.IsPaused = true;
        _currentAgent.ClearAll();

        SKMapper wm = Pages[_testIndex]();
        //wm.EnsureRenderers();
        _currentAgent.IsPaused = false;
        return wm;
    }
}
