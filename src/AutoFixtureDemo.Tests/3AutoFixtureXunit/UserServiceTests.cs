using AutoFixtureDemo.Business;
using AutoFixtureDemo.Business.Interfaces;
using FluentAssertions;
using Xunit;

namespace AutoFixtureDemo.Tests._3AutoFixtureXunit
{
  public class UserServiceTests
  {
    [Theory, AutoMoqData]
    public void UserService_IsIUserService(UserService sut)
    {
      // arrange

      // act/assert
      sut.Should().BeAssignableTo<IUserService>();
    }
  }
}