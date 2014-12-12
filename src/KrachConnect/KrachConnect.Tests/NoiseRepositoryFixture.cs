using KrachConnect.DomainModelService;
using NUnit.Framework;

namespace KrachConnect.Tests
{
  internal class NoiseRepositoryFixture
  {
    [Test]
    public void After_Initialization_MeasuringPoints_Is_Not_Null()
    {
      var nr = new NoiseRepository();
      Assert.NotNull(nr.MeasuringPoints);
    }
    [Test]
    public void After_Initialization_NoiseMeasurments_Is_Not_Null()
    {
      var nr = new NoiseRepository();
      Assert.NotNull(nr.NoiseMeasurements);
    }
  }
}