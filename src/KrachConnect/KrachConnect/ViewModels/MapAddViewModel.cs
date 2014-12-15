using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
  class MapAddViewModel
  {
    private NoiseRepository repository;
    private IEnumerable<MeasuringPointViewModel> measuringPoints;

    public MapAddViewModel(NoiseRepository repository)
    {
      this.repository = repository;
      measuringPoints = repository.MeasuringPoints;

    }

    public MeasuringPointViewModel SelectedMeasuringPoint
    {
      get { return measuringPoints.First(mp => mp.IsSelected); }
    }

    public IEnumerable<MeasuringPointViewModel> MeasuringPoints { get { return measuringPoints; }}
  }
}
