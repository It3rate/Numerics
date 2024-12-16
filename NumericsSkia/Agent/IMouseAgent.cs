
using NumericsAPI.CommandEngine;
using NumericsCore.Utils;
using NumericsSkia.Agent;
using Numerics.Utils;

namespace NumericsSkia.Agent;

public interface IMouseAgent : IAgent
{
    CommandStack Stack { get; }

    bool IsPaused { get; set; }

    bool MouseDown(MouseArgs e);
    bool MouseMove(MouseArgs e);
    bool MouseUp(MouseArgs e);
    //bool KeyDown(KeyArgs e);
    //bool KeyUp(KeyArgs e);
    bool MouseDoubleClick(MouseArgs e);
    bool MouseWheel(MouseArgs e);
}
