using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private ObservableCollection<MeasuringPointViewModel> measuringPoints;
    private NoiseMeasurement newNoiseMeasurement;
    private MeasuringPointViewModel selectedMeasuringPoint;
    private ObservableCollection<NoiseMeasurementViewModel> measurementsAddedInThisReading = new ObservableCollection<NoiseMeasurementViewModel>();


    public int TotalMeasuringPoints { get { return MeasuringPoints.Count(); } }

    public int CurrentNumber
    {
      get { return MeasuringPoints.Count(mp => mp.JustMeasured) + 1; }
    }

    public MapAddViewModel(NoiseRepository repository)
    {
      this.repository = repository;
      MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPoints);
      newNoiseMeasurement = new NoiseMeasurement
      {
        MeasurementDate = DateTime.Now
      };
      MeasuringPoints.First(mp => !mp.JustMeasured).IsSelected = true;

    }

    public MeasuringPointViewModel SelectedMeasuringPoint
    {
      get { return MeasuringPoints.First(mp => !mp.JustMeasured); }
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

      MeasurementsAddedInThisReading.Add(new NoiseMeasurementViewModel(newNoiseMeasurement));
      NotifyOfPropertyChange(() => MeasurementsAddedInThisReading);

      var oldNewNoiseMearument = newNoiseMeasurement;
      NewNoiseMeasurement = new NoiseMeasurement
      {
        MeasurementDate = oldNewNoiseMearument.MeasurementDate,
        Employee = oldNewNoiseMearument.Employee,
        Method = oldNewNoiseMearument.Method
      };
      
      SelectedMeasuringPoint.IsSelected = false;
      SelectedMeasuringPoint.JustMeasured = true;
      NotifyOfPropertyChange(() => MeasuringPoints);
      if (MeasuringPoints.Count(mp => !mp.JustMeasured) > 0){
      SelectNextMeasuringPoint();
      }
    }

    public void SelectNextMeasuringPoint()
    {
      MeasuringPoints.First(mp => !mp.JustMeasured).IsSelected = true;

      NotifyOfPropertyChange(() => SelectedMeasuringPoint);
      NotifyOfPropertyChange(() => MeasurementsAddedInThisReading);
      NotifyOfPropertyChange(() => CanClick);
      NotifyOfPropertyChange(() => AllDone);
      NotifyOfPropertyChange(() => MeasuringPoints);
      NotifyOfPropertyChange(() => CurrentNumber);

    }

    public bool CanClick
    {
      get { return MeasuringPoints.Count(mp => !mp.JustMeasured) > 0; }
    }

    public ObservableCollection<NoiseMeasurementViewModel> MeasurementsAddedInThisReading
    {
      get { return measurementsAddedInThisReading; }
      set
      {
        measurementsAddedInThisReading = value;
        NotifyOfPropertyChange(() => MeasurementsAddedInThisReading);
      }
    } 

    public Visibility AllDone
    {
      get { return MeasuringPoints.Count(mp => !mp.JustMeasured) > 0 ? Visibility.Hidden : Visibility.Visible; }
    }

    public ObservableCollection<MeasuringPointViewModel> MeasuringPoints
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
