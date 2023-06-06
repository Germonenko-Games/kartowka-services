using System.Security.Cryptography;
using Kartowka.Authorization.Core.Contracts;
using Kartowka.Authorization.Core.Models;
using Kartowka.Authorization.Core.Resources;
using Kartowka.Authorization.Core.Services;
using Kartowka.Common.Crypto.Abstractions;
using Kartowka.Core;
using Kartowka.Core.Exceptions;
using Kartowka.Core.Models;
using Kartowka.Core.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;

namespace Kartowka.Authorization.Tests.Services;

[TestClass, TestCategory("Authorization")]
public class UserAuthorizationServiceTests
{
    private const int ActiveUserId = 1;

    private const int InactiveUserId = 2;
    
    private const string ActiveUserEmail = "active@email.com";
    
    private const string InactiveUserEmail = "inactive@email.com";
    
    private const string ActiveUsername = "active_user";
    
    private const string InactiveUsername = "inactive_user";
    
    private const string TestPassword = "password123!@#";
    
    private static readonly Mock<IAccessTokenGenerator> AccessTokenGeneratorMock = new();

    private static readonly Mock<IHasher> HasherMock = new();

    private static readonly Mock<IStringLocalizer<ErrorMessages>> StringLocalizer = new();

    private static readonly CoreContext Context;
    
    private static readonly UserAuthorizationService UserAuthorizationService;

    static UserAuthorizationServiceTests()
    {
        var options = new DbContextOptionsBuilder<CoreContext>()
            .UseInMemoryDatabase(nameof(UserAuthorizationServiceTests))
            .Options;

        Context = new CoreContext(options);
        UserAuthorizationService = new(
            Context,
            AccessTokenGeneratorMock.Object,
            HasherMock.Object,
            StringLocalizer.Object
        );
    }

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        Context.Users.Add(new User
        {
            Id = ActiveUserId,
            Status = UserStatus.Active,
            Username = ActiveUsername,
            EmailAddress = ActiveUserEmail,
            PasswordHash = RandomNumberGenerator.GetBytes(10),
            PasswordSalt = RandomNumberGenerator.GetBytes(10)
        });
        Context.Users.Add(new User
        {
            Id = InactiveUserId,
            Status = UserStatus.Unverified,
            Username = InactiveUsername,
            EmailAddress = InactiveUserEmail,
            PasswordHash = RandomNumberGenerator.GetBytes(10),
            PasswordSalt = RandomNumberGenerator.GetBytes(10)
        });
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
    }

    [TestMethod("Authorize valid user")]
    public async Task AuthorizeValidUser_Should_ReturnTokenInfo()
    {
        var user = await Context.Users.AsNoTracking().FirstAsync(u => u.EmailAddress == ActiveUserEmail);

        var token = new TokenInfo
        {
            AccessToken = "test123",
            IssueDate = DateTimeOffset.Now,
            ExpireDate = DateTimeOffset.Now,
        };

        AccessTokenGeneratorMock
            .Setup(generator => generator.GenerateToken(It.Is<User>(u => u.EmailAddress == ActiveUserEmail)))
            .Returns(token);

        HasherMock
            .Setup(hasher => hasher.Hash(TestPassword, user.PasswordSalt!))
            .Returns(user.PasswordHash!);

        var result = await UserAuthorizationService.AuthorizeAsync(new()
        {
            EmailAddress = ActiveUserEmail,
            Password = TestPassword,
        });
        
        Assert.AreEqual(token, result);
        
        AccessTokenGeneratorMock.VerifyAll();
        HasherMock.VerifyAll();
    }

    [TestMethod("Try authorize user that does not exist")]
    [ExpectedException(typeof(KartowkaNotFoundException))]
    public async Task AuthorizeUserThatDoesNotExist_Should_ThrowException()
    {
        await UserAuthorizationService.AuthorizeAsync(new()
        {
            EmailAddress = $"{ActiveUserEmail}1",
            Password = "test",
        });
    }

    [TestMethod("Try authorize nullable password to sign in")]
    [ExpectedException(typeof(KartowkaNotFoundException))]
    public async Task TryAuthorizeUserUsingNullablePassword_Should_ThrowException()
    {
        await UserAuthorizationService.AuthorizeAsync(new()
        {
            EmailAddress = ActiveUserEmail,
        });
    }
    
    [TestMethod("Try authorize nullable password to sign in")]
    [ExpectedException(typeof(KartowkaNotFoundException))]
    public async Task TryAuthorizeUserUsingIncorrectPassword_Should_ThrowException()
    {
        var user = await Context.Users.AsNoTracking().FirstAsync(u => u.EmailAddress == ActiveUserEmail);
        
        HasherMock
            .Setup(hasher => hasher.Hash(TestPassword, user.PasswordSalt!))
            .Returns(user.PasswordHash!);
        
        await UserAuthorizationService.AuthorizeAsync(new()
        {
            EmailAddress = ActiveUserEmail,
            Password = $"{TestPassword}1"
        });
    }

    [TestMethod("Try authorize inactive user")]
    [ExpectedException(typeof(KartowkaException))]
    public async Task TryAuthorizeInactiveUser_Should_ThrowException()
    {
        var user = await Context.Users.AsNoTracking().FirstAsync(u => u.EmailAddress == InactiveUserEmail);
        
        HasherMock
            .Setup(hasher => hasher.Hash(TestPassword, user.PasswordSalt!))
            .Returns(user.PasswordHash!);

        await UserAuthorizationService.AuthorizeAsync(new()
        {
            EmailAddress = InactiveUserEmail,
            Password = TestPassword
        });
    }
}