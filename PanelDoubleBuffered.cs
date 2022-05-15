namespace Savan
{
    public class PanelDoubleBuffered : System.Windows.Forms.Panel
    {
        public PanelDoubleBuffered()
        {
            DoubleBuffered = true;
            UpdateStyles();
        }
    }
}
