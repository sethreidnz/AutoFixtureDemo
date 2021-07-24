using System;
using AutoFixtureDemo.Business.Models;

namespace AutoFixtureDemo.Business.Services
{
  public interface IUserService
  {
    public UserModel GetUserById(Guid userId);
  }
}