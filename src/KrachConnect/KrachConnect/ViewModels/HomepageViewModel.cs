using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using KrachConnect.DomainModelService;


namespace KrachConnect.ViewModels
{
  class HomepageViewModel : PropertyChangedBase
  {
      private NoiseRepository repository;
      private ObservableCollection<NoiseMeasurementViewModel> noiseMeasurements;

      public HomepageViewModel(NoiseRepository repository)
      {
          this.repository = repository;
          var noiseViewModels = repository.NoiseMeasurements.Select(nm => new NoiseMeasurementViewModel(nm));
          NoiseMeasurements = new ObservableCollection<NoiseMeasurementViewModel>(noiseViewModels);
      }

      public ObservableCollection<NoiseMeasurementViewModel> NoiseMeasurements
      {
          get { return noiseMeasurements; }
          set
          {
              noiseMeasurements = value;
              NotifyOfPropertyChange(() => NoiseMeasurements);
          }
      }
  }

}
