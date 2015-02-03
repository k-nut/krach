using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Navigation;
using Caliburn.Micro;
using KrachConnect.DomainModelService;
using MahApps.Metro.Controls;
using OfficeOpenXml;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using Screen = Caliburn.Micro.Screen;

namespace KrachConnect.ViewModels
{
  internal class AlternativeEvaluationViewModel : Screen
  {
    private string _searchTerm = "";
    private string _selectedValueType;
    private DateTime maxDate = DateTime.Today.AddDays(1);
    private ObservableCollection<MeasuringPointViewModel> measuringPoints;
    private DateTime minDate = DateTime.Today.AddYears(-1);
    private ObservableCollection<NoiseMeasurementViewModel> noiseMeasurements;
    private PlotModel plotModel;
    private MeasuringPointViewModel selectedMeasuringPoint;
    private PlotModel totalsPlotModel;
    private PlotModel _historgrammPlotModel;
    private MeasuringPoint _selectedMeasuringPoint;
    public override string DisplayName { get { return "Evaluation"; } }

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


      MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels);
      NoiseMeasurements =
        new ObservableCollection<NoiseMeasurementViewModel>(
          repository.NoiseMeasurements.Select(nm => new NoiseMeasurementViewModel(nm)));

      DrawTotalsChart();
      PopulatePlotModel();

      foreach (MeasuringPointViewModel mp in MeasuringPoints)
      {
        mp.PropertyChanged += IsSelectedChanged;
      }

