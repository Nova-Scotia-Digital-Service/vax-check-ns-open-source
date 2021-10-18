using System;
using System.Collections.ObjectModel;
using VaxCheckNS.Mobile.BindableModels;
using VaxCheckNS.Mobile.Helpers;
using VaxCheckNS.Mobile.AppResources;
using Xamarin.Forms;
using VaxCheckNS.Mobile.Services;

namespace VaxCheckNS.Mobile.ViewModels
{
    public class TutorialViewModel : BaseViewModel
    {
        private new readonly IDataService _dataService;
        private bool _isFinalTutorialPosition;

        public ObservableCollection<CarouselItem> TutorialItems { get; set; }
        public Command DoneCommand { get; set; }
        public Command SkipCommand { get; set; }
        public Command TutorialPositionChangedCommand { get; set; }

        public bool IsFinalTutorialPosition
        {
            get { return _isFinalTutorialPosition; }
            set { SetProperty(ref _isFinalTutorialPosition,value); }
        }

        public TutorialViewModel()
        {
            _dataService = DependencyService.Resolve<IDataService>();

            DoneCommand = new Command(() => Done());
            SkipCommand = new Command(() => Skip());
            TutorialPositionChangedCommand = new Command((position) => VerifyIsLastTutorialPosition(position));

            var translationHelper = new TranslationHelper();
            string tutorial1Text = (string)translationHelper.ProvideValue(nameof(TextResources.Tutorial1Text));
            string tutorial2Text = (string)translationHelper.ProvideValue(nameof(TextResources.Tutorial2Text));
            string tutorial3Text = (string)translationHelper.ProvideValue(nameof(TextResources.Tutorial3Text));
            string tutorial4Text = (string)translationHelper.ProvideValue(nameof(TextResources.Tutorial4Text));

            TutorialItems = new ObservableCollection<CarouselItem>
            {
                new CarouselItem(tutorial1Text, "Logo.png"),
                new CarouselItem(tutorial2Text, "Logo.png"),
                new CarouselItem(tutorial3Text, "Logo.png"),
                new CarouselItem(tutorial4Text, "Logo.png"),
            };
        }

        public void Done()
        {
            _dataService.CompleteTutorial();
            GoTo("///MainFlow");
        }

        public void Skip()
        {
            _dataService.CompleteTutorial();
            GoTo("///MainFlow");
        }

        public void VerifyIsLastTutorialPosition(object o)
        {
            if(o is int position)
            {
                IsFinalTutorialPosition = position + 1 == TutorialItems.Count;
            }
        }
    }
}
