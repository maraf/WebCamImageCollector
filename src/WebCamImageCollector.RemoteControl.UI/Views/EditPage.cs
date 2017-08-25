using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace WebCamImageCollector.RemoteControl.Views
{
    public class EditPage : NavigationPage
    {
        public bool IsNewRecord { get; protected set; }

        protected void SelectText(TextBox target)
        {
            if (IsNewRecord)
                target.SelectAll();
            else
                target.Select(target.Text.Length, 0);
        }
    }
}
