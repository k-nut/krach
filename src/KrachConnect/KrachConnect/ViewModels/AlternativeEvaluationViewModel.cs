using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Caliburn.Micro;
using KrachConnect.DomainModelService;
using OfficeOpenXml;
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
    private PlotModel totalsPlotModel;
    private MeasuringPointViewModel selectedMeasuringPoint;
    private ObservableCollection<MeasuringPointViewModel> measuringPoints;
    private DateTime minDate = DateTime.Today.AddYears(-1);
    private DateTime maxDate = DateTime.Today.AddDays(1);



    public IEnumerable<NoiseMeasurementViewModel> FilteredNoiseMeasurementViewModels
    {
      get { return NoiseMeasurements.Where(nm => nm.MeasurementDate >= MinDate && nm.MeasurementDate <= MaxDate)
        .Where(nm => SelectedMeasuringPoints.Contains(nm.MeasuringPoint)); }
    }

    public IEnumerable<MeasuringPoint> SelectedMeasuringPoints
    {
      get { return MeasuringPoints.Where(mp => mp.IsSelected).Select(mp => mp.Model); }
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

      foreach (var mp in MeasuringPoints)
      {
        mp.PropertyChanged += IsSelectedChanged;
      }
    }

    private void IsSelectedChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsSelected")
      {
        NotifyOfPropertyChange(() => SelectedMeasuringPoints);
        NotifyOfPropertyChange(() => FilteredNoiseMeasurementViewModels);
        UpdatePlots();
      }
    }

    public void Export()
    {

      Stream myStream;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();

      saveFileDialog1.FilterIndex = 2;
      saveFileDialog1.FileName = "Export.xlsx";
      saveFileDialog1.DefaultExt = "xlsx";
      saveFileDialog1.RestoreDirectory = true;

      if (saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        myStream = saveFileDialog1.OpenFile();
        using (var package = new ExcelPackage(myStream))
        {
          var selectedNoiseMeasuremenets = FilteredNoiseMeasurementViewModels.Where(nm => nm.MeasuringPoint != null).ToList();// TODO: If the data is valid this cannot happen
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


          for (var i = 0; i < selectedNoiseMeasuremenets.Count(); i++)
          {
            var nm = selectedNoiseMeasuremenets[i];
            worksheet.Cells[i + 2, 1].Value = nm.MeasuringPoint.Name;
            worksheet.Cells[i + 2, 2].Value = nm.MeasurementDate;
            worksheet.Cells[i + 2, 2].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.FullDateTimePattern;
            worksheet.Cells[i + 2, 3].Value = nm.MinValue;
            worksheet.Cells[i + 2, 4].Value = nm.AverageValue;
            worksheet.Cells[i + 2, 5].Value = nm.MaxValue;
            worksheet.Cells[i + 2, 6].Value = nm.Employee;
            worksheet.Cells[i + 2, 7].Value = nm.MeasuringPoint.Notes;
            worksheet.Cells[i + 2, 8].Value = nm.MeasuringMethod != null ? nm.MeasuringMethod.Name :"nicht angegeben";
            worksheet.Cells[i + 2, 9].Value = nm.MeasuringPoint.IsArchived? "Ja": "Nein";
          }
          worksheet.Cells.AutoFitColumns(0);

          package.Save();
        }
        myStream.Close();
      }
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
      var count = 0;
      foreach (var group in noiseMeausurements)
      {
        var lineSeries = new LineSeries { Title = group.Key.Name, Color = allowedColors[count]};
        count = allowedColors.Count() == count ? 0 : count + 1;
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