using System;
using VaxCheckNS.Mobile.Helpers;

namespace VaxCheckNS.Mobile.BindableModels
{
    public class CarouselItem : BaseBindable
    {
        private string _imageSource;
        private string _caption;

        public string Caption
        {
            get { return _caption; }
            set { SetProperty(ref _caption, value); }
        }

        public string ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        public CarouselItem(string caption, string imageSource)
        {
            Caption = caption;
            ImageSource = imageSource;
        }
    }
}
