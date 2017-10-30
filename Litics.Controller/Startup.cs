using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.Threading.Tasks;
using System.Web.Cors;

[assembly: OwinStartup(typeof(Litics.Controller.Startup))]

namespace Litics.Controller
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            const string rootFolder = "D:\\WORK\\Git\\Litics\\Litics.UI\\wwwroot";
            var fileSystem = new PhysicalFileSystem(rootFolder);
            var options = new FileServerOptions
            {
                RequestPath = PathString.Empty,
                EnableDefaultFiles = true,
                FileSystem = fileSystem
            };
            options.StaticFileOptions.FileSystem = fileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = false;
            app.UseFileServer(options);

            app.UseCors(CorsOptions.AllowAll);
            ConfigureAuth(app);
        }
    }
}
