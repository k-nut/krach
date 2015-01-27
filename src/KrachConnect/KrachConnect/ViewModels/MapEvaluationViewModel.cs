using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
  class MapEvaluationViewModel:Screen
  {
    private IEnumerable<MeasuringPointViewModel> measuringPoints;
    private IEnumerable<NoiseMeasurement> noiseMeasurements;
    private DateTime _selectedDate = DateTime.Today;
    private DateTime maxDate = DateTime.Today;


    public MapEvaluationViewModel(NoiseRepository repository)
    {
      MeasuringPoints = repository.MeasuringPointViewModels;
      noiseMeasurements = repository.NoiseMeasurements;
      MeasurementDates = new HashSet<DateTime>(noiseMeasurements.Select(nm => nm.MeasurementDate));
      SelectedDate = MeasurementDates.Max();
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

    public IEnumerable<DateTime> MeasurementDates { get; set; }

    public IEnumerable<MeasuringPointWithValue> FilteredMeasuringPoints
    {
      get
      {
        var measurementsForSelectedDate = noiseMeasurements.Where(nm => nm.MeasurementDate == SelectedDate);
        var all = new List<MeasuringPointWithValue>();
        foreach (var measurement in measurementsForSelectedDate)
        {
          var measuringPoint = measurement.MeasuringPoint;
          if (measuringPoint.Position == null) continue;
          all.Add(new MeasuringPointWithValue
          {
            XPosition = measuringPoint.Position.XPosition,
            YPosition = measuringPoint.Position.YPosition,
            Name = measuringPoint.Name,
            MaxValue = measurement.MaxValue
          });
        }
        return all;
      }
    }

    public DateTime SelectedDate
    {
      get { return _selectedDate; }
      set
      {
        _selectedDate = value; 
        NotifyOfPropertyChange(() => SelectedDate);
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
      }
    }

    public class MeasuringPointWithValue : PropertyChangedBase
    {
      private int _xPosition;
      private int _yPosition;
      private string _name;
      private float _maxValue;

      public int XPosition
      {
        get { return _xPosition; }
        set
        {
          _xPosition = value;
          NotifyOfPropertyChange(() => XPosition);
        }
      }

      public int YPosition
      {
        get { return _yPosition; }
        set
        {
          _yPosition = value;
          NotifyOfPropertyChange(() => YPosition);
        }
      }

      public string Name
      {
        get { return _name; }
        set
        {
          _name = value;
          NotifyOfPropertyChange(() => Name);
        }
      }

      public float MaxValue
      {
        get { return _maxValue; }
        set
        {
          _maxValue = value;
          NotifyOfPropertyChange(() => MaxValue);
        }
      }
     
    }
    public DateTime MaxDate
    {
        get { return maxDate; }
        set
        {
            maxDate = value;
            NotifyOfPropertyChange(() => MaxDate);
        }
    }
  }
}
