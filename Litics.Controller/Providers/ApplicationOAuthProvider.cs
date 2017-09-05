using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.Data.Entity;
using Litics.Controller.Models;
using Litics.Controller.BusinessLogic.Helper;

namespace Litics.Controller.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            if (context.ClientId == null)
            {
                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                var accountName = context.OwinContext.Get<string>("accountname");
                ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password, accountName);

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                   OAuthDefaults.AuthenticationType);
               // ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                 //   CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = CreateProperties(user.UserName, user.Account.Name);
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(oAuthIdentity);
            }
            else
            {
                var dbContext = context.OwinContext.Get<ApplicationDbContext>();
                var client = await dbContext.Account.FirstOrDefaultAsync(account => account.Id == context.ClientId);
                context.Options.AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(20);
                var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, client.Name));
                var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
                context.Validated(ticket);
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }


        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;

            if (!context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                string accountName = context.Parameters.Where(f => f.Key == "accountname").Select(f => f.Value).SingleOrDefault()[0];
                context.OwinContext.Set<string>("accountname", accountName);
                context.Validated();
                return;
            }

            ApplicationDbContext dbContext = context.OwinContext.Get<ApplicationDbContext>();
            ApplicationUserManager userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            if (dbContext == null)
            {
                context.SetError("server_error");
                context.Rejected();
                return;
            }

            try
            {
                var client = await dbContext.Account.FirstOrDefaultAsync(account => account.Id == clientId);

                if (client != null)
                {
                    // Client has been verified.
                    client.Id = clientId;
                  //  client.AllowedGrant = OAuthGrant.ResourceOwner;
                   // client.CreatedOn = DateTimeOffset.UtcNow;
                    context.OwinContext.Set<Account>("oauth:client", client);
                    context.Validated(clientId);
                }
                else
                {
                    // Client could not be validated.
                    context.SetError("invalid_client", "Client credentials are invalid.");
                    context.Rejected();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                context.SetError("server_error");
                context.Rejected();
            }
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, string accountName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName },
                { "accountname", accountName }
            };
            return new AuthenticationProperties(data);
        }

    }
}