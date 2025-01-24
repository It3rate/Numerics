using Numerics.CoreConcepts.Time;
using NumericsAPI.Commands;

namespace NumericsAPI.CommandEngine;
public interface ITask
{
    int Id { get; }
    ICommand Command { get; }
    CommandAgent Agent { get; set; }
    TaskTimer Timer { get; }
    float InterpolationT { get; }
    bool IsValid { get; }
    void RunTask();
    void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime);
    void UnRunTask();
    //void OnAddedToCommand(ICommand command);
}

public interface ICreateTask
{
}
