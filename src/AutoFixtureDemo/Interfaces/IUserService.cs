using System;
using System.Threading.Tasks;
using AutoFixtureDemo.Models;

namespace AutoFixtureDemo.Interfaces
{
  public interface IUserService
  {
    Task<UserModel> CreateUser(UserModel user);
  }
}