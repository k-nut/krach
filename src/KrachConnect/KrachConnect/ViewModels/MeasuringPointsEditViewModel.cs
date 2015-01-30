using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using System.IO;
using KrachConnect.DomainModelService;
using OxyPlot.Reporting;
using Image = System.Drawing.Image;

namespace KrachConnect.ViewModels
{
  internal class MeasuringPointsEditViewModel : HasMapScreen
  {
    private readonly NoiseRepository repository;
    private ObservableCollection<MeasuringPointViewModel> measuringPointViewModels;
    private MeasuringPointViewModel selectedMeasuringPoint;
    private IEnumerable<NoiseMap> _maps;
    private bool showActive = true;
    private bool showArchived = false;
    public override string DisplayName { get { return "MeasuringPointsEdit"; } }


    public MeasuringPointsEditViewModel(NoiseRepository repository) :base(repository)
    {
      this.repository = repository;
      MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels);
      SelectedMeasuringPoint = MeasuringPoints.Any() ? MeasuringPoints.First() : new MeasuringPointViewModel(new MeasuringPoint());
      PropertyChanged += NotifyActiveMapChanged;

    }

    private void NotifyActiveMapChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ActiveMap")
      {
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
      }
    }

    public ObservableCollection<MeasuringPointViewModel> MeasuringPoints
    {
      get { return measuringPointViewModels; }
      set
      {
        measuringPointViewModels = value;
        NotifyOfPropertyChange(() => MeasuringPoints);
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
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

    public IEnumerable<MeasuringPointViewModel> FilteredMeasuringPoints
    {
      get
      {
        IEnumerable<MeasuringPointViewModel> filtered;
        filtered = MeasuringPoints.Where(mp => mp.NoiseMap == ActiveMap);
        if (ShowActive && !ShowArchived)
        {
          filtered = filtered.Where(mp => !mp.IsArchived);
        }
        else if (!ShowActive && ShowArchived)
        {
          filtered = filtered.Where(mp => mp.IsArchived);
        }
        else if (!ShowActive && !ShowArchived)
        {
          filtered = new List<MeasuringPointViewModel>();
        }
        return filtered;
      }
    }

    public bool ShowActive
    {
      get { return showActive; }
      set
      {
        showActive = value;
        NotifyOfPropertyChange(() => ShowActive);
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);

      }
    }

    public bool ShowArchived
    {
      get { return showArchived; }
      set
      {
        showArchived = value;
        NotifyOfPropertyChange(() => ShowArchived);
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
      }
    }

    public void SaveToHub()
    {
      repository.Save();
    }

    public void ChangeSelectedMeasuringPoint(object dataContext)
    {
      SelectedMeasuringPoint.IsSelected = false;
      var measuringPointViewModel = (MeasuringPointViewModel)dataContext;
      SelectedMeasuringPoint = measuringPointViewModel;

      var oldName = measuringPointViewModel.Name;
      var oldDescription = measuringPointViewModel.Notes;
      var oldIsArchived = measuringPointViewModel.IsArchived;

      var box = new ConfirmationBoxViewModel(measuringPointViewModel, repository);
      var result = new WindowManager().ShowDialog(box);
      if (result == true)
      {
        // OK was clicked
        SaveToHub();
        if (measuringPointViewModel.Deleted == true)
        {
          MeasuringPoints.Remove(measuringPointViewModel);
          NotifyOfPropertyChange(() => MeasuringPoints);
          NotifyOfPropertyChange(() => FilteredMeasuringPoints);
        }
      }
      else
      {
        measuringPointViewModel.Name = oldName;
        measuringPointViewModel.Notes = oldDescription;
        measuringPointViewModel.IsArchived = oldIsArchived;
      }
    }

    public void AddNewMeasuringPoint(object xPosition, object yPosition)
    {
      var x = (int)xPosition;
      var y = (int)yPosition;
      var newPosition = new NoiseMapPosition
      {
        NoiseMap = ActiveMap,
        XPosition = x - 10,
        YPosition = y - 10
        // 10 is a magic number
        // because our points are width and height 20px
        // in order to position the points in the center
        // we simply subtract half the size (10)
      };
      SelectedMeasuringPoint.IsSelected = false;
      SelectedMeasuringPoint = new MeasuringPointViewModel(new MeasuringPoint
      {
        Position = newPosition,
        Name = "Neuer Messpunkt",
      });
      var box = new ConfirmationBoxViewModel(SelectedMeasuringPoint, true);
      var result = new WindowManager().ShowDialog(box);
      if (result == true)
      {
        // OK was clicked
        MeasuringPoints.Add(SelectedMeasuringPoint);
        repository.MeasuringPoints.Add(SelectedMeasuringPoint.Model);
        NotifyOfPropertyChange(() => MeasuringPoints);
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
        SaveToHub();
      }
    }
  }
}