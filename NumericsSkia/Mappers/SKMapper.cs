
using NumericsSkia.Drawing;
using NumericsSkia.Renderer;
using NumericsCore.Primitives;
using NumericsCore.Utils;
using SkiaSharp;
using NumericsSkia.Agent;

namespace NumericsSkia.Mappers;

public abstract class SKMapper : IDrawableElement
{
    public int Id { get; set; }
    protected static int idCounter = 0;

    public RenderAgent Agent { get; }
    public CoreRenderer Renderer => Agent.Renderer;
    public SKCanvas Canvas => Renderer.Canvas;
    protected CorePens Pens => Renderer.Pens;

    public bool Do2DRender { get; set; } = false;

    public SKSegment Guideline { get; set; } = new SKSegment(0, 0, 1, 1);
    public SKSegment InvertedGuideline => new SKSegment(Guideline.StartPoint, Guideline.StartPoint - (Guideline.EndPoint - Guideline.StartPoint));

    public SKPoint StartPoint
    {
        get => Guideline.StartPoint;
        set => Reset(value, EndPoint);
    }
    public SKPoint MidPoint => Guideline.Midpoint;
    public SKPoint EndPoint
    {
        get => Guideline.EndPoint;
        set => Reset(StartPoint, value);
    }
    public SKPoint[] EndPoints => new SKPoint[] { StartPoint, EndPoint };

    protected SKMapper(RenderAgent agent, SKSegment guideline = default)
    {
        Agent = agent;
        Guideline = guideline ?? new SKSegment(0, 0, 1, 1);
    }
    public virtual void Reset(SKPoint startPoint, SKPoint endPoint)
    {
        Guideline.Reset(startPoint, endPoint);
    }
    public virtual void Reset(SKSegment segment)
    {
        Reset(segment.StartPoint, segment.EndPoint);
    }

    //public abstract SKPoint StartPoint { get; set; }
    //public abstract SKPoint MidPoint { get; }
    //public abstract SKPoint EndPoint { get; set; }

    public abstract void Draw();


}
