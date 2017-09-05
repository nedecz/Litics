using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System.Web.Security;
using Litics.Controller.Models;
using Litics.Controller.BusinessLogic.Helper;
using System.Security.Claims;
using Microsoft.Owin.Security.OAuth;
using Litics.Entities.Enum;

namespace Litics.Controller.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationRoleManager roleManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
            };
        }
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IHttpActionResult> GetUsers()
        {
            var users = await UserManager.GetUsersAsync(User.Identity.GetUserId());
            return Ok(users.Select(s => new
            {
                s.Account.Name,
                s.UserName,
                s.Email,
                roles = s.Roles.Select(r => new { RoleManager.FindById(r.RoleId).Name })
            }));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("CreateApp")]
        public async Task<IHttpActionResult> CreateApp()
        {
            var account = await UserManager.FindAccountByUserIdAsync(User.Identity.GetUserId());
            var app = await UserManager.CreateApp(account.Id);
            if (app != null)
            {
                return Ok(app);
            }
            else
            {
                return Ok($"Application is already exists!");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("CreateEsIndex")]
        public async Task<IHttpActionResult> CreateEsIndex()
        {
            var account = await UserManager.FindAccountByUserIdAsync(User.Identity.GetUserId());
            var esIndex = await UserManager.CreateEsIndex(account.Id);
            if (esIndex != null)
            {
                return Ok(esIndex);
            }
            else
            {
                return Ok($"Application is not exists!");
            }
        }
            
            
            /*[HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("CreateDeviceToken")]
        public async Task<IHttpActionResult> CreateDeviceToken()
        {
            var loggedUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var accountId = loggedUser.AccountId;
            var accountPassword = loggedUser.Account.ClientSecretHash;

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
           
            identity.AddClaim(new Claim(ClaimTypes.Role, "Guest"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            
            var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);


            return Ok(accessToken);
        }
        */
        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbAccount = await UserManager.FindAccountAsync(model.AccountName);
            if (dbAccount == null)
            {
                var clientPasswd = Membership.GeneratePassword(16, 4);
                var account = new Account { Name = model.AccountName, Id = Guid.NewGuid().ToString() };
                var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email, Account = account, AccountId = account.Id };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                var savedUser = await UserManager.FindAsync(model.UserName, model.Password, model.AccountName);
                var addRoleResult = await UserManager.AddToRoleAsync(savedUser.Id, RolesType.Administrator.ToString());

                if (!addRoleResult.Succeeded)
                {
                    return GetErrorResult(addRoleResult);
                }
            }
            else
            {
                var clientPasswd = Membership.GeneratePassword(16, 4);
                var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email, AccountId = dbAccount.Id };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                var savedUser = await UserManager.FindAsync(model.UserName, model.Password, model.AccountName);
                var addRoleResult = await UserManager.AddToRoleAsync(savedUser.Id, RolesType.User.ToString());

                if (!addRoleResult.Succeeded)
                {
                    return GetErrorResult(addRoleResult);
                }
            }

            return Ok();
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }


        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}

