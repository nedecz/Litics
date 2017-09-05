using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Litics.Controller.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Data.Entity;
using System.Security.Cryptography;
using Litics.Entities;
using NLog;

namespace Litics.Controller
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public async Task<ApplicationUser> FindAsync(string userName, string password, string accountName)
        {
            ApplicationUser selectedUser = null;
            using (var context = new ApplicationDbContext())
            {
                selectedUser = await context.Users.Include(p => p.Account).Where(user => user.Account.Name == accountName).
                    SingleOrDefaultAsync(user => user.UserName == userName);

                if (PasswordHasher.VerifyHashedPassword(selectedUser.PasswordHash, password) == PasswordVerificationResult.Success)
                {
                    return selectedUser;
                }
            }
            return selectedUser;
        }

        public async Task<Account> FindAccountAsync(string accountName)
        {
            Account account = null;
            using (var context = new ApplicationDbContext())
            {
                account = await context.Account.SingleOrDefaultAsync(acc => acc.Name == accountName);
                return account;
            }
        }

        public async Task<Account> FindAccountByUserIdAsync(string userId)
        {
            using (var context = new ApplicationDbContext())
            {
                var userAccId = await context.Users.SingleOrDefaultAsync(user => user.Id == userId);

                var account = await context.Account.SingleOrDefaultAsync(acc => acc.Id == userAccId.AccountId);
                return account;
            }
        }
        public async Task<AppInformation> CreateApp(string accountId)
        {
            try
            {
                Logger.Debug($"CreateApp... AccountId: {accountId}");
                using (var context = new ApplicationDbContext())
                {
                    var account = await context.Account.SingleOrDefaultAsync(acc => acc.Id == accountId);
                    if (account.AppId == null && account.ApiKey == null)
                    {
                        account.AppId = Guid.NewGuid().ToString();
                        account.ApiKey = GenerateAppKey();
                        await context.SaveChangesAsync();
                        var app = new AppInformation()
                        {
                            ApiKey = account.ApiKey,
                            AppId = account.AppId
                        };
                        return app;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateApp Error! AccountId: {accountId}, Msg: {ex.ToString()}");
                throw;
            }
        }
        public async Task<AppInformation> ModifyApp(string accountId)
        {
            try
            {
                Logger.Debug($"ModifyApp... AccountId: {accountId}");
                using (var context = new ApplicationDbContext())
                {
                    var account = await context.Account.SingleOrDefaultAsync(acc => acc.Id == accountId);
                    if (account.AppId != null && account.ApiKey != null)
                    {
                        account.AppId = Guid.NewGuid().ToString();
                        account.ApiKey = GenerateAppKey();
                        await context.SaveChangesAsync();
                        var app = new AppInformation()
                        {
                            ApiKey = account.ApiKey,
                            AppId = account.AppId
                        };
                        return app;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"ModifyApp Error! AccountId: {accountId}, Msg: {ex.ToString()}");
                throw;
            }
        }
        public async Task<bool> DeleteApp(string accountId)
        {
            try
            {
                Logger.Debug($"DeleteApp... AccountId: {accountId}");
                using (var context = new ApplicationDbContext())
                {
                    var account = await context.Account.SingleOrDefaultAsync(acc => acc.Id == accountId);
                    if (account.AppId != null && account.ApiKey != null)
                    {
                        account.AppId = null;
                        account.ApiKey = null;
                        await context.SaveChangesAsync();
                        var app = new AppInformation()
                        {
                            ApiKey = account.ApiKey,
                            AppId = account.AppId
                        };
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"DeleteApp Error! AccountId: {accountId}, Msg: {ex.ToString()}");
                throw;
            }
        }
        public async Task<string> CreateEsIndex(string accountId)
        {
            try
            {
                Logger.Debug($"CreateEsIndex... AccountId: {accountId}");
                using (var context = new ApplicationDbContext())
                {
                    var account = await context.Account.SingleOrDefaultAsync(acc => acc.Id == accountId);
                    if (account.AppId != null && account.ApiKey != null)
                    {
                        account.EsIndex = Guid.NewGuid().ToString();
                        await context.SaveChangesAsync();
                        return account.EsIndex;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateEsIndex Error! AccountId: {accountId}, Msg: {ex.ToString()}");
                throw;
            }
        }

        public async Task<bool> DeleteEsIndex(string accountId)
        {
            try
            {
                Logger.Debug($"DeleteEsIndex... AccountId: {accountId}");
                using (var context = new ApplicationDbContext())
                {
                    var account = await context.Account.SingleOrDefaultAsync(acc => acc.Id == accountId);
                    if (account.AppId != null && account.ApiKey != null)
                    {
                        account.EsIndex = null;
                        await context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"DeleteEsIndex Error! AccountId: {accountId}, Msg: {ex.ToString()}");
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(string userId)
        {
            IEnumerable<ApplicationUser> selectedUsers = null;
            using (var context = new ApplicationDbContext())
            {
                var selectedUser = await context.Users.Include(p => p.Account).Include(i => i.Roles).FirstOrDefaultAsync(user => user.Id == userId);
                selectedUsers = await context.Users.Include(p => p.Account).Include(i => i.Roles).Where(acc => acc.AccountId == selectedUser.AccountId).ToListAsync();
                return selectedUsers;
            }
        }

        public virtual async Task<IdentityResult> AddUserToRolesAsync(
       string userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<ApplicationUser, string>)Store;
            var user = await FindByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore
                .GetRolesAsync(user)
                .ConfigureAwait(false);

            // Add user to each role using UserRoleStore
            foreach (var role in roles.Where(role => !userRoles.Contains(role)))
            {
                await userRoleStore.AddToRoleAsync(user, role).ConfigureAwait(false);
            }

            // Call update once when all roles are added
            return await UpdateAsync(user).ConfigureAwait(false);
        }


        public virtual async Task<IdentityResult> RemoveUserFromRolesAsync(
            string userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<ApplicationUser, string>)Store;
            var user = await FindByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore
                .GetRolesAsync(user)
                .ConfigureAwait(false);

            // Remove user to each role using UserRoleStore
            foreach (var role in roles.Where(userRoles.Contains))
            {
                await userRoleStore
                    .RemoveFromRoleAsync(user, role)
                    .ConfigureAwait(false);
            }

            // Call update once when all roles are removed
            return await UpdateAsync(user).ConfigureAwait(false);
        }

        private string GenerateAppKey()
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyByteArray = new byte[32]; //256 bit
                cryptoProvider.GetBytes(secretKeyByteArray);
                var apiKey = Convert.ToBase64String(secretKeyByteArray);
                return apiKey;
            }
        }
    }
}