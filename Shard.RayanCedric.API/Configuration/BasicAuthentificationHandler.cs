using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Shard.RayanCedric.API.Configuration;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly AdminCredentials _adminCredentials;
    private readonly Wormholes _wormholes;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
        IOptions<AdminCredentials> adminCredentialsOptions, IOptions<Wormholes> wormholes)
        : base(options, logger, encoder)
    {
        _adminCredentials = adminCredentialsOptions.Value;
        _wormholes = wormholes.Value;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!TryGetAuthorizationHeader(out var authHeader))
            return AuthenticateResult.Fail("Invalid Authorization Header");

        var credentials = ParseCredentials(authHeader.Parameter);
        if (credentials == null)
            return AuthenticateResult.Fail("Invalid Basic Authentication Credentials");

        var (username, password) = credentials.Value;

        if (IsValidAdminUser(username, password))
            return AuthenticateSuccess(username, "administrator");

        if (IsValidShardUser(username, password))
            return AuthenticateSuccess(username, "shard");

        return AuthenticateResult.Fail("Invalid username or password");
    }

    private bool TryGetAuthorizationHeader(out AuthenticationHeaderValue? authHeader)
    {
        var authHeaderValue = Request.Headers.Authorization;
        authHeader = null;

        try
        {
            authHeader = AuthenticationHeaderValue.Parse(authHeaderValue);
            return authHeader.Scheme == "Basic";
        }
        catch
        {
            return false;
        }
    }

    private (string username, string password)? ParseCredentials(string authParameter)
    {
        var credentialBytes = Convert.FromBase64String(authParameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

        return credentials.Length == 2 ? 
            (credentials[0], credentials[1]) :
            null;
    }

    private bool IsValidAdminUser(string username, string password)
    {
        return username == _adminCredentials.Username && password == _adminCredentials.Password;
    }

    private bool IsValidShardUser(string username, string password)
    {
        if (!username.StartsWith("shard-")) return false;

        var serverName = username[6..];
        try
        {
            var server = _wormholes[serverName];
            return server.SharedPassword == password;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    private AuthenticateResult AuthenticateSuccess(string username, string role)
    {
        var identity = new GenericIdentity(username, Scheme.Name);
        var principal = new GenericPrincipal(identity, [role]);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}