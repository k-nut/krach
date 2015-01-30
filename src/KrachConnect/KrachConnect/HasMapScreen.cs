using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.PeerResolvers;
using Caliburn.Micro;
using KrachConnect.DomainModelService;
using Action = System.Action;
using File = System.IO.File;

namespace KrachConnect
{
  public class HasMapScreen : Screen
  {
    private NoiseMap _activeMap;
    private IEnumerable<NoiseMap> _maps;
    public void Update(){}
    
    public HasMapScreen(NoiseRepository repository)
    {
      NoiseMaps = repository.Maps;
      ActiveMap = NoiseMaps.First();
    }

    public IEnumerable<NoiseMap> NoiseMaps
    {
      get { return _maps; }
      set
      {
        _maps = value;
        NotifyOfPropertyChange(() => NoiseMaps);
      }
    }

    public NoiseMap ActiveMap
    {
      get { return _activeMap; }
      set
      {
        _activeMap = value;
        NotifyOfPropertyChange(() => ActiveMap);
        NotifyOfPropertyChange(() => ActiveMapPath);
        Update();
      }
    }

    public String ActiveMapPath
    {
      get
      {
        string filePath = Path.GetTempFileName();
        File.WriteAllBytes(filePath, ActiveMap.File.BinarySource);
        return filePath;
      }
    }
  }
}