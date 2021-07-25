using System;

namespace AutoFixtureDemo.Models
{
  public class UserModel
  {
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }
  }
}