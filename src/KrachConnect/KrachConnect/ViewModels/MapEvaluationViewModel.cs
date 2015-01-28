using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using KrachConnect.DomainModelService;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace KrachConnect.ViewModels
{
  class MapEvaluationViewModel : Screen
  {
    private IEnumerable<MeasuringPointViewModel> measuringPoints;
    private IEnumerable<NoiseMeasurement> noiseMeasurements;
    private DateTime maxDate = DateTime.Today;


    public MapEvaluationViewModel(NoiseRepository repository)
    {
      MeasuringPoints = repository.MeasuringPointViewModels;
      noiseMeasurements = repository.NoiseMeasurements;
      MeasurementDates = new HashSet<DateTime>(noiseMeasurements.Select(nm => nm.MeasurementDate));
      MaxDate = MeasurementDates.Max();
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
        var all = new List<MeasuringPointWithValue>();
        foreach (var measuringPoint in MeasuringPoints)
        {
          NoiseMeasurement lastNoiseMeasurementForPoint;
          try
          {
            lastNoiseMeasurementForPoint = noiseMeasurements.Where(nm => nm.MeasuringPoint == measuringPoint.Model)
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

    public class MeasuringPointWithValue : PropertyChangedBase
    {
      private int _xPosition;
      private int _yPosition;
      private string _name;
      private float _maxValue;
      private DateTime _date;

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
    public DateTime MaxDate
    {
      get { return maxDate; }
      set
      {
        maxDate = value;
        NotifyOfPropertyChange(() => MaxDate);
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
      }
    }
  }
}
