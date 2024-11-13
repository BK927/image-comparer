using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageComparer
{
    public static class ControlExtensions
    {
        public static Task InvokeAsync(this Control control, Action action)
        {
            var tcs = new TaskCompletionSource<object>();
            if (control.InvokeRequired)
            {
                control.BeginInvoke((MethodInvoker)delegate
                {
                    try
                    {
                        action();
                        tcs.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });
            }
            else
            {
                try
                {
                    action();
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }
            return tcs.Task;
        }
    }
}