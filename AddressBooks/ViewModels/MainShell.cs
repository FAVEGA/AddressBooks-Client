using System;
using System.Threading;
using AddressBooks.Api;
using Stylet;

namespace AddressBooks.ViewModels
{
    class MainShell : Conductor<IScreen>.Collection.OneActive
    {

        private readonly IAddressBooksApi _addressBooksApi;
        private readonly IUpdatable _updatable;

        public MainShell(IAddressBooksApi addressBooksApi, IUpdatable updatable, AddressesPage addressesViewModel, GroupsPage groupsViewModel,
            AddressBooksPage addressBooksViewModel)
        {
            _updatable = updatable;
            _addressBooksApi = addressBooksApi;
            NotifyDataSetCanUpdate(null);
            Items.Add(addressesViewModel);
            Items.Add(groupsViewModel);
            Items.Add(addressBooksViewModel);

            ActiveItem = addressesViewModel;
        }


        Timer _dataSetUpdateTimer;

        protected override void OnDeactivate()
        {
            _dataSetUpdateTimer = new Timer(NotifyDataSetCanUpdate, null, TimeSpan.Zero, TimeSpan.FromMinutes(0.5));
        }

        protected override void OnActivate()
        {
            if (_dataSetUpdateTimer != null)
            {
                _dataSetUpdateTimer.Dispose();
            }
        }

        public void NotifyDataSetCanUpdate(object state)
        {
            _updatable.NotifyCanUpdate();
        }
    }
}