      SelectedValueType = DisplayValueTypes.First();
    }


    public IEnumerable<NoiseMeasurementViewModel> FilteredNoiseMeasurementViewModels
    {
      get
      {
        return NoiseMeasurements.Where(nm => nm.MeasurementDate >= MinDate && nm.MeasurementDate <= MaxDate)
          .Where(nm => SelectedMeasuringPoints.Contains(nm.MeasuringPoint));
      }
    }

    public List<MeasuringPoint> SelectedMeasuringPoints
    {
      get { return MeasuringPoints.Where(mp => mp.IsSelected).Select(mp => mp.Model).ToList(); }
    }


    public string SearchTerm
    {
      get { return _searchTerm; }
      set
      {
        _searchTerm = value;
        NotifyOfPropertyChange(() => SearchTerm);
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
      }
    }

    public IEnumerable<MeasuringPointViewModel> FilteredMeasuringPoints
    {
      get { return MeasuringPoints.Where(mp => mp.Name.ToLower().Contains(SearchTerm.ToLower())); }
    }

    public IEnumerable<String> DisplayValueTypes
    {
      get
      {
        return new List<String> { "Minimalwert", "Mittelwert", "Maximalwert" };
      }
    }

    public PlotModel HistorgrammPlotModel
    {
      get { return _historgrammPlotModel; }
      set
      {
        _historgrammPlotModel = value;
        NotifyOfPropertyChange(() => HistorgrammPlotModel);
      }
    }

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

    public string SelectedValueType
    {
      get { return _selectedValueType; }
      set
      {
        _selectedValueType = value;
        NotifyOfPropertyChange(() => SelectedValueType);
        UpdatePlots();
      }
    }

    public MeasuringPoint SelectedMeasuringPoint
    {
      get { return _selectedMeasuringPoint; }
      set
      {
        _selectedMeasuringPoint = value; 
        NotifyOfPropertyChange(() => SelectedMeasuringPoint);
        NotifyOfPropertyChange(() => NextEnabled);
        NotifyOfPropertyChange(() => PrevEnabled);
        DrawHistogramm();
      }
    }

    public void Filter(object text)
    {
      SearchTerm = text.ToString();
    }

    public void SelectAll()
    {
      foreach (var measuringPointViewModel in FilteredMeasuringPoints)
      {
        measuringPointViewModel.IsSelected = true;
      }
    }

    public void DeSelectAll()
    {
      foreach (var measuringPointViewModel in FilteredMeasuringPoints)
      {
        measuringPointViewModel.IsSelected = false;
      }
    }

    private void UpdatePlots()
    {
      DrawTotalsChart();
      PopulatePlotModel();
      DrawHistogramm();
    }

    private void IsSelectedChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsSelected")
      {
        NotifyOfPropertyChange(() => SelectedMeasuringPoints);
        NotifyOfPropertyChange(() => FilteredNoiseMeasurementViewModels);
        if (SelectedMeasuringPoint == null)
        {
          SelectedMeasuringPoint = SelectedMeasuringPoints.First();
        }
        NotifyOfPropertyChange(() => NextEnabled);
        NotifyOfPropertyChange(() => PrevEnabled);
        UpdatePlots();
      }
    }

    public void Export()
    {
      var saveFileDialog1 = new SaveFileDialog
      {
        FilterIndex = 2,
        FileName = "Export.xlsx",
        DefaultExt = "xlsx",
        RestoreDirectory = true
      };

      if (saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        var myStream = saveFileDialog1.OpenFile();
        using (var package = new ExcelPackage(myStream))
        {
          var selectedNoiseMeasuremenets =
            FilteredNoiseMeasurementViewModels.Where(nm => nm.MeasuringPoint != null).ToList();

          ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Messwerte");
          worksheet.Cells[1, 1].Value = "Messpunkt";
          worksheet.Cells[1, 2].Value = "Datum";
          worksheet.Cells[1, 3].Value = "Minimalwert";
          worksheet.Cells[1, 4].Value = "Durchschnittswert";
          worksheet.Cells[1, 5].Value = "Maximalwert";
          worksheet.Cells[1, 6].Value = "Mitarbeiter";
          worksheet.Cells[1, 7].Value = "Bemerkung";
          worksheet.Cells[1, 8].Value = "Messverfahren";
          worksheet.Cells[1, 9].Value = "Messpunkt ist archiviert";
          worksheet.Cells[worksheet.Dimension.Address].AutoFilter = true;


          for (int i = 0; i < selectedNoiseMeasuremenets.Count(); i++)
          {
            NoiseMeasurementViewModel nm = selectedNoiseMeasuremenets[i];
            worksheet.Cells[i + 2, 1].Value = nm.MeasuringPoint.Name;
            worksheet.Cells[i + 2, 2].Value = nm.MeasurementDate;
            worksheet.Cells[i + 2, 2].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.FullDateTimePattern;
            worksheet.Cells[i + 2, 3].Value = nm.MinValue;
            worksheet.Cells[i + 2, 4].Value = nm.AverageValue;
            worksheet.Cells[i + 2, 5].Value = nm.MaxValue;
            worksheet.Cells[i + 2, 6].Value = nm.Employee;
            worksheet.Cells[i + 2, 7].Value = nm.MeasuringPoint.Notes;
            worksheet.Cells[i + 2, 8].Value = nm.MeasuringMethod != null ? nm.MeasuringMethod.Name : "nicht angegeben";
            worksheet.Cells[i + 2, 9].Value = nm.MeasuringPoint.IsArchived ? "Ja" : "Nein";
          }
          worksheet.Cells.AutoFitColumns(0);

          package.Save();
        }
        myStream.Close();
      }
    }


    private void DrawTotalsChart()
    {
      TotalsPlotModel.Series.Clear();
      TotalsPlotModel.Axes.Clear();
      TotalsPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, MajorTickSize = 1, MinorTickSize = 0 });

      IEnumerable<NameCountCouple> all = (from mp in MeasuringPoints
                                          let count =
                                            FilteredNoiseMeasurementViewModels.Where(nm => nm.MeasuringPoint == mp.Model).Count(nm => nm.MaxValue > 80)
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

    public void SelectNextMeasuringPoint()
    {
      var position = SelectedMeasuringPoints.IndexOf(SelectedMeasuringPoint);
      SelectedMeasuringPoint = SelectedMeasuringPoints[++position];
    }

    public void SelectPreviousMeasuringPoint()
    {
      var position = SelectedMeasuringPoints.IndexOf(SelectedMeasuringPoint);
      SelectedMeasuringPoint = SelectedMeasuringPoints[--position];
    }

    public bool NextEnabled
    {
      get { return SelectedMeasuringPoints.IndexOf(SelectedMeasuringPoint) < SelectedMeasuringPoints.Count() - 1; }
    }

    public bool PrevEnabled
    {
      get { return SelectedMeasuringPoints.IndexOf(SelectedMeasuringPoint) != 0; }
    }


    private void PopulatePlotModel()
    {
      PlotModel.Series.Clear();

      IEnumerable<IGrouping<MeasuringPoint, NoiseMeasurementViewModel>> noiseMeausurements =
        FilteredNoiseMeasurementViewModels.Where(nm => nm.MeasuringPoint != null).GroupBy(nm => nm.MeasuringPoint);

      var allowedColors = new List<OxyColor>
      {
        OxyColors.Orchid,
        OxyColors.Blue,
        OxyColors.Purple,
        OxyColors.MediumAquamarine,
        OxyColors.MediumBlue,
        OxyColors.Orchid,
        OxyColors.PowderBlue,
        OxyColors.RosyBrown,
        OxyColors.Lavender,
        OxyColors.Olive,
        OxyColors.OldLace,
        OxyColors.PeachPuff,
        OxyColors.Pink,
        OxyColors.Peru
      };
      int count = 0;
      foreach (var group in noiseMeausurements)
      {
        var lineSeries = new LineSeries
        {
          Title = group.Key.Name,
          Color = allowedColors[count],
          MarkerType = MarkerType.Circle,
          MarkerSize = 5,
          MarkerFill = allowedColors[count]
        };
        count = (count + 1) % allowedColors.Count();

        Func<NoiseMeasurementViewModel, float> func;

        switch (SelectedValueType)
        {
          case "Minimalwert":
            func = (m) => m.MinValue;
            break;
          case "Mittelwert":
            func = (m) => m.AverageValue;
            break;
          default:
            func = (m) => m.MaxValue;
            break;
        }

        foreach (NoiseMeasurementViewModel measurement in group.OrderBy(nm => nm.MeasurementDate))
        {
          lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(measurement.MeasurementDate, func(measurement)));
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

    private void DrawHistogramm()
    {
      if (SelectedMeasuringPoint == null) return;
      var measurements = NoiseMeasurements.Where(nm => nm.MeasuringPoint == SelectedMeasuringPoint).ToList();

      Func<NoiseMeasurementViewModel, float> func;

      switch (SelectedValueType)
      {
        case "Minimalwert":
          func = (m) => m.MinValue;
          break;
        case "Mittelwert":
          func = (m) => m.AverageValue;
          break;
        default:
          func = (m) => m.MaxValue;
          break;
      }

      var total = new List<NameCountCouple>();
      for (var i = 0; i <= 120; i+=20)
      {
        total.Add(new NameCountCouple
        {
          Count = measurements.Count(nm => func(nm) >= i && func(nm) < i + 20),
          Name = i + "-" + (i + 20) + " db"
        });
      }

      HistorgrammPlotModel = new PlotModel();
      HistorgrammPlotModel.Title = SelectedMeasuringPoint.Name;
      HistorgrammPlotModel.Series.Add(new ColumnSeries{ItemsSource = total, ValueField = "Count", ColumnWidth = 100});
      HistorgrammPlotModel.Axes.Add(new CategoryAxis{Position = AxisPosition.Bottom, ItemsSource = total, LabelField = "Name"});
      HistorgrammPlotModel.InvalidatePlot(true);
    }

    private class NameCountCouple
    {
      public string Name { get; set; }
      public int Count { get; set; }
    }
  }
}