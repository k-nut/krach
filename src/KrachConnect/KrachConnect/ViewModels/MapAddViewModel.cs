using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
  class MapAddViewModel : PropertyChangedBase
  {
    private NoiseRepository repository;
    private IEnumerable<MeasuringPointViewModel> measuringPoints;
    private NoiseMeasurement newNoiseMeasurement;
    private Stack<MeasuringPointViewModel> unusedMearingPoints;
    private MeasuringPointViewModel selectedMeasuringPoint;

    public MapAddViewModel(NoiseRepository repository)
    {
      this.repository = repository;
      measuringPoints = repository.MeasuringPoints;
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

    public void AddNoiseMeasurementToSelectedMeasuringPoint()
    {
      newNoiseMeasurement.MeasuringPoint = SelectedMeasuringPoint.Model;
      var oldNewNoiseMearument = newNoiseMeasurement;
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
  }
}
