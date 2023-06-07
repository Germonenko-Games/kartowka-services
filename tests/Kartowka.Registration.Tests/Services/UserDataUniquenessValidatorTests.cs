using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Kartowka.Core;
using Kartowka.Core.Models;
using Kartowka.Core.Models.Enums;
using Kartowka.Registration.Core.Models;
using Kartowka.Registration.Core.Resources;
using Kartowka.Registration.Core.Services.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;

namespace Kartowka.Registration.Tests.Services;

[TestClass, TestCategory("Registration")]
public class UserDataUniquenessValidatorTests
{
    private const string TestUsername = "test_user";

    private const string TestEmail = "test@user.com";
    
    private static readonly CoreContext Context;

    private static readonly Mock<IStringLocalizer<ErrorMessages>> StringLocalizerMock;

    private static readonly UserDataUniquenessValidator UserDataUniquenessValidator;
    
    static UserDataUniquenessValidatorTests()
    {
        var contextOptionsBuilder = new DbContextOptionsBuilder<CoreContext>()
            .UseInMemoryDatabase(nameof(UserDataUniquenessValidatorTests));

        Context = new(contextOptionsBuilder.Options);
        StringLocalizerMock = new();
        UserDataUniquenessValidator = new(Context, StringLocalizerMock.Object);
    }

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        Context.Users.Add(new User
        {
            Id = 1,
            Status = UserStatus.Active,
            Username = TestUsername,
            EmailAddress = TestEmail,
            PasswordHash = RandomNumberGenerator.GetBytes(10),
            PasswordSalt = RandomNumberGenerator.GetBytes(10)
        });
        Context.SaveChanges();
        Context.ChangeTracker.Clear();

        StringLocalizerMock
            .Setup(localizer => localizer[It.IsAny<string>()])
            .Returns(new LocalizedString("123", "123"));
    }

    [TestMethod("Pass unique user to user uniqueness validator")]
    public async Task PassNewUser_Should_MarkAsValid()
    {
        var duplicateUser = new UserData
        {
            EmailAddress = $"{TestUsername}1",
            Username = $"{TestEmail}1",
        };

        var errors = new List<ValidationResult>();
        var valid = await UserDataUniquenessValidator.ValidateAsync(duplicateUser, errors);
        
        Assert.AreEqual(0, errors.Count);
        Assert.IsTrue(valid);
    }

    [TestMethod("Pass duplicate user to user uniqueness validator")]
    [DataRow(TestEmail, "", 1)]
    [DataRow("", TestUsername, 1)]
    [DataRow(TestEmail, TestUsername, 2)]
    public async Task PassDuplicateUser_Should_MarkAsInvalid(string username, string email, int errorsCount)
    {
        var duplicateUser = new UserData
        {
            EmailAddress = username,
            Username = email,
        };

        var errors = new List<ValidationResult>();
        var valid = await UserDataUniquenessValidator.ValidateAsync(duplicateUser, errors);
        
        Assert.AreEqual(errorsCount, errors.Count);
        Assert.IsFalse(valid);
    }
}