
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Timer = System.Timers.Timer;
//using Numerics.Primitives;
//using System.Timers;

//namespace NumericsCore.Sequencer;

//public interface IRunner
//{
//}


//public class Runner : IRunner
//{
//    public static Runner Instance = new Runner();

//    public long CurrentMS { get; private set; }
//    private Timer _sysTimer;
//    private DateTime _startTime;
//    private TimeSpan _lastTime;
//    private TimeSpan _currentTime;
//    private bool _isPaused;
//    private bool _isBusy = false;
//    private static TimeSpan _delayTime = new TimeSpan(0);


//    public Runner()
//    {
//        _sysTimer = Reset();
//    }
//    private Timer Reset()
//    {
//        _startTime = DateTime.Now;

//        _currentTime = DateTime.Now - _startTime;
//        _lastTime = _currentTime; ;

//        var sysTimer = new Timer();
//        sysTimer.Elapsed += Tick;
//        sysTimer.Interval = 8;
//        sysTimer.Enabled = true;
//        return sysTimer;
//    }
//    private void Tick(object? sender, ElapsedEventArgs e)
//    {
//        if (!_isPaused && !_isBusy) // && _needsUpdate)
//        {
//            _isBusy = true;
//            _currentTime = e.SignalTime - (_startTime + _delayTime);
//            CurrentMS = (long)_currentTime.TotalMilliseconds;

//            _lastTime = _currentTime;
//        }

//        _isBusy = false;
//    }
//}





using Numerics.CoreConcepts.Time;
using Numerics.Primitives;
using Numerics.Utils;
using System.Timers;
using Timer = System.Timers.Timer;

namespace NumericsCore.Sequencer;

public interface IRunner
{
    //MillisecondNumber CurrentMS { get; }
    MillisecondNumber DeltaMS { get; }
}

public class Runner : IRunner
{
    public event EventHandler OnContextStringChanged;

    public static Runner CurrentRunner;
    public static Runner GetRunnerById(int id) => CurrentRunner;

    public IAgent Agent { get; set; }

    public string lbEquation; // todo: callback event

    private bool _isPaused;
    private static DateTime _pauseTime;
    private static TimeSpan _delayTime = new TimeSpan(0);
    public static DateTime StartTime { get; private set; }

    private static MillisecondNumber? _millisecondNumber;
    public static MillisecondNumber CurrentMS {
        get
        {
            if(_millisecondNumber == null)
            {
                _millisecondNumber = MillisecondNumber.Create(0, long.MaxValue);
            }
            return _millisecondNumber!;
        }
    } 
    public MillisecondNumber DeltaMS { get; } = MillisecondNumber.Zero(false);

    private Timer _sysTimer;
    private DateTime _startTime;
    private TimeSpan _lastTime;
    private TimeSpan _currentTime;
    public long CurrentTimeMs => (long)_currentTime.TotalMilliseconds;

    public Runner(IAgent agent)
    {
        Agent = agent;
        CurrentRunner = this;
        Reset();
    }
    public string EquationText { get; private set; }
    public void SetEquationText(string txt)
    {
        EquationText = txt;
        OnContextStringChanged?.Invoke(this, new EventArgs());
    }
    //private float t = 0;
    private bool _isBusy = false;
    private bool _needsUpdate = true;
    public bool HasUpdated { get; set; } = false;
    public bool NeedsUpdate() => _needsUpdate = true;

    private void Tick(object sender, ElapsedEventArgs e)
    {
        if (!_isPaused && !_isBusy) // && _needsUpdate)
        {
            _isBusy = true;
            _currentTime = e.SignalTime - (StartTime + _delayTime);
            CurrentMS.Focal.EndTick = (long)_currentTime.TotalMilliseconds;
            DeltaMS.Focal.EndTick = (long)(CurrentMS.EndTick - _lastTime.TotalMilliseconds);

            Agent.Update(CurrentMS, DeltaMS);

            _lastTime = _currentTime;
            _needsUpdate = !HasUpdated;
        }

        _isBusy = false;
    }

    public void ActivateComposite(int id)
    {
        //var composite = Composites[id];
        //if (composite != null)
        //{
        //	if (composite is ITimeable anim)
        //	{
        //		anim.StartTime = (float) (DateTime.Now - Runner.StartTime).TotalMilliseconds;
        //	}

        //	Composites.ActivateElement(composite.Id);
        //}
    }

    public void DeactivateComposite(int id)
    {
        //Composites.DeactivateElement(id);
    }

    public void Clear()
    {
        _sysTimer?.Stop();
        _sysTimer.Close();
        _sysTimer = null;
        Reset();
    }

    public void Reset()
    {
        StartTime = DateTime.Now; // or leave at zero?

        _currentTime = DateTime.Now - StartTime;
        _lastTime = _currentTime;
        CurrentMS.Focal.EndTick = (long)_currentTime.TotalMilliseconds;

        _sysTimer = new Timer();
        _sysTimer.Elapsed += Tick;
        _sysTimer.Interval = 8;
        _sysTimer.Enabled = true;
    }

    public void Pause()
    {
        if (!_isPaused)
        {
            OnPause(this, null);
        }
    }

    public void UnPause()
    {
        if (_isPaused)
        {
            OnPause(this, null);
        }
    }
    public void TogglePause()
    {
        OnPause(this, null);
    }

    private void OnPause(object sender, EventArgs e)
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            _pauseTime = DateTime.Now;
            //foreach (var id in Composites.ActiveIds)
            //{
            //	if (Composites.ContainsKey(id) && (Composites[id] is ITimeable))
            //	{
            //		((ITimeable) Composites[id]).Pause();
            //	}
            //}
        }
        else
        {
            _delayTime += DateTime.Now - _pauseTime;
            _lastTime = DateTime.Now - (StartTime + _delayTime);
            //foreach (var id in Composites.ActiveIds)
            //{
            //	if (Composites.ContainsKey(id) && (Composites[id] is ITimeable))
            //	{
            //		((ITimeable) Composites[id]).Resume();
            //	}
            //}
        }
    }

}