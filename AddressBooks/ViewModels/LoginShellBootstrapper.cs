using AddressBooks.Api;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBooks.ViewModels
{
    class LoginShellBootstrapper : Bootstrapper<LoginShell>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind<IAddressBooksApi>().To<CachedAddressBooksApi>().InSingletonScope();
        }
    }
}
