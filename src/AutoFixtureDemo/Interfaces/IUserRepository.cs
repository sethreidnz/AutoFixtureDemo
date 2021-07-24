using System;
using AutoFixtureDemo.Business.Models;

namespace AutoFixtureDemo.Business.Interfaces
{
  public interface IUserRepository
  {
    public UserModel GetUserById(Guid userId);
  }
}