using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using OxyPlot;

namespace Krach.ViewModels
{
  [Export(typeof (IShell))]
  public class ShellViewModel : Screen, IShell
  {
    private readonly string description;
    private string firstSelected = "Blue";
    private string secondSelected = "Transparent";
    private string thirdSelected = "Transparent";

    private readonly List<DataPoint> points = new List<DataPoint>
    {
      new DataPoint(0, 4),
      new DataPoint(10, 13),
      new DataPoint(20, 15),
      new DataPoint(30, 16),
      new DataPoint(40, 12),
      new DataPoint(50, 12)
    };

    public void SelectNext()
    {
      firstSelected = "Transparent";
      secondSelected = "Blue";
      NotifyOfPropertyChange(() => FirstSelected);
      NotifyOfPropertyChange(() => SecondSelected);
    }


    public ShellViewModel()
    {
      description = "hier steht ein TExt";
      NotifyOfPropertyChange(() => Description);
    }

    public string Description
    {
      get { return description; }
    }

    public string SecondSelected
    {
      get { return secondSelected; }
    }

    public string FirstSelected
    {
      get { return firstSelected; }
    }

    public string ThirdSelectd
    {
      get { return thirdSelected; }
    }


    public List<DataPoint> Points
    {
      get { return points; }
    }

    public void Close()
    {
      TryClose();
    }
  }
}