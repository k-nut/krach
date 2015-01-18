using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace KrachConnect.ViewModels
{
  internal class AlternativeEvaluationViewModel : PropertyChangedBase
  {
    private ObservableCollection<NoiseMeasurementViewModel> noiseMeasurements;
    private NoiseRepository repository;
    public LineSeries MinValues { get; private set; }
    public LineSeries AverageValues { get; private set; }
    public LineSeries MaxValues { get; private set; }
    private PlotModel plotModel;
    private MeasuringPointViewModel selectedMeasuringPoint;
    private ObservableCollection<MeasuringPointViewModel> measuringPoints;
    private ObservableCollection<NoiseMeasurementViewModel> tooLoudMeasurments;
    private PlotModel totalsPlotModel;

    public PlotModel PlotModel
    {
      get
      {
        return plotModel;
      }
      set
      {
        plotModel = value;
        NotifyOfPropertyChange(() => PlotModel);
      }
    }

    public ObservableCollection<NoiseMeasurementViewModel> NoiseMeasurements
    {
      get { return noiseMeasurements; }
      set
      {
        noiseMeasurements = value;
        NotifyOfPropertyChange(() => NoiseMeasurements);
      }
    }

    public ObservableCollection<MeasuringPointViewModel> MeasuringPoints
    {
      get { return measuringPoints; }
      set
      {
        measuringPoints = value;
        NotifyOfPropertyChange(() => MeasuringPoints);
      }
    }


    public MeasuringPointViewModel SelectedMeasuringPoint
    {
      get { return selectedMeasuringPoint; }
      set
      {
        selectedMeasuringPoint = value;
     //   PopulatePlotModel();
        NotifyOfPropertyChange(() => SelectedMeasuringPoint);
      }
    }

    public ObservableCollection<NoiseMeasurementViewModel> TooLoudMeasurements
    {
      get { return tooLoudMeasurments; }
      set
      {
        tooLoudMeasurments = value;
        NotifyOfPropertyChange(() => TooLoudMeasurements);
      }
    }

    public PlotModel TotalsPlotModel
    {
      get { return totalsPlotModel; }
      set
      {
        totalsPlotModel = value;
        NotifyOfPropertyChange(() => TotalsPlotModel);
      }
    }

    public AlternativeEvaluationViewModel(NoiseRepository repository)
    {
   //   PlotModel = new PlotModel();
    //  PlotModel.Axes.Add(new DateTimeAxis(AxisPosition.Bottom));

      this.repository = repository;


      MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels);
      NoiseMeasurements = new ObservableCollection<NoiseMeasurementViewModel>(repository.NoiseMeasurements.Select(nm => new NoiseMeasurementViewModel(nm)));
      SelectedMeasuringPoint = MeasuringPoints.First();
      var lastMeasuringDate = NoiseMeasurements.Max(nm => nm.MeasurementDate);
      var tooLoudMeasurements = NoiseMeasurements.Where(nm => nm.MeasurementDate == lastMeasuringDate)
        .Where(nm => nm.MaxValue > 60);
      TooLoudMeasurements = new ObservableCollection<NoiseMeasurementViewModel>(tooLoudMeasurements);

      DrawTotalsChart();


      
    }

    private class NameCountCouple
      {
      public string Name { get; set; }
      public int Count { get; set; }
    }

    private void DrawTotalsChart()
    {
      TotalsPlotModel = new PlotModel();
      var all = (from mp in MeasuringPoints
                 let count = NoiseMeasurements.Where(nm => nm.MeasuringPoint == mp.Model).Count(nm => nm.MaxValue > 60)
                 select new NameCountCouple { Name = mp.Name, Count = count }
                 ).ToList();
      TotalsPlotModel.Axes.Add(new CategoryAxis { ItemsSource = all, LabelField = "Name", Angle = 45 });
      TotalsPlotModel.Series.Add(new ColumnSeries { ItemsSource = all, ValueField = "Count" });
      TotalsPlotModel.Title = "Grenzwerueberschreitungen pro Messpunkt (gesamt)";
      
    }

    private void PopulatePlotModel()
    {
      PlotModel.Series.Clear();
      PlotModel.Title = SelectedMeasuringPoint.Name;

      var noiseMeausurements = NoiseMeasurements.Where(nm => nm.MeasuringPoint == SelectedMeasuringPoint.Model);

      this.MinValues = new LineSeries { Color = OxyColors.Green };
      this.AverageValues = new LineSeries { Color = OxyColors.Orange };
      this.MaxValues = new LineSeries { Color = OxyColors.Red };
      foreach (var mearuement in noiseMeausurements.ToList().OrderBy(nm => nm.MeasurementDate))
      {
        this.MinValues.Points.Add(DateTimeAxis.CreateDataPoint(mearuement.MeasurementDate, mearuement.MinValue));
        this.AverageValues.Points.Add(DateTimeAxis.CreateDataPoint(mearuement.MeasurementDate, mearuement.AverageValue));
        this.MaxValues.Points.Add(DateTimeAxis.CreateDataPoint(mearuement.MeasurementDate, mearuement.MaxValue));
      }
      PlotModel.Series.Add(this.MinValues);
      PlotModel.Series.Add(this.AverageValues);
      PlotModel.Series.Add(this.MaxValues);
      PlotModel.InvalidatePlot(true);

    }
  }
}