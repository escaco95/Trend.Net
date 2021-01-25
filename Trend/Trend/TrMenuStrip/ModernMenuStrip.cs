using System.Drawing;
using System.Windows.Forms;

namespace Trend
{
    /// <summary>
    /// A trendy modern <seealso cref="MenuStrip"/> control.
    /// </summary>
    public class ModernMenuStrip : MenuStrip
    {
        public ModernMenuStrip()
        {
            m_Renderer = new ModernMenuStripRenderer();
            this.Renderer = m_Renderer;
        }

        private ModernMenuStripRenderer m_Renderer;

        public Color A
        {
            get => m_Renderer.ColorTable.ButtonSelectedBorder;
        }
    }
    class ModernMenuStripRenderer : ToolStripProfessionalRenderer
    {
        public ModernMenuStripRenderer() : base(new ModernMenuStripColorTable())
        {

        }

        public ProfessionalColorTable Table => base.ColorTable;
    }
    class ModernMenuStripColorTable : ProfessionalColorTable
    {

    }
}
