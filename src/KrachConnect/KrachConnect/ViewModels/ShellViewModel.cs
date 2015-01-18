using System;
using System.Collections.ObjectModel;
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

    public void ShowMapScreen(ObservableCollection<MeasuringPointViewModel> selectedMeasuringPoints )
    {
      ActivateItem(new MapAddViewModel(nr, this, selectedMeasuringPoints));
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