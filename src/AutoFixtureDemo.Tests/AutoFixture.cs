using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using AutoFixtureDemo.Models;
using FluentValidation;

namespace AutoFixtureDemo.Tests
{
  public class AutoMoq : AutoDataAttribute
  {
    public AutoMoq() : base(GetDefaultFixture)
    {
    }

    private static IFixture GetDefaultFixture()
    {
      var autoMoqCustomization = new AutoMoqCustomization();
      var fixture = new Fixture().Customize(autoMoqCustomization);
      fixture.Register<IValidator<UserModel>>(() => new UserModelValidator());
      return fixture;
    }
  }
}