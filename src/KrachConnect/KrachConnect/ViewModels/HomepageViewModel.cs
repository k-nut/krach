using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
  internal class HomepageViewModel : PropertyChangedBase
  {
    private ObservableCollection<NoiseMeasurementViewModel> noiseMeasurements;
    private NoiseRepository repository;

    public HomepageViewModel(NoiseRepository repository)
    {
      this.repository = repository;
      IEnumerable<NoiseMeasurementViewModel> noiseViewModels =
        repository.NoiseMeasurements.Select(nm => new NoiseMeasurementViewModel(nm));
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