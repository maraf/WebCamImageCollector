using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using Windows.UI.Xaml.Controls;

namespace WebCamImageCollector.RemoteControl.Views
{
    public class ImageZoom
    {
        private float? zoomFactor;
        private int? width;
        private int? height;
        private double? horizontalOffset;
        private double? verticalOffset;

        public void Apply(ScrollViewer view, ClientImageModel model)
        {
            if (zoomFactor != null)
            {
                if (width != null && width != model.Width)
                {
                    float ratio = (float)width / model.Width;
                    zoomFactor = Math.Abs(zoomFactor.Value * ratio);
                }

                view.ChangeView(horizontalOffset, verticalOffset, zoomFactor);
            }

            width = model.Width;
            height = model.Height;
        }

        public void Save(ScrollViewer view)
        {
            zoomFactor = view.ZoomFactor;
            horizontalOffset = view.HorizontalOffset;
            verticalOffset = view.VerticalOffset;
        }
    }
}
