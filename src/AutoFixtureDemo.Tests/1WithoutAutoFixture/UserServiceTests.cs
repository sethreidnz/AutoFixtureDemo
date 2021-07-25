using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoFixtureDemo.Interfaces;
using AutoFixtureDemo.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace AutoFixtureDemo.Tests._1WithoutAutoFixture
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

    [Fact]
    public async Task CreateUser_GivenValidUser_ShouldCreateUser()
    {
      // arrange
      var user = new UserModel
      {
        Id = Guid.NewGuid(),
        UserName = "ExampleUser",
        Email = "email@example.com"
      };
      var userRepositoryMock = new Mock<IUserRepository>();
      var userValidator = new UserModelValidator();
      userRepositoryMock.Setup(s => s.GetUserByEmail(It.IsAny<string>()))
        .Returns(Task.FromResult<UserModel>(null));
      userRepositoryMock.Setup(s => s.CreateUser(It.IsAny<UserModel>()))
        .Returns(Task.FromResult(user));
      var sut = new UserService(userRepositoryMock.Object, userValidator);

      // act/assert
      var result = await sut.CreateUser(user);

      // assert
      result.Should().BeEquivalentTo(user);
      userRepositoryMock.Verify(s => s.GetUserByEmail(It.Is<string>(v =>
          v == user.Email)),
        Times.Once);
      userRepositoryMock.Verify(s => s.CreateUser(It.Is<UserModel>(v =>
          v.Id == user.Id &&
          v.UserName == user.UserName &&
          v.Email == user.Email)),
        Times.Once);
    }

    [Fact]
    public async Task CreateUser_IfRequiredPropertiesAreNotProvided_ShouldThrowArgumentException()
    {
      // arrange
      var userWithoutUserName = new UserModel
      {
        Id = Guid.NewGuid(),
        UserName = string.Empty,
        Email = "email@example.com"
      };
      var userInvalidEmail = new UserModel
      {
        Id = Guid.NewGuid(),
        UserName = "ExampleUser",
        Email = "invalid email",
      };
      var userWithoutEmail = new UserModel
      {
        Id = Guid.NewGuid(),
        UserName = "ExampleUser",
        Email = string.Empty,
      };
      var userRepositoryMock = new Mock<IUserRepository>();
      var userValidator = new UserModelValidator();
      var sut = new UserService(userRepositoryMock.Object, userValidator);

      // act/assert
      Func<Task> createWithoutUserName = async () => await sut.CreateUser(userWithoutUserName);
      Func<Task> createWithInvalidEmail = async () => await sut.CreateUser(userWithoutEmail);
      Func<Task> createWithoutEmail = async () => await sut.CreateUser(userWithoutEmail);

      // assert
      await createWithoutEmail.Should().ThrowAsync<FluentValidation.ValidationException>();
      await createWithInvalidEmail.Should().ThrowAsync<FluentValidation.ValidationException>();
      await createWithoutUserName.Should().ThrowAsync<FluentValidation.ValidationException>();
      userRepositoryMock.Verify(s => s.CreateUser(It.Is<UserModel>(v =>
          v.UserName == userWithoutUserName.UserName &&
          v.Email == userWithoutUserName.Email)),
        Times.Never);
    }

    [Fact]
    public async Task CreateUser_IfUserWithEmailAddressAlreadyExists_ShouldThrowException()
    {
      // arrange
      var userToCreate = new UserModel
      {
        Id = Guid.NewGuid(),
        UserName = "ExampleUser",
        Email = "email@example.com"
      };
      var existingUser = new UserModel
      {
        Id = Guid.NewGuid(),
        UserName = "ExampleUser",
        Email = "email@example.com"
      };
      var userRepositoryMock = new Mock<IUserRepository>();
      userRepositoryMock.Setup(s => s.GetUserByEmail(It.IsAny<string>()))
        .Returns(Task.FromResult(userToCreate));
      var userValidator = new UserModelValidator();
      var sut = new UserService(userRepositoryMock.Object, userValidator);

      // act/assert
      Func<Task> createUser = async () => await sut.CreateUser(userToCreate);

      // assert
      await createUser.Should().ThrowAsync<Exception>();
      userRepositoryMock.Verify(s => s.GetUserByEmail(It.Is<string>(v =>
          v == userToCreate.Email)),
        Times.Once);
      userRepositoryMock.Verify(s => s.CreateUser(It.Is<UserModel>(v =>
          v.UserName == userToCreate.UserName &&
          v.Email == userToCreate.Email)),
        Times.Never);
    }
  }
}