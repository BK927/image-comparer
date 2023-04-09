using System;
using System.Windows.Forms;

namespace ImageComparer
{
    public class TagFlowLayoutPanel : FlowLayoutPanel
    {
        // Define the custom event
        public event EventHandler ChildTextChanged;

        // Override OnControlAdded method
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control is TextBox textBox)
            {
                textBox.TextChanged += ChildTextBox_TextChanged;
            }
        }

        // Override OnControlRemoved method
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            if (e.Control is Button button)
            {
                button.TextChanged -= ChildTextBox_TextChanged;
            }
        }

        // Raise the custom event
        private void ChildTextBox_TextChanged(object sender, EventArgs e)
        {
            ChildTextChanged?.Invoke(sender, e);
        }
    }
}
