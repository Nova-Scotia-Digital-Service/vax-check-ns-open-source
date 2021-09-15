using ProofOfVaccine.Mobile.Helpers;
using ProofOfVaccine.Mobile.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.ViewModels
{
    public class BaseViewModel : BaseBindable
    {
        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public Command GoBackCommand { get; set; }
        public Command ScanCommand { get; set; }

        private readonly IErrorManagementService _errorManagementService;
        public BaseViewModel()
        {
            _errorManagementService = DependencyService.Resolve<IErrorManagementService>();

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            ScanCommand = new Command(async () => await Shell.Current.GoToAsync("ScanPage"));
        }

        #region Busy Mechanism
        private static readonly Guid _defaultTracker = new Guid("A7848922-C10A-4D6A-9D82-4987F638F718");
        private IList<Guid> _busyLocks = new List<Guid>();

        public bool IsBusy
        {
            get => _busyLocks.Any();
            set
            {
                if (value && !_busyLocks.Contains(_defaultTracker))
                {
                    _busyLocks.Add(_defaultTracker);
                    RaisePropertyChanged(nameof(IsBusy));
                }

                if (!value && _busyLocks.Contains(_defaultTracker))
                {
                    _busyLocks.Remove(_defaultTracker);
                    RaisePropertyChanged(nameof(IsBusy));
                }
            }
        }

        protected BusyHelper Busy() => new BusyHelper(this);

        private void ForceUnlock()
        {
            _busyLocks = new List<Guid>();
        }

        protected sealed class BusyHelper : IDisposable
        {
            private readonly BaseViewModel _viewModel;
            private readonly Guid _tracker;

            public BusyHelper(BaseViewModel viewModel)
            {
                _viewModel = viewModel;
                _tracker = Guid.NewGuid();
                _viewModel._busyLocks.Add(_tracker);
                _viewModel.RaisePropertyChanged(nameof(IsBusy));
            }

            public void Dispose()
            {
                _viewModel._busyLocks.Remove(_tracker);
                _viewModel.RaisePropertyChanged(nameof(IsBusy));
            }
        }
        #endregion
    }
}
