using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Sequencer;

public delegate void TimedEventHandler(object sender, EventArgs e);

public interface ITimeable
{
    float InterpolationT { get; set; }
    //double DeltaTime { get; }
    //double _currentTime { get; }
    //double PreviousTime { get; }

    double StartTime { get; set; }

    Number DelayDuration { get; }
    double DelayValue { get; }
    double DurationValue { get; }

    //bool IsReverse { get; }
    //bool IsComplete { get; }

    event TimedEventHandler StartTimedEvent;
    event TimedEventHandler StepTimedEvent;
    event TimedEventHandler EndTimedEvent;

    void Restart();
    void Reverse();

    void Pause();
    void Resume();
}
