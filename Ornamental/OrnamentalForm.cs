using SkiaSharp.Views.Desktop;
using NumericsSkia.Renderer;
using Numerics.Primitives;
using NumericsSkia.Agent;
using Ornamental.Utils;
using Sys = System.Windows.Forms;
using NumericsCore.Sequencer;
using NumericsSkia.Utils;
using MathDemo;
using NumericsSkia.Mappers;
using Ornamental.Agent;

namespace Ornamental;

public partial class OrnamentalForm : Form
{
    private readonly IDemos _demos;
    private readonly CoreRenderer _renderer;
    private readonly SKControl _control;
    private readonly OrnamentAgent _renderAgent;
    private Runner _runner;
    //private Workspace _workspace;
    private SKMapper _mapper;

    public OrnamentalForm()
    {
        InitializeComponent();
        DoubleBuffered = true;
        KeyPreview = true;
        _renderer = CoreRenderer.Instance;
        _control = new SKControl();
        _control.Width = Width;
        _control.Height = Height;
        _control.Dock = DockStyle.Fill;
        _renderer.Width = Width;
        _renderer.Height = Height;
        corePanel.Controls.Add(_control);

        _control.PaintSurface += DrawOnPaintSurface;
        _control.MouseDown += OnMouseDown;
        _control.MouseMove += OnMouseMove;
        _control.MouseUp += OnMouseUp;
        _control.MouseDoubleClick += OnMouseDoubleClick;
        _control.MouseWheel += OnMouseWheel;
        _control.PreviewKeyDown += OnPreviewKeyDown;
        corePanel.Resize += OnResize;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;

        _demos = new SymSlides();
        _renderAgent = new OrnamentAgent(this, _renderer, _demos);

        _runner = _renderAgent.Runner;
        _ = Execute(null, 50);
    }
    private void OnResize(object? sender, EventArgs e)
    {
        _renderer.Width = corePanel.Width;
        _renderer.Height = corePanel.Height;
    }
    private void DrawOnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        _renderer.DrawOnCanvas(e.Surface.Canvas);
    }
    public async Task Execute(Action action, int timeoutInMilliseconds)
    {
        await Task.Delay(timeoutInMilliseconds);
        ReloadTest();
        NeedsUpdate();
    }

    private void SetMapper(SKMapper mapper)
    {
        _mapper = mapper;
        _renderAgent.Mapper = _mapper;
    }

    public void PreviousTest()
    {
        _runner.HasUpdated = false;
        SetMapper(_demos.PreviousTest(_renderAgent));
        _runner.HasUpdated = true;
    }
    public void ReloadTest()
    {
        _runner.HasUpdated = false;
        SetMapper(_demos.Reload(_renderAgent));
        _runner.HasUpdated = true;
    }
    public void NextTest()
    {
        _runner.HasUpdated = false;
        SetMapper(_demos.NextTest(_renderAgent));
        _runner.HasUpdated = true;
    }

    private void OnMouseDown(object? sender, MouseEventArgs e) { if (_renderAgent.MouseDown(e.ToMouseArgs())) { NeedsUpdate(); } }
    private void OnMouseMove(object? sender, MouseEventArgs e) { if (_renderAgent.MouseMove(e.ToMouseArgs())) { NeedsUpdate(); } }
    private void OnMouseUp(object? sender, MouseEventArgs e) { if (_renderAgent.MouseUp(e.ToMouseArgs())) { NeedsUpdate(); } }
    private void OnMouseDoubleClick(object? sender, MouseEventArgs e) { if (_renderAgent.MouseDoubleClick(e.ToMouseArgs())) { NeedsUpdate(); } }
    private void OnMouseWheel(object? sender, MouseEventArgs e) { if (_renderAgent.MouseWheel(e.ToMouseArgs())) { NeedsUpdate(); } }


    private void OnPreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
    {
        if (e.KeyData == Sys.Keys.Right || e.KeyData == Sys.Keys.Left || e.KeyData == Sys.Keys.Down || e.KeyData == Sys.Keys.Up) // todo: can't figure out why arrow keys aren't passed, passing manually for now but may lead to double invoke.
        {
            var ea = new KeyEventArgs(e.KeyData);
            //if (_mouseAgent.KeyDown(ea.ToKeyArgs())) { NeedsUpdate(); }
        }
    }
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        //if (_mouseAgent.KeyDown(e.ToKeyArgs())) { NeedsUpdate(); }
        e.SuppressKeyPress = true; // ### Don't do this if eventually using menus etc. This supresses the alt'n sound the system gives thinking it can't find a menu item.
    }
    private void OnKeyUp(object? sender, KeyEventArgs e) 
    { 
        if (_renderAgent == null)// || _mouseAgent.KeyUp(e.ToKeyArgs())) 
        { 
            NeedsUpdate(); 
        } 
    }

    public void NeedsUpdate()
    {
        _control.Invalidate();
        _runner.NeedsUpdate();
    }

}
