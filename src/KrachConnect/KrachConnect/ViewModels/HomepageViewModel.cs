using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
  internal class HomepageViewModel : PropertyChangedBase
  {
    private IEnumerable<NoiseMeasurementViewModel> noiseMeasurements;

    public HomepageViewModel(NoiseRepository repository)
    {
      var noiseViewModels =
        repository.NoiseMeasurements.Select(nm => new NoiseMeasurementViewModel(nm))
          .ToList();
      var newestDate = noiseViewModels.Max(nm => nm.MeasurementDate);
      var newestMeasurements = noiseViewModels.Where(nm => nm.MeasurementDate == newestDate);
      NoiseMeasurements = new ObservableCollection<NoiseMeasurementViewModel>(newestMeasurements);
    }

    public IEnumerable<NoiseMeasurementViewModel> NoiseMeasurements
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