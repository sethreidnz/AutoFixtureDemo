using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace AutoFixtureDemo.Business.Tests
{
  public class AutoFixture
  {
    public class AutoMoqCommon : AutoDataAttribute
    {
      public AutoMoqCommon() : base(GetDefaultFixture)
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
}