using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
  internal class ConfirmationBoxViewModel : Screen
  {
    private MeasuringPointViewModel measuringPoint;
    private NoiseRepository repository;
    private bool thisIsANewMeasuringPoint;

    public ConfirmationBoxViewModel(MeasuringPointViewModel measuringPoint, NoiseRepository repository)
    {
      this.MeasuringPoint = measuringPoint;
      this.repository = repository;
    }

    public ConfirmationBoxViewModel(MeasuringPointViewModel measuringPoint, bool thisIsANewMeasuringPoint)
    {
      this.MeasuringPoint = measuringPoint;
      this.repository = repository;
      this.thisIsANewMeasuringPoint = thisIsANewMeasuringPoint;
    }

    public MeasuringPointViewModel MeasuringPoint
    {
      get { return measuringPoint; }
      set
      {
        measuringPoint = value;
        NotifyOfPropertyChange(() => MeasuringPoint);
      }
    }

    public Visibility DeleteButtonVisibility
    {
      get
      {
        if (thisIsANewMeasuringPoint)
        {
          return Visibility.Collapsed;
        }
        var thereAreNoiseMeasuremetnsWithThisMeasuringPoint =
          repository.NoiseMeasurements.Any(nm => nm.MeasuringPoint == MeasuringPoint.Model);
        return thereAreNoiseMeasuremetnsWithThisMeasuringPoint ? Visibility.Collapsed : Visibility.Visible;
      }
    }

    public void DeleteMeasuringPoint()
    {
      repository.MeasuringPoints.Remove(MeasuringPoint.Model);
      MeasuringPoint.Deleted = true;
      TryClose(true);
    }

    public void OK()
    {
      TryClose(true);
    }

    public void Cancel()
    {
      TryClose(false);
    } 
  }
}