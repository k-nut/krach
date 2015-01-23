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
        private ObservableCollection<NoiseMeasurementViewModel> tooLoudMeasurments;
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
            PlotModel = new PlotModel();
            PlotModel.Axes.Add(new DateTimeAxis(AxisPosition.Bottom));
            PlotModel.Title = "Alle Messpunkte";

            this.repository = repository;


            MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels);
            NoiseMeasurements =
                new ObservableCollection<NoiseMeasurementViewModel>(
                    repository.NoiseMeasurements.Select(nm => new NoiseMeasurementViewModel(nm)));
            SelectedMeasuringPoint = MeasuringPoints.First();
            LoadTooLoudMeasuringPointsFromLastReading();

            DrawTotalsChart();
            PopulatePlotModel();



        }

        private void LoadTooLoudMeasuringPointsFromLastReading()
        {
            var lastMeasuringDate = NoiseMeasurements.Max(nm => nm.MeasurementDate);
            var tooLoudMeasurements = NoiseMeasurements.Where(nm => nm.MeasurementDate == lastMeasuringDate)
                .Where(nm => nm.MaxValue > 60);
            TooLoudMeasurements = new ObservableCollection<NoiseMeasurementViewModel>(tooLoudMeasurements);
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
            TotalsPlotModel.Title = "Grenzwertüberschreitungen pro Messpunkt (gesamt)";

        }


        private void PopulatePlotModel()
        {
            PlotModel.Series.Clear();

            var noiseMeausurements =
                NoiseMeasurements.Where(nm => nm.MeasuringPoint != null).GroupBy(nm => nm.MeasuringPoint);


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