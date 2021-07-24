using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace AutoFixtureDemo.Tests
{
  
  public class AutoMoqData : AutoDataAttribute
  {
    public AutoMoqData() : base(GetDefaultFixture)
    {
    }

    private static IFixture GetDefaultFixture()
    {
      var autoMoqCustomization = new AutoMoqCustomization();
      var fixture = new Fixture().Customize(autoMoqCustomization);
      return fixture;
    }
  }
}