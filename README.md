# AutoFixture Demo

This repo is intended to give a practical view of what [AutoFixture](https://github.com/AutoFixture/AutoFixture) does and how it's used.

## Pre-requisites

In order to run the tests locally on your machine you will need the following installed:

- [.NET core](https://dotnet.microsoft.com/download)

## About the code

The solution is made up of two projects:

- [src/AutoFixtureDemo](src/AutoFixtureDemo)
- [src/AutoFixtureDemo.Tests](src/AutoFixtureDemo.Tests)

The code that we are testing is a service called `UserService` in `src/AutoFixtureDemo/UserService.cs`. It is a service with a dependency on a repository `IUserRepository` which can query and persist data from a data store. Note that `IUserRepository` does not have an implementation as I am mocking it out in the tests.

The tests are written four times each to illustrate the various features of AutoFixture and the AutoFixture ecosystem. There are three packages that This project uses provided by AutoFixture and it is easier to understand how they all fit together looking at them being introduced separately.

### The tests

Below are some details to help you understand what is happening between each re-write of the tests. But first in order to run the tests run the following from the command line:

```
cd src/AutoFixtureDemo.Tests
npm run test
```

#### Without AutoFixture

The first tests are regular unit tests using Xunit2, Moq and FluentAssertions. These can be found [here](src/AutoFixtureDemo.Tests/1WithoutAutoFixture/UserServiceTests.cs).

### AutoFixture only

The first set of tests using AutoFixture focus simply on the raw AutoFixture API [here](src/AutoFixtureDemo.Tests/2AutoFixtureOnly/UserServiceTests.cs). Notice that in the constructor an `IFixture` is instantiated manually. Then the fixture is configured to generate the `UserModel` correctly by using the method `IFixture.Create<T>` to create an email address using AutoFixtures built in generator for `System.Net.Mail.MailAddress`. This means I can have AutoFixture generate realistic data that will still pass my business validation rules but allow me not to have to manually generate values or hard code them.

Then in the tests instead of having to manually create the `UserModel` instances I use `IFixture.Create<T>` to create a generated `UserModel`. Although this is slight improvement, it would be nice if we didn't have to manually create the fixture.

### AutoFixture + AutoFixture.Xunit2

AutoFixture.Xunit2 is a package that integrates AutoFixture with Xunit's `[Theory]` attribute. What this means is that you can decorate your test with a custom attribute that you create which is responsible for instantiating and configuring the default fixture, and returning it for use in the tests using the `[Theory]` attribute.

This can be seen in the file [AutoFixture.cs](src/AutoFixtureDemo.Tests/3AutoFixtureXunit/AutoFixture.cs) which defines the attribute class `AutoMoq` which essentially moves the `IFixture` configuration from the constructor of the test, to this central location which all your tests can use.

This is then used in the test [here](src/AutoFixtureDemo.Tests/3AutoFixtureXunit/UserServiceTests.cs) by decorating the tests with `[Theory, AutoMoq]` instead of `Fact`.

The final step is now instead of manually using the `IFixture` to create the values, you can add parameters to the test method and AutoFixture.Xunit2 will take care of generating the values. For example:

```csharp
[Theory, AutoMoq]
public async Task CreateUser_GivenValidUser_ShouldCreateUser(
    UserModel user)
{
    var sut = new UserService()
    var result = sut.CreateUser(user);
    result.Should.BeEquivalentTo(user);
}
```

You may have noticed in these tests we are still having to manually create our mocks and the system under test (`sut`). In the next set of tests we will use `AutoFixture.AutoMoq` to do that for us.

### AutoFixture + AutoFixture.Xunit2 + AutoFixture.AutoMoq

Now that the core concepts of AutoFixture and it's integration with Xunit are in place the final powerful feature of AutoFixture is the integration with the mocking library `Moq`. Since in .NET we use dependency injection to manage dependencies, we can take advantage of that during our tests. Instead of manually creating our system under test (`sut`) and `Mock<T>` instances of our dependencies we can configure our `IFixture` instance to have do all of that for us.

In the [AutoFixture attribute class for these tests](src/AutoFixtureDemo.Tests/4AutoFixtureAndAutoMoq/AutoFixture.cs) you'll notice that we have the following lines:

```csharp
var autoMoqCustomization = new AutoMoqCustomization();
var fixture = new Fixture().Customize(autoMoqCustomization);
```

This is where we register the `AutoMoqCustomization` customization that will automatically create `Mock<T>` instances when generating classes. This means that you can create your system under test and mocks the same way you create your test data.

If you don't need to use any of the mocks you can just generate the `sut` like this:

```csharp
[Theory, AutoMoq]
public void UserService_IsIUserService(UserService sut)
{
    // arrange

    // act/assert
    sut.Should().BeAssignableTo<IUserService>();
}
```

If you need to do some setup on the mocks, you can pass them in like this:

```csharp
[Theory, AutoMoq]
public async Task CreateUser_GivenValidUser_ShouldCreateUser(
    [Frozen] Mock<IUserRepository> userRepositoryMock,
    UserService sut,
    UserModel user)
{
    // arrange
    userRepositoryMock.Setup(s => s.GetUserByEmail(It.IsAny<string>()))
    .Returns(Task.FromResult<UserModel>(null));
    userRepositoryMock.Setup(s => s.CreateUser(It.IsAny<UserModel>()))
    .Returns(Task.FromResult(user));

    //---- test --- //
}
```

Notice the use of the `[Frozen]` attribute **and** that the mock come before the service that depends on it. This attribute tells AutoFixture that if that specific service, `Mock<IUserRepository>` is asked for again in this test then return this very same instance. Meaning that when the next line we ask for `UserService sut` AutoFixture will resolve the dependency of `IUserRepository` using the mock that was created using the `[Frozen]` attribute.

This lets you then do the setup and verification on these mocks during your test, but you never have to do the dance of passing the mocks to the constructor of your system under test. This is good as the constructor is an implementation detail that shouldn't really be in your test. It also means if you add new dependencies to `IUserService` you don't have to update your tests except where you want to mock out the new dependency.

#### Injecting real services

Since we are essentially using AutoFixture's `IFixture` as a dependency container we can tell it to resolve real services if we want as well. This can be useful for pure logic like mappers and validators because you can use the real instance and test them for free while testing other code that consumes them. Since they have no dependencies, and you are using AutoFixture to generate realistic test data, the validation should pass and you are getting closer to testing the method or service as it's used in your application.

You can see an example of this in [AutoFixture attribute class for these tests](src/AutoFixtureDemo.Tests/4AutoFixtureAndAutoMoq/AutoFixture.cs):

```csharp
      fixture.Register<IValidator<UserModel>>(() => new UserModelValidator());
```

Here I am telling AutoFixture when it gets a request for `IValidator<UserModel>` to return a new instance of `UserModelValidator`.
