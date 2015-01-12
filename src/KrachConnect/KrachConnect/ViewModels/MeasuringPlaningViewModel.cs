using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
  class MeasuringPlaningViewModel : PropertyChangedBase
  {
    private NoiseRepository repository;
    private ObservableCollection<MeasuringPointViewModel> measuringPointViewModels;
    private MeasuringPointViewModel selectedMeasuringPoint;

    public MeasuringPlaningViewModel(NoiseRepository repository)
    {
      this.repository = repository;
      MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels);
      SelectedMeasuringPoint = MeasuringPoints.First();
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

    public MeasuringPointViewModel SelectedMeasuringPoint
    {
      get { return selectedMeasuringPoint; }
      set
      {
        selectedMeasuringPoint = value;
        selectedMeasuringPoint.IsSelected = true;
        NotifyOfPropertyChange(() => SelectedMeasuringPoint);
      }
    }

    public void ToggleSelection(object dataContext)
    {
      var measuringPoint = (MeasuringPointViewModel)dataContext;
      measuringPoint.IsSelected = !measuringPoint.IsSelected;
    }

    public void SaveToHub()
    {
      repository.Save();
    }
  }
}
