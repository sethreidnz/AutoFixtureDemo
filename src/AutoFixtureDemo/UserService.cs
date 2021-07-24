using System;
using AutoFixtureDemo.Business.Interfaces;
using AutoFixtureDemo.Business.Models;

namespace AutoFixtureDemo.Business
{
  public class UserService : IUserService
  {
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }
    public UserModel GetUserById(Guid userId)
    {
      throw new NotImplementedException();
    }
  }
}