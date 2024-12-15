using Numerics.CoreConcepts.Time;

namespace Numerics.Utils;

public interface IAgent
{

    void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime);

    void ClearAll();
}
