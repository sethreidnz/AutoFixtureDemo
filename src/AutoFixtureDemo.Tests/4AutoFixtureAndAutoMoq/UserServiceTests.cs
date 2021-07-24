using AutoFixtureDemo.Business;
using AutoFixtureDemo.Business.Interfaces;
using FluentAssertions;
using Xunit;

namespace AutoFixtureDemo.Tests._4AutoFixtureAndAutoMoq
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