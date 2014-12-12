using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KrachConnect.DomainModelService;
using Moq;
using NUnit.Framework;

namespace KrachConnect.Tests
{
  class NoiseMeasurmentAddViewModelFixture
  {
    [Test]
    public void Adding_A_New_NoiseMeasurement_To_MeasuringPoint()
    {
      var measuringPointMock = new Mock<MeasuringPoint>();
      measuringPointMock.Setup(mpm => mpm.Name).Returns("Test");
//      var mp = new MeasuringPoint{Name="Test"};
      var nm = new NoiseMeasurement {MeasuringPoint = measuringPointMock.Object};
      Assert.AreEqual(nm.MeasuringPoint, measuringPointMock.Object.Name);
      Assert.AreEqual(nm.MeasuringPoint.Name, "Test");
    }
  }
}
