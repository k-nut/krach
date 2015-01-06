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
  }
}
