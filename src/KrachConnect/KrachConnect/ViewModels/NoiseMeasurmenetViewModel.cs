using System;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
  internal class NoiseMeasurementViewModel : PropertyChangedBase
  {
    private NoiseMeasurement m_Model;

    public NoiseMeasurementViewModel(NoiseMeasurement nm)
    {
      m_Model = nm;
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

    public MeasuringMethod MeasuringMethod
    {
        get { return m_Model.Method; }
        set
        {
            m_Model.Method = value;
            NotifyOfPropertyChange(() => MeasuringMethod);
        }
    }

    public float MinValue
    {
      get { return m_Model.MinValue; }
      set
      {
        value = (float)Math.Round(value, 1);
        m_Model.MinValue = value;
        if (AverageValue < value)
        {
          AverageValue = value;
          MaxValue = value;
        }

        NotifyOfPropertyChange(() => MinValue);
      }
    }

    public float AverageValue
    {
      get { return m_Model.AverageValue; }
      set
      {
        value = (float) Math.Round(value, 1);
        m_Model.AverageValue = value;
        if (MinValue > value)
        {
          MinValue = value;
        }
        if (MaxValue < value)
        {
          MaxValue = value;
        }
        NotifyOfPropertyChange(() => AverageValue);
      }
    }


    public float MaxValue
    {
      get { return m_Model.MaxValue; }
      set
      {
        value = (float)Math.Round(value, 1);
        m_Model.MaxValue = value;
        if (MinValue > value)
        {
          MinValue = value;
        }
        if (AverageValue > value)
        {
          AverageValue = value;
        }
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