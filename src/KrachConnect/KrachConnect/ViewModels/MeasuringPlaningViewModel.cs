using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
  internal class MeasuringPlaningViewModel : PropertyChangedBase
  {
    private readonly NoiseRepository repository;
    private ObservableCollection<MeasuringPointViewModel> measuringPointViewModels;
    private MeasuringPointViewModel selectedMeasuringPoint;

    public MeasuringPlaningViewModel(NoiseRepository repository)
    {
      this.repository = repository;
      MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels);
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

    public void ToggleSelection(object dataContext)
    {
      var measuringPoint = (MeasuringPointViewModel) dataContext;
      measuringPoint.IsSelected = !measuringPoint.IsSelected;
    }

    public void Save()
    {
      List<MeasuringPointViewModel> selectedMeasuringPoints = MeasuringPoints.Where(mp => mp.IsSelected).ToList();
      foreach (MeasuringPointViewModel measuringPointViewModel in selectedMeasuringPoints)
      {
        measuringPointViewModel.IsSelected = false;
      }
      repository.MeasuringWalk = selectedMeasuringPoints;
    }
  }
}