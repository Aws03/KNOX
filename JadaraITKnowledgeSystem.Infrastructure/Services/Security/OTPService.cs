using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Users.ValueObjects;
using JadaraITKnowledgeSystem.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Security;

public class OTPService : IOTPService
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    private const int OTP_EXPIRY_MINUTES = 10;
    private const int RESEND_INTERVAL_MINUTES = 3;

    public OTPService(IApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<string> GenerateOTPAsync(int userId)
    {
        var random = new Random();
        var otp = random.Next(100000, 999999).ToString();

        var oldOtps = await _context.VerificationOTPs
            .Where(o => o.UserId == userId && !o.IsUsed)
            .ToListAsync();

        foreach (var oldOtp in oldOtps)
        {
            oldOtp.MarkUsed();
        }

        var otpRecord = VerificationOTP.Create(userId, otp, DateTime.UtcNow.AddMinutes(OTP_EXPIRY_MINUTES));

        await _context.VerificationOTPs.AddAsync(otpRecord);
        await _context.SaveChangesAsync();

        return otp;
    }

    public async Task<Result<int>> ValidateOtpAsync(string email, string otp)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
            return Error.Validation("OTP.InvalidInput", "Email and OTP are required");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Address == email);
        if (user == null)
            return Error.NotFound("User.NotFound", "User not found");

        var otpRecord = await _context.VerificationOTPs
            .Where(o => o.UserId == user.Id
                     && o.OTP == otp
                     && !o.IsUsed
                     && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpRecord == null)
            return Error.Validation("OTP.Invalid", "Invalid or expired OTP");

        otpRecord.MarkUsed();
        await _context.SaveChangesAsync();

        return user.Id;
    }

    public async Task<bool> SendOtpAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Address == email);
        if (user == null)
            return false;

        var lastOtp = await _context.VerificationOTPs
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (lastOtp != null && lastOtp.CreatedAt > DateTime.UtcNow.AddMinutes(-RESEND_INTERVAL_MINUTES))
        {
            return false;
        }

        var otp = await GenerateOTPAsync(user.Id);

        var emailRequest = BuildEmailRequest(user.Email.Address, user.Name.Value, otp);
        var sent = await _emailService.SendEmailAsync(emailRequest);
        return sent;
    }

    public async Task<bool> CheckOTPValidityByEmailAsync(string email, string otp)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
            return false;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Address.ToLower() == email.Trim().ToLower());
        if (user == null)
            return false;

        var otpRecord = await _context.VerificationOTPs
            .Where(o => o.UserId == user.Id
                     && o.OTP == otp
                     && !o.IsUsed
                     && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        return otpRecord != null;
    }

    private EmailRequest BuildEmailRequest(string toEmail, string userName, string otpNumber)
    {
        var currentDate = DateTime.UtcNow.ToString("dd MMM, yyyy");

        var textBody = $"Your OTP is: {otpNumber}\n" +
                       "It will expire in 10 minutes.\n\n" +
                       "Please copy the code and use it to continue.";

        var htmlBody = $@"
        <!DOCTYPE html>
        <html lang='en'>
          <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>OTP Verification</title>
            <link href='https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap' rel='stylesheet'>
          </head>
          <body style='margin:0; font-family:Poppins, sans-serif; background:#ffffff; font-size:14px;'>
            <div style='max-width:600px; margin:0 auto; padding:20px;
                background:#f4f7ff; background-image:url(https://archisketch-resource.s3.ap-northeast-2.amazonaws.com/vrstyler/1661497957196_595865/email-template-background-banner);
                background-repeat:no-repeat; background-size:cover; background-position:top center;
                color:#434343;'>

              <header>
                <table style='width:100%;'>
                  <tr>
                    <td>
                      <img alt='eCampus' src='https://ecampus.b-cdn.net/37d82a0a-31f3-49fb-a20f-b2c53fed89af.png' height='36px' style='max-width:150px;'/>
                    </td>
                    <td style='text-align:right;'>
                      <span style='font-size:14px; line-height:30px; color:#ffffff;'>{currentDate}</span>
                    </td>
                  </tr>
                </table>
              </header>

              <main>
                <div style='margin-top:40px; padding:40px 20px; background:#ffffff;
                    border-radius:20px; text-align:center;'>
                  <div style='max-width:480px; margin:0 auto;'>
                    <h1 style='margin:0; font-size:20px; font-weight:600; color:#061845;'>Your OTP Code</h1>
                    <p style='margin:15px 0 0; font-size:15px; font-weight:500;'>Hello {userName},</p>
                    <p style='margin:15px 0 0; font-size:14px; line-height:22px;'>
                      Thank you for using <b>eCampus</b>. Use the OTP below to complete your request.<br/>
                      The OTP is valid for <span style='font-weight:600; color:#ff9625;'>10 minutes</span>. 
                      Do not share this code with anyone.
                    </p>
                    <p style='margin:25px 0 0; font-size:26px; font-weight:600; letter-spacing:12px; color:#ff9625;'>
                      {otpNumber}
                    </p>
                  </div>
                </div>

                <p style='max-width:360px; margin:40px auto 0; text-align:center;
                    font-size:13px; font-weight:500; color:#8c8c8c;'>
                  Need help? Contact us at 
                  <a href='mailto:support@ecampus.com' style='color:#061845; text-decoration:none;'>support@ecampus.com</a>
                  or visit our <a href='#' style='color:#061845; text-decoration:none;'>Help Center</a>.
                </p>
              </main>

              <footer style='width:100%; max-width:480px; margin:20px auto 0; text-align:center; border-top:1px solid #e6ebf1; padding-top:20px;'>
                <p style='margin:0; font-size:15px; font-weight:600; color:#434343;'>eCampus</p>
                <p style='margin:8px 0 0; color:#434343; font-size:13px;'>Learning Without Limits</p>
                <div style='margin-top:16px;'>
                  <a href='https://web.facebook.com/profile.php?id=61575006386806' target='_blank'><img width='32px' alt='Facebook' src='https://ecampus.b-cdn.net/facebook.png'/></a>
                  <a href='https://www.instagram.com/ecampusjo/' target='_blank' style='margin-left:8px;'><img width='32px' alt='Instagram' src='https://ecampus.b-cdn.net/instagram.png'/></a>
                  <a href='https://www.youtube.com/@ecampusacademy_jo' target='_blank' style='margin-left:8px;'><img width='32px' alt='Youtube' src='https://ecampus.b-cdn.net/youtube.png'/></a>
                </div>
                <p style='margin:16px 0 0; font-size:12px; color:#434343;'>© {DateTime.UtcNow.Year} eCampus. All rights reserved.</p>
              </footer>
            </div>
          </body>
        </html>";

        return new EmailRequest
        {
            from = _emailService.GetDefaultFrom(),
            recipients = new List<EmailRecipient> 
            { 
                new EmailRecipient { email = toEmail, name = userName } 
            },
            subject = "Your eCampus OTP Code",
            text_content = textBody,
            html_content = htmlBody
        };
    }
}