using Numerics.CoreConcepts.Time;

namespace NumericsAPI.CommandEngine;

// todo: a task timer for animation is really just a transform.
public class TaskTimer //: ITimeable
{
    public float InterpolationT { get; set; }
    public bool IsComplete { get; protected set; } = false;

    public long StartTime { get; set; }
    private ICommandStack? _stack;
    private long _runningTime;
    private long _currentTime;
    private long _pauseTimeMs;
    private long _delayTimeMs = 0;
    private bool _isPaused;
    protected bool IsReverse { get; set; } = false; // might need for undo/scrub

    public MillisecondNumber DelayDuration { get; } // can 'type' numbers, this would be on a domain of 0-30 focal unit, and 0-max range, time trait.
    public long DelayMs => DelayDuration.StartTick; // Delay is unot, duration is unit
    public long DurationMs => DelayDuration.EndTick;

    public TaskTimer(long delayMs, long durationMs)
    {
        DelayDuration = MillisecondNumber.Create(delayMs, durationMs);
    }

    public void Begin(ICommandStack stack)
    {
        _stack = stack;
        _pauseTimeMs = _stack.CurrentTime.EndTick;
    }

    public void Restart()
    {
        StartTime = _stack!.CurrentTime.EndTick - _stack.StartTime.StartTick;
        _currentTime = StartTime;
        _runningTime = 0;
        _delayTimeMs = 0;
        IsComplete = false;
    }

    public void StartUpdate(long currentTime, long deltaTime)
    {
        //_currentTime = currentTime;
        if (!_isPaused)
        {
            _runningTime += deltaTime + _delayTimeMs;
            _delayTimeMs = 0;
            _currentTime = StartTime + _runningTime;
            if (_currentTime > StartTime + DurationMs)
            {
                IsComplete = true;
                InterpolationT = 1f;
            }
            else
            {
                InterpolationT = (float)(_currentTime < StartTime ? 0 :
                    _currentTime > StartTime + DurationMs ? 1f :
                    (_currentTime - StartTime) / (float)DurationMs);
            }

            InterpolationT = IsReverse ? 1f - InterpolationT : InterpolationT;
        }
    }
    public void EndUpdate(double currentTime, double deltaTime)
    {
        if (IsComplete)
        {
        }
    }
    public void Reverse()
    {
        IsReverse = !IsReverse;
    }

    //public override ParametricSeries GetNormalizedPropertyAtT(PropertyId propertyId, ParametricSeries seriesT)
    //{
    //    ParametricSeries result;
    //    if (propertyId == PropertyId.InterpolationT)
    //    {
    //        result = new ParametricSeries(1, InterpolationT);
    //    }
    //    else
    //    {
    //        result = base.GetNormalizedPropertyAtT(propertyId, new ParametricSeries(1, InterpolationT)); // todo: include seriesT, probably needed when scrubbing.
    //    }

    //    return result;
    //}

    //public override ISeries GetSeriesAtT(PropertyId propertyId, float t, ISeries parentSeries)
    //{
    //    ParametricSeries result;
    //    if (propertyId == PropertyId.InterpolationT)//PropertyIdSet.IsTSampling(propertyId))
    //    {
    //        result = new ParametricSeries(1, InterpolationT); // this is straight timer lookup, so no ref to input t needed.
    //    }
    //    else
    //    {
    //        result = base.GetNormalizedPropertyAtT(propertyId, new ParametricSeries(1, InterpolationT));
    //    }
    //    return result;
    //}

    public void Pause()
    {
        _isPaused = true;
        _pauseTimeMs = _stack!.CurrentTime.EndTick;
    }

    public void Resume()
    {
        _isPaused = false;
        _delayTimeMs = _stack!.CurrentTime.EndTick - _pauseTimeMs;
    }
}
