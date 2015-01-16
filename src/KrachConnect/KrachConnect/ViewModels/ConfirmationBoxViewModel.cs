using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
  internal class ConfirmationBoxViewModel : Screen
  {
    private MeasuringPointViewModel measuringPoint;

    public ConfirmationBoxViewModel(MeasuringPointViewModel measuringPoint)
    {
      this.MeasuringPoint = measuringPoint;
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