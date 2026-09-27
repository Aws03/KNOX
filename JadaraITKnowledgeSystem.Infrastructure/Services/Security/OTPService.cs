using System.Security.Cryptography;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using EmailAddress = JadaraITKnowledgeSystem.Domain.Users.ValueObjects.Email;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Security;

public sealed class OTPService(IApplicationDbContext context, IEmailService emailService, TimeProvider timeProvider) : IOTPService
{
    private const int OTP_EXPIRY_MINUTES = 10;
    private const int RESEND_INTERVAL_MINUTES = 3;

    private static Error InvalidOtp => Error.Validation("OTP.Invalid", "Invalid or expired OTP");

    public async Task<bool> SendOtpAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email.Address == EmailAddress.Normalize(email), cancellationToken);
        if (user == null)
            return false;

        var now = timeProvider.GetUtcNow();
        var lastSentAt = await context.VerificationOTPs
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.Id)
            .Select(o => (DateTimeOffset?)o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastSentAt > now.AddMinutes(-RESEND_INTERVAL_MINUTES))
            return false;

        // Only the newest code is ever valid.
        var previous = await context.VerificationOTPs
            .Where(o => o.UserId == user.Id && !o.IsUsed)
            .ToListAsync(cancellationToken);
        foreach (var code in previous)
            code.MarkUsed();

        // OTPs gate account verification and password resets, so they must come from a CSPRNG.
        var otp = RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString();
        context.VerificationOTPs.Add(VerificationOTP.Create(user.Id, otp, now.UtcDateTime.AddMinutes(OTP_EXPIRY_MINUTES)));
        await context.SaveChangesAsync(cancellationToken);

        await emailService.SendAsync(BuildEmailMessage(user.Email.Address, user.Name.Value, otp), cancellationToken);
        return true;
    }

    public async Task<Result<int>> ValidateOtpAsync(string email, string otp, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
            return Error.Validation("OTP.InvalidInput", "Email and OTP are required");

        var userId = await context.Users
            .Where(u => u.Email.Address == EmailAddress.Normalize(email))
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        // Same answer for unknown accounts as for wrong codes: no account enumeration.
        if (userId is null)
            return InvalidOtp;

        var latest = await context.VerificationOTPs
            .Where(o => o.UserId == userId && !o.IsUsed)
            .OrderByDescending(o => o.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest is null)
            return InvalidOtp;

        var verified = latest.TryVerify(otp.Trim(), timeProvider.GetUtcNow().UtcDateTime);
        await context.SaveChangesAsync(cancellationToken);

        return verified ? userId.Value : InvalidOtp;
    }

    private EmailMessage BuildEmailMessage(string toEmail, string userName, string otpNumber)
    {
        var today = timeProvider.GetUtcNow();
        var name = System.Net.WebUtility.HtmlEncode(userName);

        var textBody = $"Hello {userName},\n\n" +
                       $"Your KNOX verification code is: {otpNumber}\n" +
                       $"It expires in {OTP_EXPIRY_MINUTES} minutes. Do not share it with anyone.\n\n" +
                       "If you did not request this code, you can ignore this email.";

        var htmlBody = $@"
        <!DOCTYPE html>
        <html lang='en'>
          <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>KNOX verification code</title>
          </head>
          <body style='margin:0; background:#f4f7ff; font-family:Segoe UI, Helvetica, Arial, sans-serif; font-size:14px; color:#434343;'>
            <div style='max-width:600px; margin:0 auto; padding:24px;'>
              <table style='width:100%;'>
                <tr>
                  <td style='font-size:22px; font-weight:700; color:#061845;'>KNOX</td>
                  <td style='text-align:right; color:#8c8c8c;'>{today:dd MMM yyyy}</td>
                </tr>
              </table>

              <div style='margin-top:24px; padding:40px 20px; background:#ffffff; border-radius:16px; text-align:center;'>
                <h1 style='margin:0; font-size:20px; font-weight:600; color:#061845;'>Your verification code</h1>
                <p style='margin:16px 0 0; font-size:15px;'>Hello {name},</p>
                <p style='margin:12px 0 0; line-height:22px;'>
                  Use the code below to continue. It expires in
                  <b>{OTP_EXPIRY_MINUTES} minutes</b>. Do not share it with anyone.
                </p>
                <p style='margin:24px 0 0; font-size:28px; font-weight:700; letter-spacing:12px; color:#ff9625;'>{otpNumber}</p>
              </div>

              <p style='margin:24px 0 0; text-align:center; font-size:12px; color:#8c8c8c;'>
                If you did not request this code, you can ignore this email.<br/>
                © {today:yyyy} KNOX
              </p>
            </div>
          </body>
        </html>";

        return new EmailMessage(toEmail, userName, "Your KNOX verification code", htmlBody, textBody);
    }
}
