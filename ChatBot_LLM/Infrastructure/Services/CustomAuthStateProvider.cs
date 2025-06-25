using ChatBot_LLM.Domain.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace ChatBot_LLM.Infrastructure.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly UserSessionService _userSession;

        public CustomAuthStateProvider(UserSessionService userSession)
        {
            _userSession = userSession;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity = new ClaimsIdentity();

            if (_userSession.IsAuthenticated)
            {
                identity = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, _userSession.Username),
                new Claim(ClaimTypes.Role, _userSession.Role)
            }, "apiauth_type", ClaimTypes.Name, ClaimTypes.Role);
            }

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }

        public void NotifyUserAuthentication(string username, string role)
        {
            _userSession.SetUser(username, role);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void NotifyUserLogout()
        {
            _userSession.Clear();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

}
