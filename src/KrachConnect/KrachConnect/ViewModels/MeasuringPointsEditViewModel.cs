using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
  class MeasuringPointsEditViewModel : PropertyChangedBase
  {
    private NoiseRepository repository;
    private ObservableCollection<MeasuringPointViewModel> measuringPointViewModels; 

    public MeasuringPointsEditViewModel(NoiseRepository repository)
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

    public void ToggleArchivation(object dataContext)
    {
      var measuringPoint = (MeasuringPointViewModel) dataContext;
      measuringPoint.IsArchived = !measuringPoint.IsArchived;
      repository.Save();
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
          var measuringPoint = new MeasuringPoint
          {
              Name = "Neuer Messpunkt",
              Position = newPosition
          };
          MeasuringPoints.Add(new MeasuringPointViewModel(measuringPoint));
          repository.MeasuringPoints.Add(measuringPoint);
          repository.Save();
      }
  }
}
