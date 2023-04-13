using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EmailAddress = SendGrid.Helpers.Mail.EmailAddress;
using Response = SendGrid.Response;

namespace Transversals.Business.Authentication.Application.Services;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Transversals.Business.Authentication.Application.Services.IEmailService" />
[ExcludeFromCodeCoverage]
public class EmailService : IEmailService
{
    private readonly EmailAddress _from;

    private readonly EmailAddressAttribute _emailAddressValidator;
    private readonly ISendGridClient _sendGridClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sendGridClient">The send grid client.</param>
    public EmailService(IConfiguration configuration, ISendGridClient sendGridClient)
    {
        _from = new EmailAddress(configuration["SendGrid:From:Address"], configuration["SendGrid:From:Name"]);
        _emailAddressValidator = new EmailAddressAttribute();
        _sendGridClient = sendGridClient;
    }

    /// <summary>
    /// Determines whether [is valid email address] [the specified address].
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>
    ///   <c>true</c> if [is valid email address] [the specified address]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidEmailAddress(string address)
    {
        return _emailAddressValidator.IsValid(address);
    }

    /// <summary>
    /// Sends the asynchronous.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <param name="content">The content.</param>
    /// <param name="tos">The tos.</param>
    /// <returns></returns>
    public async Task<bool> SendAsync(string subject, string content, params string[] tos)
    {
        SendGridMessage message = new();

        message.SetFrom(_from);
        message.AddTos(tos.Select(to => new EmailAddress(to)).ToList());
        message.SetSubject(subject);
        message.AddContent(MimeType.Html, content);
        Response response = await _sendGridClient.SendEmailAsync(message);

        return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted;
    }
}
