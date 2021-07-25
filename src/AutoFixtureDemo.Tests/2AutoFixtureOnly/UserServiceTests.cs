using System;
using System.Net.Mail;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixtureDemo.Interfaces;
using AutoFixtureDemo.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace AutoFixtureDemo.Tests._2AutoFixtureOnly
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
      var user = _fixture.Create<UserModel>();
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
      var userWithoutUserName = _fixture.Create<UserModel>();
      userWithoutUserName.UserName = string.Empty;
      var userInvalidEmail = _fixture.Create<UserModel>();
      userInvalidEmail.Email = "invalid email";
      var userWithoutEmail = _fixture.Create<UserModel>();
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

    [Fact]
    public async Task CreateUser_IfUserWithEmailAddressAlreadyExists_ShouldThrowException()
    {
      // arrange
      var existingUser = _fixture.Create<UserModel>();
      var userToCreate = _fixture.Create<UserModel>();
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