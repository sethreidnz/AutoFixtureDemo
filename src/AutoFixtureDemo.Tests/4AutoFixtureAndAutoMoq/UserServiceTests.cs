using AutoFixture.Xunit2;
using AutoFixtureDemo.Interfaces;
using FluentAssertions;
using Xunit;

namespace AutoFixtureDemo.Tests._4AutoFixtureAndAutoMoq
{
  public class UserServiceTests
  {
    [Theory, AutoMoq]
    public void UserService_IsIUserService(UserService sut)
    {
      // arrange

      // act/assert
      sut.Should().BeAssignableTo<IUserService>();
    }
  }
}