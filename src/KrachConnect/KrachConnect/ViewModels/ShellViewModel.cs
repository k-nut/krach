using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
  public class ShellViewModel : Conductor<Object>
  {
    private NoiseRepository nr = new NoiseRepository();

    public ShellViewModel()
    {
      ActivateItem(new HomepageViewModel());
    }

    public void ShowMapScreen()
    {
      ActivateItem(new MapAddViewModel(nr));
    }


    public void ShowHomePageScreen()
    {
      ActivateItem(new HomepageViewModel());
    }


    public void ShowMeasuringPointsEditScreen()
    {
      ActivateItem(new MeasuringPointsEditViewModel(nr));
    }

    public void ShowMapSliderScreen()
    {
      ActivateItem(new MapAddSliderViewModel(nr));
    }

    public void ShowGreenScreen()
    {
      //ActivateItem(new GreenViewModel());
    }

    public void ShowBlueScreen()
    {
      //ActivateItem(new BlueViewModel());
    }
  }
}
