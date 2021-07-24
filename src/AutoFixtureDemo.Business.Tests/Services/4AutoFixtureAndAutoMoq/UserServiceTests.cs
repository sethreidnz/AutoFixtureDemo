using AutoFixture.Xunit2;
using AutoFixtureDemo.Business.Services;
using FluentAssertions;
using Xunit;

namespace AutoFixtureDemo.Business.Tests.Services._4AutoFixtureAndAutoMoq
{
  public class UserServiceTests
  {
    [Theory, AutoData]
    public void UserService_IsIUserService(UserService sut)
    {
      // arrange

      // act/assert
      sut.Should().BeAssignableTo<IUserService>();
    }
  }
}