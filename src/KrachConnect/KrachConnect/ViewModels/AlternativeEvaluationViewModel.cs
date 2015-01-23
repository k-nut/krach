using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Annotations;
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
    private DateTime minDate = DateTime.Today.AddYears(-1);
    private DateTime maxDate = DateTime.Today;



    public IEnumerable<NoiseMeasurementViewModel> FilteredNoiseMeasurementViewModels
    {
      get { return NoiseMeasurements.Where(nm => nm.MeasurementDate >= MinDate && nm.MeasurementDate <= MaxDate); }
    }

    private PlotModel totalsPlotModel;

    public PlotModel PlotModel
    {
      get { return plotModel; }
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

    public PlotModel TotalsPlotModel
    {
      get { return totalsPlotModel; }
      set
      {
        totalsPlotModel = value;
        NotifyOfPropertyChange(() => TotalsPlotModel);
      }
    }

    public DateTime MinDate
    {
      get { return minDate; }
      set
      {
        minDate = value;
        NotifyOfPropertyChange(() => MinDate);
        NotifyOfPropertyChange(() => FilteredNoiseMeasurementViewModels);
        UpdatePlots();
      }
    }

    public DateTime MaxDate
    {
      get { return maxDate; }
      set
      {
        maxDate = value;
        NotifyOfPropertyChange(() => MaxDate);
        NotifyOfPropertyChange(() => FilteredNoiseMeasurementViewModels);
        UpdatePlots();
      }
    }

    private void UpdatePlots()
    {
      DrawTotalsChart();
      PopulatePlotModel();
    }

    public AlternativeEvaluationViewModel(NoiseRepository repository)
    {
      PlotModel = new PlotModel { Title = "Alle Messpunkte" };
      var dateAxis = new DateTimeAxis
      {
        Position = AxisPosition.Bottom,
        StringFormat = "d.M.yyyy"
      };
      PlotModel.Axes.Add(dateAxis);


      TotalsPlotModel = new PlotModel { Title = "Grenzwertüberschreitungen pro Messpunkt (gesamt)" };


      this.repository = repository;


      MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels);
      NoiseMeasurements =
          new ObservableCollection<NoiseMeasurementViewModel>(
              repository.NoiseMeasurements.Select(nm => new NoiseMeasurementViewModel(nm)));

      DrawTotalsChart();
      PopulatePlotModel();



    }

    private class NameCountCouple
    {
      public string Name { get; set; }
      public int Count { get; set; }
    }

    private void DrawTotalsChart()
    {
      TotalsPlotModel.Series.Clear();
      TotalsPlotModel.Axes.Clear();

      var all = (from mp in MeasuringPoints
                 let count = FilteredNoiseMeasurementViewModels.Where(nm => nm.MeasuringPoint == mp.Model).Count(nm => nm.MaxValue > 60)
                 select new NameCountCouple { Name = mp.Name, Count = count }
          );
      all = all.Where(ncc => ncc.Count > 0).OrderBy(ncc => ncc.Count).ToList();
      TotalsPlotModel.Axes.Add(new CategoryAxis
      {
        ItemsSource = all,
        LabelField = "Name",
        Position = AxisPosition.Left
      });
      TotalsPlotModel.Series.Add(new BarSeries
      {
        ItemsSource = all,
        ValueField = "Count",
        FillColor = OxyColors.Red
      });

      TotalsPlotModel.InvalidatePlot(true);

    }


    private void PopulatePlotModel()
    {
      PlotModel.Series.Clear();

      var noiseMeausurements =
          FilteredNoiseMeasurementViewModels.Where(nm => nm.MeasuringPoint != null).GroupBy(nm => nm.MeasuringPoint);


      foreach (var group in noiseMeausurements)
      {
        var lineSeries = new LineSeries { Title = group.Key.Name };
        foreach (var measurement in group.OrderBy(nm => nm.MeasurementDate))
        {
          lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(measurement.MeasurementDate, measurement.MaxValue));
        }
        PlotModel.Series.Add(lineSeries);
      }
      PlotModel.Annotations.Add(new LineAnnotation
      {
        Y = 80,
        Type = LineAnnotationType.Horizontal,
        Color = OxyColors.DarkRed
      });

      PlotModel.IsLegendVisible = true;
      PlotModel.LegendPosition = LegendPosition.TopRight;
      PlotModel.LegendBackground = OxyColors.White;


      PlotModel.InvalidatePlot(true);

    }
  }
}