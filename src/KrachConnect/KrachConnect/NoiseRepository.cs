using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Linq;
using System.Net;
using KrachConnect.DomainModelService;

namespace KrachConnect
{
  public class NoiseRepository
  {
    private readonly DomainModelContext _context;
    private DataServiceCollection<MeasuringPoint> _measuringPoints;
    private DataServiceCollection<NoiseMeasurement> _noiseMeasurements;
    private DataServiceCollection<NoiseMap> _maps;
    private List<MeasuringPointViewModel> measuringPointViewModels = new List<MeasuringPointViewModel>();

    public NoiseRepository()
    {
      _context = new DomainModelContext(new Uri("http://141.45.92.171:7000/OpenResKitHub"));
      _context.Credentials = new NetworkCredential("root", "ork123");
      LoadMeasuringPoints();
      LoadMaps();
      LoadNoiseMeasurements();

      //addMeasuringPoint();
    }

    public IEnumerable<MeasuringPointViewModel> MeasuringPoints
    {
      get { return measuringPointViewModels; }
    }

    public DataServiceCollection<NoiseMeasurement> NoiseMeasurements
    {
      get { return _noiseMeasurements; }
    }

    public DataServiceCollection<NoiseMap> Maps
    {
      get { return _maps; }
    }

    private void LoadNoiseMeasurements()
    {
      _noiseMeasurements = new DataServiceCollection<NoiseMeasurement>(_context);
      DataServiceQuery<NoiseMeasurement> query = _context.NoiseMeasurements.Expand("MeasuringPoint");
      _noiseMeasurements.Load(query);
    }

    private void LoadMaps()
    {
      _maps = new DataServiceCollection<NoiseMap>(_context);
      DataServiceQuery<NoiseMap> query = _context.NoiseMaps.Expand("File");
      _maps.Load(query);
    }

    private void LoadMeasuringPoints()
    {
      _measuringPoints = new DataServiceCollection<MeasuringPoint>(_context);
      DataServiceQuery<MeasuringPoint> query = _context.MeasuringPoints.Expand("Position");
      
      _measuringPoints.Load(query);
      foreach (var mp in _measuringPoints)
      {
        measuringPointViewModels.Add(new MeasuringPointViewModel(mp));
      }
      measuringPointViewModels.Last().IsSelected = true;
    }

    private void addMeasuringPoint()
    {
      var position = new NoiseMapPosition {XPosition = 300, YPosition = 190};
      var mp = new MeasuringPoint{Name = "Schleifmaschine", Position = position};
      _measuringPoints.Add(mp);
      Save();
    }

    private void Save()
    {
      IAsyncResult result = _context.BeginSaveChanges(SaveChangesOptions.Batch, r =>
      {
        var dm = (DomainModelContext) r.AsyncState;
        dm.EndSaveChanges(r);
      }, _context);
    }
  }
}