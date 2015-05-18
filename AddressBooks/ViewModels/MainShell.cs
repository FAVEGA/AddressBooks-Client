using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;
using System.Threading;
using AddressBooks.Models;
using AddressBooks.Api;

namespace AddressBooks.ViewModels
{
    class MainShell : Conductor<IScreen>.Collection.OneActive
    {

        public IObservableCollection<AddressBook> AddressBooks { get; private set; }
        private IAddressBooksApi addressBooksApi;

        public MainShell(IAddressBooksApi addressBooksApi)
        {
            this.addressBooksApi = addressBooksApi;
            this.NotifyDataSetCanUpdate(null);
            this.Items.Add(new AddressesViewModel());
            this.Items.Add(new GroupsViewModel());
            this.Items.Add(new AddressBooksViewModel());
            this.Items.Add(new UsersViewModel());
            this.Items.Add(new PermissionsViewModel());

            this.ActiveItem = this.Items.First();
        }


        Timer DataSetUpdateTimer;

        public void Deactivated()
        {
            DataSetUpdateTimer = new Timer(NotifyDataSetCanUpdate, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            Console.WriteLine("Deactivated, started timer...");
        }

        public void Activated()
        {
            Console.WriteLine("Activated, disposing timer...");
            if (DataSetUpdateTimer != null)
            {
                DataSetUpdateTimer.Dispose();
            }
        }

        public void NotifyDataSetCanUpdate(object state)
        {
            Console.WriteLine("Called NotifyDataSetCanUpdate");
            ((CachedAddressBooksApi) addressBooksApi).NotifyCanUpdate();
        }
    }
}
