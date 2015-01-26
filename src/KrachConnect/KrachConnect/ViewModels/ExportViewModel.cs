using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using EPPlusEnumerable;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing;
using Screen = Caliburn.Micro.Screen;

namespace KrachConnect.ViewModels
{
  class ExportViewModel : Screen
  {
    private NoiseRepository repository;
    private IEnumerable<NoiseMeasurementViewModel> noiseMeasurements;
    private DateTime minDate = DateTime.Today.AddYears(-1);
    private DateTime maxDate = DateTime.Today.AddDays(1);
    private List<NoiseMeasurementViewModel> _selectedNoiseMeasurementViewModels = new List<NoiseMeasurementViewModel>();

    private IEnumerable<NoiseMeasurementViewModel> selectedNoiseMeasurements;


    public ExportViewModel(NoiseRepository repository)
    {
      this.repository = repository;
      this.noiseMeasurements = repository.NoiseMeasurements.Select(nm => new NoiseMeasurementViewModel(nm));
      NotifyOfPropertyChange(() => FilteredNoiseMeasurements);
    }

    public IEnumerable<NoiseMeasurementViewModel> FilteredNoiseMeasurements
    {
      get { return noiseMeasurements.Where(nm => nm.MeasurementDate >= MinDate && nm.MeasurementDate <= MaxDate); }
    }

    public DateTime MinDate
    {
      get { return minDate; }
      set
      {
        minDate = value;
        NotifyOfPropertyChange(() => MinDate);
        NotifyOfPropertyChange(() => FilteredNoiseMeasurements);
      }
    }

    public DateTime MaxDate
    {
      get { return maxDate; }
      set
      {
        maxDate = value;
        NotifyOfPropertyChange(() => MaxDate);
        NotifyOfPropertyChange(() => FilteredNoiseMeasurements);
      }
    }

    public void SelectedRowsChangeEvent(SelectionChangedEventArgs e)
    {
      foreach (var addedRow in e.AddedItems)
      {
        _selectedNoiseMeasurementViewModels.Add(addedRow as NoiseMeasurementViewModel);
      }

      foreach (var removedRow in e.RemovedItems)
      {
        _selectedNoiseMeasurementViewModels.Remove(removedRow as NoiseMeasurementViewModel);
      }
    }

    public void Export()
    {

      Stream myStream;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();

      //saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
      saveFileDialog1.FilterIndex = 2;
      saveFileDialog1.FileName = "Export.xlsx";
      saveFileDialog1.DefaultExt = "xlsx";
      saveFileDialog1.RestoreDirectory = true;

      if (saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        myStream = saveFileDialog1.OpenFile();
        using (var package = new ExcelPackage(myStream))
        {
          var groupedNoiseMeasurements = _selectedNoiseMeasurementViewModels.Where(nm => nm.MeasuringPoint != null)// TODO: If the data is valid this cannot happen
            .GroupBy(nm => nm.MeasuringPoint);
          foreach (var group in groupedNoiseMeasurements)
          {
            var name = group.Key.Name;
            while (package.Workbook.Worksheets.Any(ws => ws.Name == name))
            {
              name = "(anderer) " + name;
            }
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(name);
            worksheet.Cells[1, 1].Value = "Datum";
            worksheet.Cells[1, 2].Value = "Minimalwert";
            worksheet.Cells[1, 3].Value = "Durchschnittswert";
            worksheet.Cells[1, 4].Value = "Maximalwert";
            var list = group.ToList();
            for (var i = 0; i < list.Count(); i++)
            {
              var nm = list[i];
              worksheet.Cells[i + 2, 1].Value = nm.MeasurementDate;
              worksheet.Cells[i + 2, 1].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
              worksheet.Cells[i + 2, 2].Value = nm.MinValue;
              worksheet.Cells[i + 2, 3].Value = nm.AverageValue;
              worksheet.Cells[i + 2, 4].Value = nm.MaxValue;
            }
            worksheet.Cells.AutoFitColumns(0);
          }
          package.Save();
        }
        myStream.Close();
      }
    }
  }
}
