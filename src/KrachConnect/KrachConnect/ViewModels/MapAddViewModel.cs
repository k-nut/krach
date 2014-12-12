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
    private DataServiceCollection<MeasuringPoint> measuringPoints;

    public MapAddViewModel(NoiseRepository repository)
    {
      this.repository = repository;
      measuringPoints = repository.MeasuringPoints;

    }

    public IEnumerable<MeasuringPoint> MeasuringPoints { get { return measuringPoints; }}
  }
}
