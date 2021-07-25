using AutoFixtureDemo.Interfaces;
using AutoFixtureDemo.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace AutoFixtureDemo.Tests._2AutoFixtureOnly
{
  public class UserServiceTests
  {
    [Fact]
    public void UserService_IsIUserService()
    {
      // arrange
      var userRepositoryMock = new Mock<IUserRepository>();
      var userValidator = new UserModelValidator();
      var sut = new UserService(userRepositoryMock.Object, userValidator);

      // act/assert
      sut.Should().BeAssignableTo<IUserService>();
    }
  }
}