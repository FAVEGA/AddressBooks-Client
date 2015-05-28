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
            builder.Bind<CachedAddressBooksApi>().To<CachedAddressBooksApi>().InSingletonScope();
            builder.Bind<IAuthenticatedAddressBooksApi>().ToFactory(container => container.Get<CachedAddressBooksApi>());
            builder.Bind<IAddressBooksApi>().ToFactory(container => container.Get<CachedAddressBooksApi>());
            builder.Bind<IRegistrar>().ToFactory(container => container.Get<CachedAddressBooksApi>());
            builder.Bind<LoginShell>().To<LoginShell>();
            builder.Bind<MainShell>().To<MainShell>();
            builder.Bind<AddressesPage>().To<AddressesPage>();
            builder.Bind<GroupsPage>().To<GroupsPage>();
            builder.Bind<AddressBooksPage>().To<AddressBooksPage>();
        }

        protected override void Launch()
        {
            var windowManager = (IWindowManager)this.GetInstance(typeof(IWindowManager));
            var loginShell = this.Container.Get<LoginShell>();
            loginShell.Closed += (o, e) => windowManager.ShowWindow(this.Container.Get<MainShell>()); 
            windowManager.ShowWindow(loginShell);
        }
    }
}
