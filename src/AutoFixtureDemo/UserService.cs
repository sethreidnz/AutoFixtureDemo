using System;
using System.Threading.Tasks;
using AutoFixtureDemo.Interfaces;
using AutoFixtureDemo.Models;
using FluentValidation;

namespace AutoFixtureDemo
{
  public class UserService : IUserService
  {
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UserModel> _userValidator;

    public UserService(IUserRepository userRepository, IValidator<UserModel> userValidator)
    {
      _userRepository = userRepository;
      _userValidator = userValidator;
    }

    public async Task<UserModel> CreateUser(UserModel user)
    {
      _userValidator.ValidateAndThrow(user);

      var existingUser = await _userRepository.GetUserByEmail(user.Email);
      if (existingUser != null)
      {
        throw new Exception("User with that email already exists");
      }

      return await _userRepository.CreateUser(user);
    }
  }
}