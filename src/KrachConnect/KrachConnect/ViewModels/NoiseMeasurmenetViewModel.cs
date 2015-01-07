using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
  class NoiseMeasurementViewModel : PropertyChangedBase
  {
    private NoiseMeasurement m_Model;

    public NoiseMeasurementViewModel(NoiseMeasurement nm)
    {
      this.m_Model = nm;
    }

    public DateTime MeasurementDate
    {
      get { return m_Model.MeasurementDate; }
      set
      {
        m_Model.MeasurementDate = value;
        NotifyOfPropertyChange(() => MeasurementDate);
      }
    }

    public float MinValue
    {
      get { return m_Model.MinValue; }
      set
      {
        m_Model.MinValue = value;
        NotifyOfPropertyChange(() => MinValue);
      }
    }

    public float AverageValue
    {
      get { return m_Model.AverageValue; }
      set
      {
        m_Model.AverageValue = value;
        NotifyOfPropertyChange(() => AverageValue);
      }
    }

    public float MaxValue
    {
      get { return m_Model.MaxValue; }
      set
      {
        m_Model.MaxValue = value;
        NotifyOfPropertyChange(() => MaxValue);
      }
    }

    public String Employee
    {
      get { return m_Model.Employee; }
      set
      {
        m_Model.Employee = value;
        NotifyOfPropertyChange(() => Employee);
      }
    }

    public MeasuringPoint MeasuringPoint
    {
      get { return m_Model.MeasuringPoint; }
      set
      {
        m_Model.MeasuringPoint = value;
        NotifyOfPropertyChange(() => MeasuringPoint);
      }
    }

    public NoiseMeasurement Model
    {
      get { return m_Model; }
      set
      {
        m_Model = value;
        NotifyOfPropertyChange(() => Model);
      }
    }

  }
}
