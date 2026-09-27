using JadaraITKnowledgeSystem.Domain.Identity;
using JadaraITKnowledgeSystem.Domain.Users.Entities;

namespace JadaraITKnowledgeSystem.UnitTests.Domain;

public class SecurityEntityTests
{
    private static readonly DateTime Now = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Otp_CorrectCode_IsConsumed()
    {
        var otp = VerificationOTP.Create(1, "123456", Now.AddMinutes(10));

        Assert.True(otp.TryVerify("123456", Now));
        Assert.False(otp.TryVerify("123456", Now)); // single use
    }

    [Fact]
    public void Otp_IsBurnedAfterFiveWrongGuesses()
    {
        var otp = VerificationOTP.Create(1, "123456", Now.AddMinutes(10));

        for (var i = 0; i < VerificationOTP.MaxFailedAttempts; i++)
            Assert.False(otp.TryVerify("000000", Now));

        Assert.True(otp.IsUsed);
        Assert.False(otp.TryVerify("123456", Now));
    }

    [Fact]
    public void Otp_Expired_IsRejected()
    {
        var otp = VerificationOTP.Create(1, "123456", Now.AddMinutes(10));

        Assert.False(otp.TryVerify("123456", Now.AddMinutes(11)));
    }

    [Fact]
    public void RefreshToken_IsActiveUntilRevokedOrExpired()
    {
        var token = RefreshToken.Create(1, "hash", Now, Now.AddDays(7), "ip");

        Assert.True(token.IsActive(Now));
        Assert.False(token.IsActive(Now.AddDays(8)));

        token.Revoke("ip", Now);
        Assert.False(token.IsActive(Now));
        Assert.Equal(Now, token.RevokedAt);
    }
}
