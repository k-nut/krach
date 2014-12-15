using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect
{
  public class MeasuringPointViewModel :PropertyChangedBase
  {
    private MeasuringPoint model;
    private bool isSelected = false;
    public MeasuringPointViewModel(MeasuringPoint mp)
    {
      model = mp;
    }

    public MeasuringPoint Model { get { return model; } } 
    public bool IsSelected 
    {get { return isSelected; }
      set
      {
        isSelected = value;
        NotifyOfPropertyChange(() => Model);
      }
    }

    public int Stroke { get { return isSelected ? 3 : 0; }}
  }
}