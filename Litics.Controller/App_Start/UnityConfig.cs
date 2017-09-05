using Litics.BusinessLogic;
using Litics.BusinessLogic.Interfaces;
using Litics.Controller.Controllers;
using Litics.Controller.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using System;
using System.Data.Entity;
using System.Web.Http;
using Unity.WebApi;

namespace Litics.Controller
{
    public static class UnityConfig
    {
        // #region Unity Container
        /* private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
         {
             var container = new UnityContainer();
             RegisterTypes(container);
             return container;
         });

         /// <summary>
         /// Gets the configured Unity container.
         /// </summary>
         public static IUnityContainer GetConfiguredContainer()
         {
             return container.Value;
         }
         #endregion

         public static void RegisterTypes(IUnityContainer container)
         {
             container.RegisterType<IConfiguration, Configuration>(new ContainerControlledLifetimeManager());
             container.RegisterType<IElasticsearchRepository, ElasticsearchRepository>(new ContainerControlledLifetimeManager());
         }*/
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IConfiguration, Configuration>(new ContainerControlledLifetimeManager());
            container.RegisterType<IElasticsearchRepository, ElasticsearchRepository>(new ContainerControlledLifetimeManager());

            container.RegisterType<DbContext, ApplicationDbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<UserManager<ApplicationUser>>(
                new HierarchicalLifetimeManager());
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(
                new HierarchicalLifetimeManager());

            container.RegisterType<AccountController>(
                new InjectionConstructor());
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}