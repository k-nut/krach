using System;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Net;
using KrachConnect.DomainModelService;

namespace KrachConnect
{
  internal class NoiseRepository
  {
    private readonly DomainModelContext _context;
    private DataServiceCollection<MeasuringPoint> _measuringPoints;
    private DataServiceCollection<NoiseMeasurement> _noiseMeasurements;

    public NoiseRepository()
    {
      _context = new DomainModelContext(new Uri("http://141.45.92.171:7000/OpenResKitHub"));
      _context.Credentials = new NetworkCredential("root", "ork123");
      LoadMeasuringPoints();
      LoadNoiseMeasurements();
    }

    private void LoadNoiseMeasurements()
    {
      _noiseMeasurements = new DataServiceCollection<NoiseMeasurement>(_context);
      DataServiceQuery<NoiseMeasurement> query = _context.NoiseMeasurements;
      _noiseMeasurements.Load(query);
      Debug.WriteLine(_noiseMeasurements);
    }

    private void LoadMeasuringPoints()
    {
      _measuringPoints = new DataServiceCollection<MeasuringPoint>(_context);
      DataServiceQuery<MeasuringPoint> query = _context.MeasuringPoints;
      _measuringPoints.Load(query);
    }

    private void Save()
    {
      IAsyncResult result = _context.BeginSaveChanges(SaveChangesOptions.Batch, r =>
      {
        var dm = (DomainModelContext) r.AsyncState;
        dm.EndSaveChanges(r);
      }, _context);
    }

    public DataServiceCollection<MeasuringPoint> MeasuringPoints {
      get { return _measuringPoints; }
    }

    public DataServiceCollection<NoiseMeasurement> NoiseMeasurements
    {
      get { return _noiseMeasurements; }
    }
  }
}