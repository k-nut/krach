using System;
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

        public void SaveToHub()
        {
            repository.Save();
        }

        public void ToggleArchivation(object dataContext)
        {
            var measuringPoint = (MeasuringPointViewModel) dataContext;
            measuringPoint.IsArchived = !measuringPoint.IsArchived;
            repository.Save();
        }

        public void ChangeSelectedMeasuringPoint(object dataContext)
        {
            SelectedMeasuringPoint.IsSelected = false;
            var measuringPointViewModel = (MeasuringPointViewModel) dataContext;
            SelectedMeasuringPoint = measuringPointViewModel;
          var box = new ConfirmationBoxViewModel(measuringPointViewModel);
          var result = new WindowManager().ShowDialog(box);
          if(result == true)
          {
          // OK was clicked
}
        }

        public void AddNewMeasuringPoint(object xPosition, object yPosition)
        {
            var x = (int) xPosition;
            var y = (int) yPosition;
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