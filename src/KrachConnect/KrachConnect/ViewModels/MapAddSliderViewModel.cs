using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
  internal class MapAddSliderViewModel : PropertyChangedBase
  {
    private readonly Stack<MeasuringPointViewModel> unusedMearingPoints;
    private IEnumerable<MeasuringPointViewModel> measuringPoints;
    private NoiseMeasurement newNoiseMeasurement;
    private NoiseRepository repository;
    private MeasuringPointViewModel selectedMeasuringPoint;

    public MapAddSliderViewModel(NoiseRepository repository)
    {
      this.repository = repository;
      measuringPoints = repository.MeasuringPointViewModels;
      unusedMearingPoints = new Stack<MeasuringPointViewModel>(measuringPoints);
      newNoiseMeasurement = new NoiseMeasurement();
      SelectedMeasuringPoint = unusedMearingPoints.Pop();
      SelectedMeasuringPoint.IsSelected = true;
      MeasuringPoints = MeasuringPoints.Select(mp =>
      {
        mp.IsSelected = mp.Model == SelectedMeasuringPoint.Model;
        return mp;
      });
    }

    public MeasuringPointViewModel SelectedMeasuringPoint
    {
      get { return selectedMeasuringPoint; }
      set
      {
        selectedMeasuringPoint = value;
        NotifyOfPropertyChange(() => SelectedMeasuringPoint);
      }
    }

    public NoiseMeasurement NewNoiseMeasurement
    {
      get { return newNoiseMeasurement; }
      set
      {
        newNoiseMeasurement = value;
        NotifyOfPropertyChange(() => NewNoiseMeasurement);
      }
    }

    public bool CanClick
    {
      get { return unusedMearingPoints.Count > 0; }
    }

    public Visibility AllDone
    {
      get { return unusedMearingPoints.Count > 0 ? Visibility.Hidden : Visibility.Visible; }
    }

    public IEnumerable<MeasuringPointViewModel> MeasuringPoints
    {
      get { return measuringPoints; }
      set
      {
        measuringPoints = value;
        NotifyOfPropertyChange(() => MeasuringPoints);
      }
    }

    public void AddNoiseMeasurementToSelectedMeasuringPoint()
    {
      newNoiseMeasurement.MeasuringPoint = SelectedMeasuringPoint.Model;
      NoiseMeasurement oldNewNoiseMearument = newNoiseMeasurement;
      NewNoiseMeasurement = new NoiseMeasurement
      {
        MeasurementDate = oldNewNoiseMearument.MeasurementDate,
        Employee = oldNewNoiseMearument.Employee,
        Method = oldNewNoiseMearument.Method
      };
      SelectNextMeasuringPoint();
    }

    public void SelectNextMeasuringPoint()
    {
      SelectedMeasuringPoint.IsSelected = false;
      SelectedMeasuringPoint = unusedMearingPoints.Pop();
      SelectedMeasuringPoint.IsSelected = true;
      MeasuringPoints = MeasuringPoints.Select(mp =>
      {
        mp.IsSelected = mp.Model == SelectedMeasuringPoint.Model;
        return mp;
      });

      NotifyOfPropertyChange(() => SelectedMeasuringPoint);
      NotifyOfPropertyChange(() => CanClick);
      NotifyOfPropertyChange(() => AllDone);
      NotifyOfPropertyChange(() => MeasuringPoints);
    }
  }
}