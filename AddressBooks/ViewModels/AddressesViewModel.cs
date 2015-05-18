using AddressBooks.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBooks.ViewModels
{
    class AddressesViewModel : Screen
    {
        List<AddressBook> AddressBooks = new List<AddressBook>();

        public AddressesViewModel()
        {
            this.DisplayName = "Direcciones";
        }
    }
}
