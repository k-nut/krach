using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
  internal class MapEvaluationViewModel : HasMapScreen
  {
    private readonly IEnumerable<NoiseMeasurement> _noiseMeasurements;
    private DateTime _maxDate = DateTime.Today;
    private IEnumerable<MeasuringPointViewModel> _measuringPoints;


    public MapEvaluationViewModel(NoiseRepository repository) :base(repository)
    {
      MeasuringPoints = repository.MeasuringPointViewModels;
      _noiseMeasurements = repository.NoiseMeasurements;
      MeasurementDates = new HashSet<DateTime>(_noiseMeasurements.Select(nm => nm.MeasurementDate));
      MaxDate = MeasurementDates.Max();
      PropertyChanged += NotifyActiveMapChanged;
     }

    private void NotifyActiveMapChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ActiveMap")
      {
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
      }
    }

    public override string DisplayName
    {
      get { return "MapEvaluation"; }
    }

    public IEnumerable<MeasuringPointViewModel> MeasuringPoints
    {
      get { return _measuringPoints; }
      set
      {
        _measuringPoints = value;
        NotifyOfPropertyChange(() => MeasuringPoints);
      }
    }

    public IEnumerable<DateTime> MeasurementDates { get; set; }

    public IEnumerable<MeasuringPointWithValue> FilteredMeasuringPoints
    {
      get
      {
        var all = new List<MeasuringPointWithValue>();
        foreach (MeasuringPointViewModel measuringPoint in MeasuringPoints)
        {
          NoiseMeasurement lastNoiseMeasurementForPoint;
          try
          {
            lastNoiseMeasurementForPoint = _noiseMeasurements.Where(nm => nm.MeasuringPoint == measuringPoint.Model && nm.MeasuringPoint.Position.NoiseMap == ActiveMap)
              .Where(nm => nm.MeasurementDate <= MaxDate)
              .OrderByDescending(nm => nm.MeasurementDate)
              .First();
          }
          catch (Exception e)
          {
            continue;
          }
          all.Add(new MeasuringPointWithValue
          {
            XPosition = measuringPoint.Position.XPosition,
            YPosition = measuringPoint.Position.YPosition,
            Name = measuringPoint.Name,
            MaxValue = lastNoiseMeasurementForPoint.MaxValue,
            Date = lastNoiseMeasurementForPoint.MeasurementDate
          });
        }
        return all;
      }
    }

    public DateTime MaxDate
    {
      get { return _maxDate; }
      set
      {
        _maxDate = value;
        NotifyOfPropertyChange(() => MaxDate);
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
      }
    }

    public class MeasuringPointWithValue : PropertyChangedBase
    {
      private DateTime _date;
      private float _maxValue;
      private string _name;
      private int _xPosition;
      private int _yPosition;

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

      public DateTime Date
      {
        get { return _date; }
        set
        {
          _date = value;
          NotifyOfPropertyChange(() => Date);
        }
      }
    }
  }
}