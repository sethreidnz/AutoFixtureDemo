using System;
using FluentValidation;

namespace AutoFixtureDemo.Models
{
  public class UserModel
  {
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }
  }

  public class UserModelValidator : AbstractValidator<UserModel>
  {
    public UserModelValidator()
    {
      RuleFor(v => v.Id)
        .NotEmpty();
      RuleFor(v => v.UserName)
        .NotEmpty();
      RuleFor(v => v.Email)
        .EmailAddress();
    }
  }
}