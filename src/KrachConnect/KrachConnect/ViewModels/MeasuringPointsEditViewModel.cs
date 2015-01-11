﻿using System;
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
  class MeasuringPointsEditViewModel : PropertyChangedBase
  {
    private NoiseRepository repository;
    private ObservableCollection<MeasuringPointViewModel> measuringPointViewModels;
    private MeasuringPointViewModel selectedMeasuringPoint;

    public MeasuringPointsEditViewModel(NoiseRepository repository)
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

    public void SaveToHub()
    {
      repository.Save();
    }

    public void ToggleArchivation(object dataContext)
    {
      var measuringPoint = (MeasuringPointViewModel)dataContext;
      measuringPoint.IsArchived = !measuringPoint.IsArchived;
      repository.Save();
    }

    public void ChangeSelectedMeasuringPoint(object dataContext)
    {
      SelectedMeasuringPoint.IsSelected = false;
      var measuringPointViewModel = (MeasuringPointViewModel)dataContext;
      SelectedMeasuringPoint = measuringPointViewModel;
    }

    public void AddNewMeasuringPoint(object xPosition, object yPosition)
    {
      var x = (int)xPosition;
      var y = (int)yPosition;
      var newPosition = new NoiseMapPosition
      {
        XPosition = x - 10,
        YPosition = y - 10
        // 10 is a magic number
        // because our points are width and height 20px
        // in order to position the points in the center
        // we simply subtract half the size (10)
      };
      ChangeSelectedMeasuringPoint(new MeasuringPointViewModel(new MeasuringPoint
      {
        Position = newPosition,
        Name = "Neuer Messpunkt"
      }));
      MeasuringPoints.Add(SelectedMeasuringPoint);
      repository.MeasuringPoints.Add(SelectedMeasuringPoint.Model);
    }
  }
}