using Numerics.Utils;
using Numerics.CoreConcepts;
using Numerics.CoreConcepts.Time;
using Numerics.Primitives;

namespace NumericsAPI.CommandEngine;
public class CommandAgent : IAgent
{
    public CommandStack Stack { get; }

    public CommandAgent()
    {
        Stack = new CommandStack(this);
    }

    public void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
    {
        //Workspace.Update(currentTime, deltaTime);
        Stack.Update(currentTime, deltaTime);
    }

    public virtual void ClearAll()
    {
        Stack.Clear();
    }
}
