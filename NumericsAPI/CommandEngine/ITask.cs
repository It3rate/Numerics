using NumericsAPI.Commands;
using NumericsCore.Sequencer;

namespace NumericsAPI.CommandEngine;
public interface ITask
{
    int Id { get; }
    ICommand Command { get; }
    CommandAgent Agent { get; set; }
    TaskTimer Timer { get; }
    bool IsValid { get; }
    void RunTask();
    void UnRunTask();
    //void OnAddedToCommand(ICommand command);
}

public interface ICreateTask
{
}
