using System;
using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
  public class ShellViewModel : Conductor<Object>
  {
    private readonly NoiseRepository nr = new NoiseRepository();


    public ShellViewModel()
    {
      ActivateItem(new HomepageViewModel(nr));
    }

    public void ShowMapScreen()
    {
      ActivateItem(new MapAddViewModel(nr, this));
    }


    public void ShowHomePageScreen()
    {
      ActivateItem(new HomepageViewModel(nr));
    }


    public void ShowMeasuringPointsEditScreen()
    {
      ActivateItem(new MeasuringPointsEditViewModel(nr));
    }

    public void ShowMeasuringPlaningScreen()
    {
      ActivateItem(new MeasuringPlaningViewModel(nr, this));
    }

    public void ShowEvaluationScreen()
    {
      ActivateItem(new EvaluationViewModel(nr));
    }

  }
}