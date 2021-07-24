using AutoFixtureDemo.Business;
using AutoFixtureDemo.Business.Interfaces;
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
      var sut = new UserService(userRepositoryMock.Object);

      // act/assert
      sut.Should().BeAssignableTo<IUserService>();
    }
  }
}