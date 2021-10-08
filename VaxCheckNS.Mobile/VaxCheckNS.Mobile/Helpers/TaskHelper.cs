using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VaxCheckNS.Mobile.Helpers
{
    public static class TaskHelper
    {
        public static async void FireAndForgetSafeAsync(this Task task, Action<Exception> handleErrorAction = null)
        {
            try
            {
                await task.ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                handleErrorAction?.Invoke(ex);
            }
        }
    }
}
