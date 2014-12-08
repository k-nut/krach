using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Documents;
using Caliburn.Micro;
using Krach.Views;
using OxyPlot;
using OxyPlot.Series;

namespace Krach.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Screen, IShell
    {
      private string description;

      private List<DataPoint> points = new List<DataPoint>
                              {
                                  new DataPoint(0, 4),
                                  new DataPoint(10, 13),
                                  new DataPoint(20, 15),
                                  new DataPoint(30, 16),
                                  new DataPoint(40, 12),
                                  new DataPoint(50, 12)
                              }; 


        public void Close()
        {
            this.TryClose();
        }

      public ShellViewModel()
      {
        description = "hier steht ein TExt";
        NotifyOfPropertyChange(() => Description);
      }

      public string Description {
        get { return description; }
      }

      public List<DataPoint> Points { get { return points; }}


    }
}