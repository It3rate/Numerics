using NumericsAPI.Commands;
using Numerics.CoreConcepts.Time;

namespace NumericsAPI.CommandEngine;
public abstract class TaskBase : ITask
{
    private static int _idCounter = 1;
    public int Id { get; }

    public ICommand Command { get; }
    public CommandAgent Agent { get; set; }
    public TaskTimer Timer { get; }
    public float InterpolationT => Timer.InterpolationT;

    public abstract bool IsValid { get; }
    public virtual void Initialize() { }

    protected TaskBase(MillisecondNumber duration)
    {
        Id = _idCounter++;
        var delayMs = duration.StartTick;
        var durationMs = duration.EndTick;
        Timer = new TaskTimer(delayMs, durationMs);
    }
    public virtual void RunTask() { }
    public virtual void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
    {
        Timer.StartUpdate(currentTime.EndTick, deltaTime.EndTick);
        Timer.EndUpdate(currentTime.EndTick, deltaTime.EndTick);
    }
    public virtual void UnRunTask() { }

}
