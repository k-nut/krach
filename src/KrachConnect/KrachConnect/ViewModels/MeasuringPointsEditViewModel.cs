using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
  class MeasuringPointsEditViewModel : PropertyChangedBase
  {
    private NoiseRepository repository;
    private ObservableCollection<MeasuringPointViewModel> measuringPointViewModels; 

    public MeasuringPointsEditViewModel(NoiseRepository repository)
    {
      this.repository = repository;
      MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPoints);
    }

    public ObservableCollection<MeasuringPointViewModel> MeasuringPoints
    {
      get { return measuringPointViewModels; }
      set
      {
        measuringPointViewModels = value;
        NotifyOfPropertyChange(() => MeasuringPoints);
      }
    }

    public void ToggleArchivation(object dataContext)
    {
      var measuringPoint = (MeasuringPointViewModel) dataContext;
      measuringPoint.IsArchived = !measuringPoint.IsArchived;
      repository.Save();
    }
  }
}
