using KrachConnect.DomainModelService;
using NUnit.Framework;

namespace KrachConnect.Tests
{
  internal class NoiseMeasurmentAddViewModelFixture
  {
    [Test]
    public void Adding_A_New_NoiseMeasurement_To_MeasuringPoint()
    {
      var mp = new MeasuringPoint {Name = "Test"};
      var nm = new NoiseMeasurement {MeasuringPoint = mp};
      Assert.AreEqual(nm.MeasuringPoint, mp);
      Assert.AreEqual(nm.MeasuringPoint.Name, "Test");
    }
  }
}