using NumericsSkia.Drawing;
using NumericsSkia.Renderer;
using NumericsSkia.Utils;

using NumericsAPI.CommandEngine;
using NumericsCore.Primitives;
using NumericsCore.Utils;
using SkiaSharp;
using Numerics.Primitives;
using NumericsCore.Sequencer;
using NumericsSkia.Mappers;
using Numerics.CoreConcepts.Time;

namespace NumericsSkia.Agent;

public class RenderAgent : CommandAgent, IMouseAgent
{
    #region Properties

    public CoreRenderer Renderer { get; }
    public Runner Runner;
    public SKMapper Mapper;
    public bool HasFocus { get; set; }
    public string Text = "";

    public IDemos Demos { get; }
    public bool IsPaused { get; set; } = true;
    public bool IsDown { get; private set; }
    public bool IsDragging { get; private set; }

    public event EventHandler OnModeChange;
    //public event EventHandler OnDisplayModeChange;
    public event EventHandler OnSelectionChange;

    public bool DoSyncMatchingBasis { get; set; } = true;

    private ColorTheme _colorTheme = ColorTheme.Normal;
    public ColorTheme ColorTheme
    {
        get => _colorTheme;
        set
        {
            if (_colorTheme != value)
            {
                _colorTheme = value;
                Renderer.GeneratePens(_colorTheme);
            }
        }
    }

    private SKPoint _rawMousePoint;
    private float _minDragDistance = 4f;
    public float ScaleTickSize { get; set; } = 0.2f;
    public Keys CurrentKey { get; private set; }
    private UIMode _uiMode = UIMode.Any;
    public UIMode UIMode
    {
        get => _uiMode;
        set
        {
            if (value != _uiMode)
            {
                _uiMode = value;
                OnModeChange?.Invoke(this, new EventArgs());
                SetSelectable(UIMode);
            }
        }
    }

    private Dictionary<int, PRange> SavedNumbers { get; } = new Dictionary<int, PRange>();
    #endregion

    public RenderAgent(CoreRenderer renderer, IDemos demos)
    {
        Renderer = renderer;
        Renderer.Agent = this;
        Runner = new Runner(this);
        Stack.CurrentTime.SetWith(Runner.CurrentMS);

        Demos = demos;// new Demos(Brain, Renderer);

        ClearMouse();
    }

    public override void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
    {
        Stack.Update(currentTime, deltaTime);
    }
    #region Mode
    private void SetSelectable(UIMode uiMode)
    {
        switch (uiMode)
        {
            case UIMode.None:
            case UIMode.Any:
                //_selectableKind = ElementKind.Any;
                break;
            //case UIMode.CreateEntity:
            //    _selectableKind = ElementKind.Any;
            //    break;
            //case UIMode.CreateTrait:
            //    _selectableKind = ElementKind.TraitPart;
            //    break;
            //case UIMode.CreateFocal:
            //    _selectableKind = _isControlDown ? ElementKind.TraitPart : ElementKind.FocalPart;
            //    break;
            //case UIMode.CreateBond:
            //    _selectableKind = _isControlDown ? ElementKind.FocalPart : ElementKind.BondPart;
            //    break;
            case UIMode.SetUnit:
                //_selectableKind = ElementKind.Focal;
                break;
                //case UIMode.Equal:
                //    _selectableKind = ElementKind.Focal;
                //    Data.Selected.Clear();
                //    break;
        }
    }
    #endregion

    #region Mouse
    public SKPoint DragPoint;

    public bool MouseDown(MouseArgs e)
    {
        if (IsPaused) { return false; }

        // Add to selection if ctrl down etc.
        _rawMousePoint = e.Location;
        var mousePoint = GetTransformedPoint(_rawMousePoint);

        IsDown = true;
        return true;
    }

    public bool MouseMove(MouseArgs e)
    {
        if (IsPaused) { return false; }

        var result = false;
        _rawMousePoint = e.Location;
        var mousePoint = GetTransformedPoint(_rawMousePoint);

        if (IsDown)
        {
            result = MouseDrag(mousePoint);
        }
        return true;
    }

    public bool MouseDrag(SKPoint mousePoint)
    {
        if (IsPaused) { return false; }


        if (IsDragging)
        {
            //var activeKind = activeHighlight.Kind;
        }
        return true;
    }

    public bool MouseUp(MouseArgs e)
    {
        if (IsPaused) { return false; }

        _rawMousePoint = e.Location;
        var mousePoint = GetTransformedPoint(_rawMousePoint);


        OnSelectionChange?.Invoke(this, new EventArgs());
        ClearMouse();

        return true;
    }
    public bool MouseDoubleClick(MouseArgs e)
    {
        if (IsPaused) { return false; }

        if (CurrentKey == Keys.Space && e.Button == MouseButtons.Left)
        {
            //Data.ResetZoom();
        }
        return true;
    }
    public bool MouseWheel(MouseArgs e)
    {
        if (IsPaused) { return false; }

        var scale = 1f + (Math.Sign(e.Delta) * ScaleTickSize);
        var rawMousePoint = e.Location;
        var mousePoint = GetTransformedPoint(_rawMousePoint);
        return true;
    }
    public void ClearMouse()
    {
        IsDown = false;
        IsDragging = false;
        SavedNumbers.Clear();
        SetSelectable(UIMode);
        DragPoint = SKPoint.Empty;
    }
    #endregion

    #region Commands
    #endregion


    #region Render
    public virtual void Draw()
    {
    }
    public SKPoint GetTransformedPoint(SKPoint point) => point; // will be matrix etc
    #endregion

    #region Serialize
    public void SaveNumberValues(Dictionary<int, PRange> numValues, params int[] ignoreIds)
    {
        numValues.Clear();
        //foreach (var kvp in Workspace.AddDomains)
        //{
        // if (!ignoreIds.Contains(kvp.Key))
        // {
        //  numValues.Add(kvp.Key, kvp.Value.Value);
        // }
        //}
    }
    public void RestoreNumberValues(Dictionary<int, PRange> numValues, params int[] ignoreIds)
    {
        //foreach (var kvp in numValues)
        //{
        // var id = kvp.Key;
        // var storedValue = kvp.Value;
        // if (!ignoreIds.Contains(id))
        // {
        //  Brain.NumberStore[id].Value = storedValue;
        // }
        //}
    }
    #endregion

    public override void ClearAll()
    {
        base.ClearAll();
        ClearMouse();
        Runner.Clear();
    }
}
