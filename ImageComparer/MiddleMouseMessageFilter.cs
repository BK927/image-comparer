using System.Drawing;
using System.Windows.Forms;

public class MiddleMouseMessageFilter : IMessageFilter
{
    public delegate void MiddleMouseButtonEventHandler(object sender, MouseEventArgs e);
    public event MiddleMouseButtonEventHandler MiddleMouseClick;

    private const int WM_MBUTTONDOWN = 0x0207;
    private const int WM_MBUTTONUP = 0x0208;

    private Control _excludedControl;

    public MiddleMouseMessageFilter(Control excludedControl)
    {
        _excludedControl = excludedControl;
    }

    public bool PreFilterMessage(ref Message m)
    {
        if (m.Msg == WM_MBUTTONDOWN || m.Msg == WM_MBUTTONUP)
        {
            MouseEventArgs args = new MouseEventArgs(MouseButtons.Middle, 0, Cursor.Position.X, Cursor.Position.Y, 0);

            if (IsMouseOverControl(_excludedControl))
            {
                return false;
            }

            if (m.Msg == WM_MBUTTONUP && MiddleMouseClick != null)
            {
                MiddleMouseClick(null, args);
            }
            return true;
        }
        return false;
    }

    private bool IsMouseOverControl(Control control)
    {
        if (control == null)
        {
            return false;
        }
        Point clientPoint = control.PointToClient(Cursor.Position);
        return control.ClientRectangle.Contains(clientPoint);
    }
}
