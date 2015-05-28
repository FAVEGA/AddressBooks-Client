using AddressBooks.Api;
using Stylet;
using StyletIoC;

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
            builder.Bind<IUpdatable>().ToFactory(container => container.Get<CachedAddressBooksApi>());
            builder.Bind<LoginShell>().To<LoginShell>();
            builder.Bind<MainShell>().To<MainShell>();
            builder.Bind<AddressesPage>().To<AddressesPage>();
            builder.Bind<GroupsPage>().To<GroupsPage>();
            builder.Bind<AddressBooksPage>().To<AddressBooksPage>();
        }

        protected override void Launch()
        {
            var windowManager = (IWindowManager)GetInstance(typeof(IWindowManager));
            var loginShell = Container.Get<LoginShell>();
            loginShell.Closed += (o, e) => windowManager.ShowWindow(Container.Get<MainShell>()); 
            windowManager.ShowWindow(loginShell);
        }
    }
}
