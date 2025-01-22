namespace NumericsSkia.Mappers;

using NumericsSkia.Agent;
using NumericsSkia.Renderer;
using SkiaSharp;

public interface IDrawableElement
{
    int Id { get; set; }
    RenderAgent Agent { get; }
    CoreRenderer Renderer { get; }
    SKCanvas Canvas { get; }
    void Draw();
}
