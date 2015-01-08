using System.Windows.Media;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect
{
  public class MeasuringPointViewModel :PropertyChangedBase
  {
    private MeasuringPoint m_Model;
    private bool isSelected = false;
    private bool justMeasured = false;
    private bool isArchived = false;
    
    public MeasuringPointViewModel(MeasuringPoint mp)
    {
      m_Model = mp;
    }

    public MeasuringPoint Model { get { return m_Model; } } 

    public bool IsSelected 
    {get { return isSelected; }
      set
      {
        isSelected = value;
        NotifyOfPropertyChange(() => Model);
        NotifyOfPropertyChange(() => IsSelected);
      }
    }

    public bool IsArchived
    {
      get { return m_Model.IsArchived; }
      set
      {
        m_Model.IsArchived = value;
        NotifyOfPropertyChange(() => IsArchived);
      }
    }
    public string Name
    {
        get { return m_Model.Name; }
        set
        {
            m_Model.Name = value;
            NotifyOfPropertyChange(() => Name);
        }
    }   


    public bool JustMeasured
    {
      get { return justMeasured; }
      set
      {
        justMeasured = value;
        NotifyOfPropertyChange(() => JustMeasured);
      }
    }
  }
}