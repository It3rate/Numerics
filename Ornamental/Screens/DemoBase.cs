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
    protected MouseAgent _currentMouseAgent;
    //protected List<int> _tests{get;} = new List<int>();

    public SKMapper PreviousTest(MouseAgent mouseAgent)
    {
        int index = _testIndex >= 1 ? _testIndex - 1 : Pages.Count - 1;
        return LoadTest(index, mouseAgent);
    }
    public SKMapper Reload(MouseAgent mouseAgent)
    {
        return LoadTest(_testIndex, mouseAgent);
    }
    public SKMapper NextTest(MouseAgent mouseAgent)
    {
        int index = _testIndex >= Pages.Count - 1 ? 0 : _testIndex + 1;
        return LoadTest(index, mouseAgent);
    }

    public SKMapper LoadTest(int index, MouseAgent mouseAgent)
    {
        _testIndex = index;
        _currentMouseAgent = mouseAgent;
        _currentMouseAgent.IsPaused = true;
        _currentMouseAgent.ClearAll();

        SKMapper wm = Pages[_testIndex]();
        //wm.EnsureRenderers();
        _currentMouseAgent.IsPaused = false;
        return wm;
    }
}
