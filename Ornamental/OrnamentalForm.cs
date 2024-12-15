using SkiaSharp.Views.Desktop;
using NumericsSkia.Renderer;
using NumericsCore.Sequencer;

namespace Ornamental
{
    public partial class OrnamentalForm : Form
    {
        private readonly CoreRenderer _renderer;
        private readonly SKControl _control;
        //private readonly MouseAgent _mouseAgent;
        private Runner _runner;
        //private Workspace _workspace;

        public OrnamentalForm()
        {
            InitializeComponent();
        }
    }
}
