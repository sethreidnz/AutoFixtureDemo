using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using AutoFixtureDemo.Interfaces;
using AutoFixtureDemo.Models;
using FluentAssertions;
using Moq;
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

    [Theory, AutoMoq]
    public async Task CreateUser_GivenValidUser_ShouldCreateUser(
      [Frozen] Mock<IUserRepository> userRepositoryMock,
      UserService sut,
      UserModel user)
    {
      // arrange
      userRepositoryMock.Setup(s => s.GetUserByEmail(It.IsAny<string>()))
        .Returns(Task.FromResult<UserModel>(null));
      userRepositoryMock.Setup(s => s.CreateUser(It.IsAny<UserModel>()))
        .Returns(Task.FromResult(user));

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

    [Theory, AutoMoq]
    public async Task CreateUser_IfRequiredPropertiesAreNotProvided_ShouldThrowArgumentException(
      [Frozen] Mock<IUserRepository> userRepositoryMock,
      UserService sut,
      UserModel userWithoutUserName,
      UserModel userInvalidEmail,
      UserModel userWithoutEmail)
    {
      // arrange
      userWithoutUserName.UserName = string.Empty;
      userInvalidEmail.Email = "invalid email";
      userWithoutEmail.Email = string.Empty;

      // act
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

    [Theory, AutoMoq]
    public async Task CreateUser_IfUserWithEmailAddressAlreadyExists_ShouldThrowException(
      [Frozen] Mock<IUserRepository> userRepositoryMock,
      UserService sut,
      UserModel existingUser,
      UserModel userToCreate)
    {
      // arrange
      userToCreate.Email = existingUser.Email;
      userRepositoryMock.Setup(s => s.GetUserByEmail(It.IsAny<string>()))
        .Returns(Task.FromResult(userToCreate));

      // act
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