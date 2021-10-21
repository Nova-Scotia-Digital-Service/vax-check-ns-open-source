using System;
using System.Threading.Tasks;
using VaxCheckNS.Mobile.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.ViewModels
{
    public class PermissionsViewModel : BaseViewModel
    {
        private readonly IOpenPlatformAppSettingsHelper _openPlatformAppSettingsHelper;

        private bool _canContinue;
        public bool CanContinue
        {
            get { return _canContinue; }
            set { SetProperty(ref _canContinue, value); }
        }

        public Command OpenPlatormAppSettingsCommand { get; set; }
        public Command AcceptCommand { get; set; }

        public PermissionsViewModel()
        {
            _openPlatformAppSettingsHelper = DependencyService.Get<IOpenPlatformAppSettingsHelper>();
            OpenPlatormAppSettingsCommand = new Command(() => OpenAppSettingsForPlatform());
            AcceptCommand = new Command(async () => await Accept());
            CanContinue = true;
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            if (HasPermissions)
            {
                GoTo("///MainFlow");
            }
        }

        private void OpenAppSettingsForPlatform()
        {
            Device.BeginInvokeOnMainThread(() => _openPlatformAppSettingsHelper.OpenAppSettings());
        }

        public async Task Accept()
        {
            if (!HasPermissions)
            {
                var permissions = await Permissions.RequestAsync<Permissions.Camera>();
                HasPermissions = permissions == PermissionStatus.Granted;

                if (HasPermissions)
                {
                    GoTo("///MainFlow");
                }
                else
                {
                    CanContinue = false;
                }
            }
        }
    }
}
