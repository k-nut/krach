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

      SelectedMeasuringPoint = MeasuringPoints.First(mp => !mp.JustMeasured);

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
      if (!repository.NoiseMeasurements.Contains(NewNoiseMeasurement))
      {
        repository.NoiseMeasurements.Add(NewNoiseMeasurement);
      }


      MeasurementsAddedInThisReading.Add(new NoiseMeasurementViewModel(newNoiseMeasurement));
      NotifyOfPropertyChange(() => MeasurementsAddedInThisReading);

      var oldNewNoiseMearument = newNoiseMeasurement;
      NewNoiseMeasurement = createNewNoseMeasurementWithStaticValuesFromOldOne(oldNewNoiseMearument);

      SelectedMeasuringPoint.IsSelected = false;
      SelectedMeasuringPoint.JustMeasured = true;
      NotifyOfPropertyChange(() => CanClick);
      NotifyOfPropertyChange(() => AllDone);


      if (MeasuringPoints.Count(mp => !mp.JustMeasured) > 0)
      {
        SelectNextMeasuringPoint();
      }
    }

    private NoiseMeasurement createNewNoseMeasurementWithStaticValuesFromOldOne(NoiseMeasurement oldNoiseMeasurement)
    {
      return new NoiseMeasurement
      {
        MeasurementDate = oldNoiseMeasurement.MeasurementDate,
        Employee = oldNoiseMeasurement.Employee,
        Method = oldNoiseMeasurement.Method
      };
    }

    public void SelectNextMeasuringPoint()
    {
      SelectedMeasuringPoint = MeasuringPoints.First(mp => !mp.JustMeasured);

      NotifyOfPropertyChange(() => MeasurementsAddedInThisReading);
      NotifyOfPropertyChange(() => MeasuringPoints);
      NotifyOfPropertyChange(() => CurrentNumber);

    }

    public void SayHello(object dataContext)
    {
      var measuringPointViewModel = (MeasuringPointViewModel)dataContext;
      var measuringPoint = measuringPointViewModel.Model;
      try
      {
        var correspondingNoiseMeasurement = measurementsAddedInThisReading.First(maitr => maitr.MeasuringPoint == measuringPoint);
        NewNoiseMeasurement = correspondingNoiseMeasurement.Model;
      }
      catch (InvalidOperationException e)
      {
        NewNoiseMeasurement = createNewNoseMeasurementWithStaticValuesFromOldOne(NewNoiseMeasurement);
      }
      SetSelectedMeasuringPoint(measuringPointViewModel);

    }

    private void SetSelectedMeasuringPoint(MeasuringPointViewModel measuringPointViewModel)
    {
      SelectedMeasuringPoint.IsSelected = false;
      SelectedMeasuringPoint = measuringPointViewModel;
    }

    public void SaveToHub()
    {
      repository.Save();
    }

    public bool CanClick
    {
      get { return MeasuringPoints.Any(mp => !mp.JustMeasured); }
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
      get { return MeasuringPoints.Any(mp => !mp.JustMeasured) ? Visibility.Hidden : Visibility.Visible; }
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
