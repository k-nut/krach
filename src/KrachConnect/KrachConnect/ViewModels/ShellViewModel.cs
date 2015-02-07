using System;
using System.Collections.ObjectModel;
using System.Security;
using System.Windows.Documents;
using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
    public class ShellViewModel : Conductor<Object>
    {
        private readonly NoiseRepository nr = new NoiseRepository();
        private bool _isEnabled;


        public ShellViewModel()
        {
            ActivateItem(new HomepageViewModel(nr));
            IsEnabled = true;
            DisplayName = "Krach";
        }

        public void ShowMapScreen(ObservableCollection<MeasuringPointViewModel> selectedMeasuringPoints)
        {
            ActivateItem(new MapAddViewModel(nr, this, selectedMeasuringPoints));
        }


        public void ShowHomePageScreen()
        {
            ActivateItem(new HomepageViewModel(nr));
            IsEnabled = true;
        }


        public void ShowMeasuringPointsEditScreen()
        {
            ActivateItem(new MeasuringPointsEditViewModel(nr));
            IsEnabled = true;
        }

        public void ShowMeasuringPlaningScreen()
        {
            ActivateItem(new MeasuringPlaningViewModel(nr, this));
            IsEnabled = false;
        }

        public void ShowAlternativeEvaluationScreen()
        {
            ActivateItem(new AlternativeEvaluationViewModel(nr));
            IsEnabled = true;
        }

        public void ShowMapEvaluationScreen()
        {
            ActivateItem(new MapEvaluationViewModel(nr));
            IsEnabled = true;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value.Equals(_isEnabled)) return;
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }
    }
}