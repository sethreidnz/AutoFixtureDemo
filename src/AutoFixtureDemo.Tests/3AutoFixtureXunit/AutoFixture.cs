using System;
using System.Net.Mail;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using AutoFixtureDemo.Models;
using FluentValidation;

namespace AutoFixtureDemo.Tests._3AutoFixtureXunit
{
  public class AutoMoq : AutoDataAttribute
  {
    public AutoMoq() : base(GetDefaultFixture)
    {
    }

    private static IFixture GetDefaultFixture()
    {
      var fixture = new Fixture();

      // customise the user model to make the data valid and pass the tests
      fixture.Customize<UserModel>(c =>
        c.With(v => v.Email, fixture.Create<MailAddress>().Address));

      return fixture;
    }
  }
}