using AutoFixtureDemo.Business.Services;
using FluentAssertions;
using Xunit;

namespace AutoFixtureDemo.Business.Tests.Services._2AutoFixtureOnly
{
  public class UserServiceTests
  {
    [Fact]
    public void UserService_IsIUserService()
    {
      // arrange
      var sut = new UserService();

      // act/assert
      sut.Should().BeAssignableTo<IUserService>();
    }
  }
}