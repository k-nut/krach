using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
    internal class MeasuringPointsEditViewModel : PropertyChangedBase
    {
        private readonly NoiseRepository repository;
        private ObservableCollection<MeasuringPointViewModel> measuringPointViewModels;
        private MeasuringPointViewModel selectedMeasuringPoint;
      private bool showActive = true;
      private bool showArchived = false;


        public MeasuringPointsEditViewModel(NoiseRepository repository)
        {
            this.repository = repository;
            MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels);
            SelectedMeasuringPoint = MeasuringPoints.Any() ? MeasuringPoints.First() : new MeasuringPointViewModel(new MeasuringPoint());
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

      public IEnumerable<MeasuringPointViewModel>  FilteredMeasuringPoints
      {
        get
        {
          IEnumerable<MeasuringPointViewModel> filtered;
          if (ShowActive && !ShowArchived)
          {
            filtered = MeasuringPoints.Where(mp => !mp.IsArchived);
          }
          else if (!ShowActive && ShowArchived)
          {
            filtered = MeasuringPoints.Where(mp => mp.IsArchived);
          }
          else if (!ShowActive && !ShowArchived)
          {
            filtered = new List<MeasuringPointViewModel>();
          }
          else
          {
            filtered = MeasuringPoints;
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
            var oldName = measuringPointViewModel.Name;
            var oldDescription = measuringPointViewModel.Notes;
            var oldIsArchived = measuringPointViewModel.IsArchived;
            var box = new ConfirmationBoxViewModel(measuringPointViewModel);
            var result = new WindowManager().ShowDialog(box);
            if (result == true)
            {
                // OK was clicked
                SaveToHub();
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
                Name = "Neuer Messpunkt"
            });
            var box = new ConfirmationBoxViewModel(SelectedMeasuringPoint);
            var result = new WindowManager().ShowDialog(box);
            if (result == true)
            {
                // OK was clicked
                MeasuringPoints.Add(SelectedMeasuringPoint);
                repository.MeasuringPoints.Add(SelectedMeasuringPoint.Model);
                SaveToHub();
            }
        }
    }
}