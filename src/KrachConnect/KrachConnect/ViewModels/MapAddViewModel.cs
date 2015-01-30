using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
  internal class MapAddViewModel : HasMapScreen
  {
    private readonly NoiseRepository repository;
    private readonly ShellViewModel shellViewModel;
    private Visibility detailVisibility = Visibility.Visible;
    private IEnumerable<String> employees;

    private ObservableCollection<NoiseMeasurementViewModel> measurementsAddedInThisReading =
      new ObservableCollection<NoiseMeasurementViewModel>();

    private ObservableCollection<MeasuringPointViewModel> measuringPoints;
    private NoiseMeasurementViewModel newNoiseMeasurement;
    private MeasuringPointViewModel selectedMeasuringPoint;

    private string showHideContent = "verstecke Details";


    public MapAddViewModel(NoiseRepository repository, ShellViewModel shellViewModel,
      ObservableCollection<MeasuringPointViewModel> selectedMeasuringPoints) : base(repository)
    {
      this.repository = repository;
      this.shellViewModel = shellViewModel;
      MeasuringMethods = new ObservableCollection<MeasuringMethod>(repository.MeasuringMethods);
      var allEmployees =
        new HashSet<String>(repository.NoiseMeasurements.Where(nm => nm.Employee != null).Select(nm => nm.Employee));
      Employees = allEmployees;


      MeasuringPoints = selectedMeasuringPoints;

      NewNoiseMeasurement = new NoiseMeasurementViewModel(new NoiseMeasurement
      {
        MeasurementDate = DateTime.Now,
        Employee = ""
      });
      // TODO: Abfangen, wenn es keinerlei Messpunkte gibt
      SelectNextMeasuringPoint();
    }

    public override string DisplayName
    {
      get { return "MapAdd"; }
    }

    public IEnumerable<MeasuringPointViewModel> FilteredMeasuringPoints
    {
      get { return MeasuringPoints.Where(mp => mp.NoiseMap == ActiveMap); }
    }

    public string ShowHideContent
    {
      get { return showHideContent; }
      set
      {
        showHideContent = value;
        NotifyOfPropertyChange(() => ShowHideContent);
      }
    }

    public Visibility DetailVisibility
    {
      get { return detailVisibility; }
      set
      {
        detailVisibility = value;
        NotifyOfPropertyChange(() => DetailVisibility);
      }
    }

    public int TotalMeasuringPoints
    {
      get { return MeasuringPoints.Count(mp => !mp.IsArchived); }
    }

    public int CurrentNumber
    {
      get { return MeasuringPoints.Count(mp => mp.JustMeasured) + 1; }
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

    public NoiseMeasurementViewModel NewNoiseMeasurement
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
      get { return MeasuringPoints.Any(mp => !mp.JustMeasured && !mp.IsArchived); }
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

    public bool IsBackButtonEnabled
    {
      get
      {
        int index = MeasuringPoints.IndexOf(SelectedMeasuringPoint);
        return index > 0;
      }
    }

    public ObservableCollection<MeasuringMethod> MeasuringMethods { get; set; }

    public IEnumerable<string> Employees
    {
      get { return employees; }
      set
      {
        employees = value;
        NotifyOfPropertyChange(() => Employees);
      }
    }

    public void AddNoiseMeasurementToSelectedMeasuringPoint()
    {
      NewNoiseMeasurement.MeasuringPoint = SelectedMeasuringPoint.Model;
      if (!repository.NoiseMeasurements.Contains(NewNoiseMeasurement.Model))
      {
        repository.NoiseMeasurements.Add(NewNoiseMeasurement.Model);
      }


      MeasurementsAddedInThisReading.Add(newNoiseMeasurement);
      NotifyOfPropertyChange(() => MeasurementsAddedInThisReading);

      NoiseMeasurementViewModel oldNewNoiseMearument = newNoiseMeasurement;
      NewNoiseMeasurement = createNewNoiseMeasurementWithStaticValuesFromOldOne(oldNewNoiseMearument);


      SelectedMeasuringPoint.IsSelected = false;
      SelectedMeasuringPoint.JustMeasured = true;
      NotifyOfPropertyChange(() => CanClick);
      NotifyOfPropertyChange(() => AllDone);


      if (MeasuringPoints.Any(mp => !mp.JustMeasured))
      {
        SelectNextMeasuringPoint();
      }
    }

    public void SelectNextMeasuringPoint()
    {
      SelectedMeasuringPoint = MeasuringPoints.First(mp => !mp.JustMeasured && !mp.IsArchived);

      if (SelectedMeasuringPoint.NoiseMap != ActiveMap)
      {
        ActiveMap = SelectedMeasuringPoint.NoiseMap;
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
      }

      NotifyOfPropertyChange(() => MeasurementsAddedInThisReading);
      NotifyOfPropertyChange(() => MeasuringPoints);
      NotifyOfPropertyChange(() => CurrentNumber);
      NotifyOfPropertyChange(() => IsBackButtonEnabled);
    }

    private NoiseMeasurementViewModel createNewNoiseMeasurementWithStaticValuesFromOldOne(
      NoiseMeasurementViewModel oldNoiseMeasurement)
    {
      return new NoiseMeasurementViewModel(new NoiseMeasurement
      {
        MeasurementDate = oldNoiseMeasurement.MeasurementDate,
        Employee = oldNoiseMeasurement.Employee,
        Method = oldNoiseMeasurement.MeasuringMethod,
        MinValue = oldNoiseMeasurement.MinValue,
        AverageValue = oldNoiseMeasurement.AverageValue,
        MaxValue = oldNoiseMeasurement.MaxValue
      });
    }


    public void ChangeSelectedMeasuringPoint(object dataContext)
    {
      var measuringPointViewModel = (MeasuringPointViewModel) dataContext;
      MeasuringPoint measuringPoint = measuringPointViewModel.Model;
      if (measuringPointViewModel.IsArchived)
      {
        return;
      }
      try
      {
        NoiseMeasurementViewModel correspondingNoiseMeasurement =
          measurementsAddedInThisReading.First(maitr => maitr.MeasuringPoint == measuringPoint);
        NewNoiseMeasurement = correspondingNoiseMeasurement;
      }
      catch (InvalidOperationException e)
      {
        NewNoiseMeasurement = createNewNoiseMeasurementWithStaticValuesFromOldOne(NewNoiseMeasurement);
      }
      SetSelectedMeasuringPoint(measuringPointViewModel);
    }

    private void SetSelectedMeasuringPoint(MeasuringPointViewModel measuringPointViewModel)
    {
      SelectedMeasuringPoint.IsSelected = false;
      SelectedMeasuringPoint = measuringPointViewModel;

      if (SelectedMeasuringPoint.NoiseMap != ActiveMap)
      {
        ActiveMap = SelectedMeasuringPoint.NoiseMap;
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
      }

      NotifyOfPropertyChange(() => IsBackButtonEnabled);
    }

    public void SaveToHub()
    {
      repository.Save();
    }

    public void ToggleDetails()
    {
      if (DetailVisibility == Visibility.Visible)
      {
        DetailVisibility = Visibility.Collapsed;
        ShowHideContent = "zeige Details";
      }
      else
      {
        DetailVisibility = Visibility.Visible;
        ShowHideContent = "verstecke Details";
      }
    }

    public void SelectPreviousMeasuringPoint()
    {
      int index = MeasuringPoints.IndexOf(SelectedMeasuringPoint);

      SetSelectedMeasuringPoint(MeasuringPoints[index - 1]);
    }

    public void CancelMeasuring()
    {
      shellViewModel.ShowHomePageScreen();
    }
  }
}