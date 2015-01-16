using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using System.Windows.Forms.VisualStyles;
using KrachConnect.DomainModelService;

namespace KrachConnect
{
  public class NoiseRepository
  {
    private readonly DomainModelContext _context;
    private DataServiceCollection<NoiseMap> _maps;
    private DataServiceCollection<MeasuringPoint> _measuringPoints;
    private DataServiceCollection<NoiseMeasurement> _noiseMeasurements;
    private DataServiceCollection<MeasuringMethod> _measuringMethods;
    private IEnumerable<MeasuringPointViewModel> measuringPointViewModels;

    public NoiseRepository()
    {
      _context = new DomainModelContext(new Uri("http://141.45.92.171:7000/OpenResKitHub"));
      _context.Credentials = new NetworkCredential("root", "ork123");

      LoadMeasuringPoints();
      LoadMaps();
      LoadNoiseMeasurements();
      LoadMeasuringMethods();
       //deleteMeasuringPointsWithoutPosition();
    }

    public List<MeasuringPointViewModel> MeasuringWalk { get; set; }


    public IEnumerable<MeasuringPointViewModel> MeasuringPointViewModels
    {
      get { return measuringPointViewModels; }
    }

    public DataServiceCollection<MeasuringPoint> MeasuringPoints
    {
      get { return _measuringPoints; }
      set { _measuringPoints = value; }
    }

    public DataServiceCollection<NoiseMeasurement> NoiseMeasurements
    {
      get { return _noiseMeasurements; }
      set { _noiseMeasurements = value; }
    }

    public DataServiceCollection<NoiseMap> Maps
    {
      get { return _maps; }
    }

      public DataServiceCollection<MeasuringMethod> MeasuringMethods
      {
          get { return _measuringMethods; }
          set { _measuringMethods = value; }
      }

      private void LoadNoiseMeasurements()
    {
      _noiseMeasurements = new DataServiceCollection<NoiseMeasurement>(_context);
      DataServiceQuery<NoiseMeasurement> query = _context.NoiseMeasurements.Expand("MeasuringPoint").Expand("Method");
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
      measuringPointViewModels = _measuringPoints.Select(mp => new MeasuringPointViewModel(mp));
    }

    private void LoadMeasuringMethods()
    {
        _measuringMethods = new DataServiceCollection<MeasuringMethod>(_context);
        DataServiceQuery<MeasuringMethod> query = _context.MeasuringMethods;

        _measuringMethods.Load(query);
        //measu = _measuringMethods.Select(mp => new MeasuringPointViewModel(mp));
    }

    private void deleteMeasuringPointsWithoutPosition()
    {
      //List<MeasuringPoint> measuringPointsWithPosition =
      //  MeasuringPoints.Where(mp => mp.Position.XPosition != 0 && mp.Position.YPosition != 0).ToList();
      foreach (var noiseMeasurement in NoiseMeasurements)
      {
        _context.DeleteObject(noiseMeasurement);
      }
      foreach (var measuringPoint in MeasuringPoints)
      {
        _context.DeleteObject(measuringPoint);
      }
      //MeasuringPoints.Load(measuringPointsWithPosition);
      Save();
    }

    public void Save()
    {
      IAsyncResult result = _context.BeginSaveChanges(SaveChangesOptions.Batch, r =>
      {
        var dm = (DomainModelContext) r.AsyncState;
        dm.EndSaveChanges(r);
      }, _context);
    }
  }
}