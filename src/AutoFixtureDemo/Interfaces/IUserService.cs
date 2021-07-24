using System;
using AutoFixtureDemo.Business.Models;

namespace AutoFixtureDemo.Business.Interfaces
{
  public interface IUserService
  {
    public UserModel GetUserById(Guid userId);
  }
}