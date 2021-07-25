using System;
using System.Net.Mail;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoFixtureDemo.Interfaces;
using AutoFixtureDemo.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace AutoFixtureDemo.Tests._3AutoFixtureXunit
{
  public class UserServiceTests
  {
    private readonly IFixture _fixture;
    public UserServiceTests()
    {
      var fixture = new Fixture();
      fixture.Customize<UserModel>(c =>
        c.With(v => v.Email, fixture.Create<MailAddress>().Address));
      _fixture = fixture;
    }

    [Theory, AutoMoq]
    public void UserService_IsIUserService(UserService sut)
    {
      // arrange

      // act/assert
      sut.Should().BeAssignableTo<IUserService>();
    }

    [Theory, AutoMoq]
    public async Task CreateUser_GivenValidUser_ShouldCreateUser(
      UserModelValidator userValidator,
      UserModel user)
    {
      // arrange
      var userRepositoryMock = new Mock<IUserRepository>();
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

    [Theory, AutoMoq]
    public async Task CreateUser_IfRequiredPropertiesAreNotProvided_ShouldThrowArgumentException(
      UserModel userWithoutUserName,
      UserModel userInvalidEmail,
      UserModel userWithoutEmail)
    {
      // arrange
      userWithoutUserName.UserName = string.Empty;
      userInvalidEmail.Email = "invalid email";
      userWithoutEmail.Email = string.Empty;

      var userRepositoryMock = new Mock<IUserRepository>();
      var userValidator = new UserModelValidator();
      var sut = new UserService(userRepositoryMock.Object, userValidator);

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
      UserModel existingUser,
      UserModel userToCreate)
    {
      // arrange
      userToCreate.Email = existingUser.Email;
      var userRepositoryMock = new Mock<IUserRepository>();
      userRepositoryMock.Setup(s => s.GetUserByEmail(It.IsAny<string>()))
        .Returns(Task.FromResult(userToCreate));
      var userValidator = new UserModelValidator();
      var sut = new UserService(userRepositoryMock.Object, userValidator);

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