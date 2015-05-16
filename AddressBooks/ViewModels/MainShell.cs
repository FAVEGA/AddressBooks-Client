using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;
using System.Threading;

namespace AddressBooks.ViewModels
{
    class MainShell
    {

        Timer DataSetUpdateTimer;

        public async void Deactivated()
        {
            DataSetUpdateTimer = new Timer(NotifyDataSetCanUpdate, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            Console.WriteLine("Deactivated, started timer...");
        }

        public async void Activated()
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
            AddressBooksApi.NotifyCanUpdate();
        }
    }
}
