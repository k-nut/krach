using System.Windows.Media;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect
{
  public class MeasuringPointViewModel :PropertyChangedBase
  {
    private MeasuringPoint model;
    private bool isSelected = false;
    private bool justMeasured = false;
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
        NotifyOfPropertyChange(() => IsSelected);
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