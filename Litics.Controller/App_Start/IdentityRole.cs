using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Litics.Controller.Models;
using Litics.Entities.Enum;
using System;

namespace Litics
{
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {

        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            manager.Create(new IdentityRole(RolesType.Administrator.ToString()));
            manager.Create(new IdentityRole(RolesType.PowerUser.ToString()));
            manager.Create(new IdentityRole(RolesType.User.ToString()));
            manager.Create(new IdentityRole(RolesType.Guest.ToString()));
            return manager;
        }
    }
}