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
using Litics.Entities.Enum;
using System.Web.Http.Cors;
using Litics.BusinessLogic.Responses;

namespace Litics.Controller.Controllers
{
    [Authorize]
    [EnableCors("*", "*", "*")]
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

        protected ApplicationUserManager UserManager
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

        protected ApplicationRoleManager RoleManager
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
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public UserInfoViewModel GetUserInfo()
        {

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
            };
        }
        [Authorize(Roles = "Administrator,PowerUser,Guest")]
        [HttpGet]
        [Route("GetUsers")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> GetUsers()
        {
            var users = await UserManager.GetUsersAsync(User.Identity.GetUserId());
            var result = users.Select(s => new UserResponse
            {
                UserId = s.Id,
                AccountName = s.Account.Name,
                AccountId = s.Account.Id,
                UserName = s.UserName,
                Email = s.Email,
                Roles = s.Roles.Select(r => new { RoleManager.FindById(r.RoleId).Name }.Name),
                Locked = s.LockoutEnabled
            });
            return Ok(result);
        }
        [Authorize(Roles = "Administrator,PowerUser,Guest")]
        [HttpGet]
        [Route("GetUser")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            var user = await UserManager.GetUserAsync(User.Identity.GetUserId(), id);
            var result = new UserResponse
            {
                UserId = user.Id,
                AccountName = user.Account.Name,
                AccountId = user.Account.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = user.Roles.Select(r => new { RoleManager.FindById(r.RoleId).Name }.Name),
                Locked = user.LockoutEnabled

            };
            return Ok(result);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("GetApp")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> GetApp()
        {
            var account = await UserManager.FindAccountByUserIdAsync(User.Identity.GetUserId());
            var app = await UserManager.GetApp(account.Id);
            if (app != null)
            {
                return Ok(app);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("CreateApp")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
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
        [Route("ModifyApp")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> ModifyApp()
        {
            var account = await UserManager.FindAccountByUserIdAsync(User.Identity.GetUserId());
            var app = await UserManager.ModifyApp(account.Id);
            if (app != null)
            {
                return Ok(app);
            }
            else
            {
                return Ok($"Application is not exists!");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        [Route("DeleteApp")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> DeleteApp()
        {
            var account = await UserManager.FindAccountByUserIdAsync(User.Identity.GetUserId());
            var isSucceed = await UserManager.DeleteApp(account.Id);
            return isSucceed ? Ok("Application deletion was succesfull!") : Ok("Application deletion failed");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("CreateEsIndex")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
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
        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        [Route("DeleteEsIndex")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> DeleteEsIndex()
        {
            var account = await UserManager.FindAccountByUserIdAsync(User.Identity.GetUserId());
            var isSucceed = await UserManager.DeleteEsIndex(account.Id);
            return isSucceed ? Ok("ES Index Delete was successfull!") : Ok("ES Index failed!");
        }
        [Authorize(Roles = "Administrator,PowerUser,Guest")]
        [HttpGet]
        [Route("GetRoles")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public IHttpActionResult GetRoles()
        {
            var roles = Enum.GetNames(typeof(RolesType));
            return Ok(roles);
        }
        [Authorize(Roles = "Administrator, PowerUser")]
        [HttpPost]
        [Route("AddUserToRole")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> AddUserToRole(UserToRoleBindingModel user)
        {

            var addRoleResult = await UserManager.AddToRoleAsync(user.UserId, user.Role.ToString());

            if (!addRoleResult.Succeeded)
            {
                return GetErrorResult(addRoleResult);
            }
            return Ok("Role successfully added!");
        }
        [Authorize(Roles = "Administrator, PowerUser")]
        [HttpPost]
        [Route("DeleteUserFromRole")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> DeleteUserFromRole(UserToRoleBindingModel user)
        {
            try
            {
                var removeFromRoleResult = await UserManager.RemoveFromRoleAsync(user.UserId, user.Role.ToString());
                if (!removeFromRoleResult.Succeeded)
                {
                    return GetErrorResult(removeFromRoleResult);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("LockUser")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> LockUser(string userId)
        {
            var lockResult = await UserManager.LockUserAccount(userId, null);
            if (!lockResult.Succeeded)
            {
                return GetErrorResult(lockResult);
            }
            return Ok($"User with {userId} locked out!");
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("UnlockUser")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> UnlockUser(string userId)
        {
            var lockResult = await UserManager.UnlockUserAccount(userId);
            if (!lockResult.Succeeded)
            {
                return GetErrorResult(lockResult);
            }
            return Ok($"User with {userId} unlocked!");
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("DeleteUser")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> DeleteUser(string userId)
        {
            try
            {
                var user = await UserManager.FindByIdAsync(userId);
                var rolesForUser = await UserManager.GetRolesAsync(userId);
                var rolesRemove = await UserManager.RemoveUserFromRolesAsync(userId, rolesForUser);
                if (!rolesRemove.Succeeded)
                {
                    return BadRequest($"Roles remove succeded: {rolesRemove.Succeeded}");
                }
                var userRemove = await UserManager.DeleteAsync(user);
                if (!userRemove.Succeeded)
                {
                    return BadRequest($"User remove succeded: {userRemove.Succeeded}");
                }
                return Ok($"User successfully deleted!");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

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

