using System;
using System.Threading.Tasks;
using AutoFixtureDemo.Interfaces;
using AutoFixtureDemo.Models;

namespace AutoFixtureDemo
{
  public class UserService : IUserService
  {
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }

    public async Task<UserModel> CreateUser(UserModel user)
    {
      if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Email))
      {
        throw new ArgumentException();
      }

      var existingUser = await _userRepository.GetUserByEmail(user.Email);
      if (existingUser != null)
      {
        throw new Exception("User with that email already exists");
      }

      return await _userRepository.CreateUser(user);
    }
  }
}