using System.Threading.Tasks;
using AutoFixtureDemo.Models;

namespace AutoFixtureDemo.Interfaces
{
  public interface IUserRepository
  {
    Task<UserModel> CreateUser(UserModel user);
    Task<UserModel> GetUserByEmail(string email);
  }
}