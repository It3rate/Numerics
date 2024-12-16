using NumericsSkia.Agent;

namespace Ornamental.Utils;

public static class MouseEventArgsExtension
{
    public static MouseArgs ToMouseArgs(this MouseEventArgs args)
    {
        var btn = (NumericsSkia.Agent.MouseButtons)args.Button;
        var result = new MouseArgs(args.X, args.Y, args.Delta, args.Clicks, btn);
        return result;
    }
}
public static class KeyEventArgsExtension
{
    public static KeyArgs ToKeyArgs(this KeyEventArgs args)
    {
        var keyData = (NumericsSkia.Agent.Keys)args.KeyData;
        var result = new KeyArgs(keyData);
        return result;
    }
}
