﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
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

        private bool? isTouchMode;

        protected bool IsTouchMode
        {
            get
            {
                if (isTouchMode == null)
                {
                    UIViewSettings settings = UIViewSettings.GetForCurrentView();
                    isTouchMode = settings.UserInteractionMode == UserInteractionMode.Touch;
                }

                return isTouchMode.Value;
            }
        }
    }
}
