using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timer = System.Timers.Timer;
using Numerics.Primitives;
using System.Timers;

namespace NumericsCore.Sequencer;

public interface IRunner
{
//    Number CurrentMS { get; }
//    Number DeltaMS { get; }
}


public class Runner : IRunner
{
    public static Runner Instance = new Runner();
    //public static Domain TimeDomain = new Domain(new Trait("time"), new Focal(0, 1000), Focal.PositiveLimits);
    //public Number CurrentMS { get; } = new Number(TimeDomain, new(0,0));
    //public Number DeltaMS { get; } = new Number(TimeDomain, new(0, 0));

    public long CurrentMS { get; private set; }
    private Timer _sysTimer;
    private DateTime _startTime;
    private TimeSpan _lastTime;
    private TimeSpan _currentTime;
    private bool _isPaused;
    private bool _isBusy = false;
    private static TimeSpan _delayTime = new TimeSpan(0);


    public Runner()
    {
        _sysTimer = Reset();
    }
    private Timer Reset()
    {
        _startTime = DateTime.Now;

        _currentTime = DateTime.Now - _startTime;
        _lastTime = _currentTime;;

        var sysTimer = new Timer();
        sysTimer.Elapsed += Tick;
        sysTimer.Interval = 8;
        sysTimer.Enabled = true;
        return sysTimer;
}
    private void Tick(object? sender, ElapsedEventArgs e)
    {
        if (!_isPaused && !_isBusy) // && _needsUpdate)
        {
            _isBusy = true;
            _currentTime = e.SignalTime - (_startTime + _delayTime);
            CurrentMS = (long)_currentTime.TotalMilliseconds;
            //DeltaMS.EndTicks = (long)(CurrentMS.EndTicks - _lastTime.TotalMilliseconds);
            //Agent.Update(CurrentMS, DeltaMS);

            _lastTime = _currentTime;
            //_needsUpdate = !HasUpdated;
        }

        _isBusy = false;
    }
}