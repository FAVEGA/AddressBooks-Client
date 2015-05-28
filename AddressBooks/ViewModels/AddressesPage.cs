using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AddressBooks.Api;
using AddressBooks.Models;
using Refit;
using Stylet;

namespace AddressBooks.ViewModels
{
    internal class AddressesPage : Screen, IRegisterable
    {
        private readonly IWindowManager _windowManager;
        private IAuthenticatedAddressBooksApi _api;

        private bool _canAddAddress;

        public bool CanAddAddress
        {
            get { return _canAddAddress; }
            set { SetAndNotify(ref _canAddAddress, value); }
        }

        private bool _canChangeAddress;

        public bool CanChangeAddress
        {
            get { return _canChangeAddress; }
            set { SetAndNotify(ref _canChangeAddress, value); }
        }

        private bool _canDeleteAddress;

        public bool CanDeleteAddress
        {
            get { return _canDeleteAddress; }
            set { SetAndNotify(ref _canDeleteAddress, value); }
        }

        private AddressBook _addressBook;

        public AddressBook AddressBook
        {
            get { return _addressBook; }
            set { SetAndNotify(ref _addressBook, value); }
        }

        private IObservableCollection<AddressBook> _addressBooks;

        public IObservableCollection<AddressBook> AddressBooks
        {
            get { return _addressBooks; }
            set { SetAndNotify(ref _addressBooks, value); }
        }

        private IObservableCollection<Group> _groups;

        public IObservableCollection<Group> Groups
        {
            get { return _groups; }
            set { SetAndNotify(ref _groups, value); }
        }

        private Group _group;

        public Group Group
        {
            get { return _group; }
            set { SetAndNotify(ref _group, value); }
        }

        private IObservableCollection<AddressViewModel> _addresses;

        public IObservableCollection<AddressViewModel> Addresses
        {
            get { return _addresses; }
            set { SetAndNotify(ref _addresses, value); }
        }

        private IObservableCollection<AddressViewModel> _selectedAddresses;

        public IObservableCollection<AddressViewModel> SelectedAddresses
        {
            get { return _selectedAddresses; }
            set { SetAndNotify(ref _selectedAddresses, value); }
        }

        private string _searchText;
        public string SearchText { 
            get { return _searchText; } 
            set { SetAndNotify(ref _searchText, value); }
        }

        public AddressesPage(IWindowManager windowManager, IRegistrar registrar, IAuthenticatedAddressBooksApi addressBooksApi)
        {
            registrar.Register(this);
            _api = addressBooksApi;
            DisplayName = "Direcciones";
            _windowManager = windowManager; 
            AddressBooks = new BindableCollection<AddressBook>();
            Groups = new BindableCollection<Group>();
            Addresses = new BindableCollection<AddressViewModel>();
            SelectedAddresses = new BindableCollection<AddressViewModel>();
        }

        public async void AddressesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            SelectedAddresses.Clear();
            foreach (var selectedItem in dataGrid.SelectedItems)
            {
                if (selectedItem is AddressViewModel)
                {
                    SelectedAddresses.Add(selectedItem as AddressViewModel);
                }
            }
        }

        public async void DeleteAddresses()
        {
            if (SelectedAddresses.Count > 0)
            {
                MessageBoxViewModel.ButtonLabels[MessageBoxResult.Yes] = "Del grupo";
                MessageBoxViewModel.ButtonLabels[MessageBoxResult.No] = "De la libreta";
                MessageBoxViewModel.ButtonLabels[MessageBoxResult.Cancel] = "Cancelar";
                var result = _windowManager.ShowMessageBox("De donde quiere eliminar las libretas seleccionadas?", "Eliminar direcciones", buttons: MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    var temp = SelectedAddresses.ToList();
                    if (Group != null)
                    {
                        foreach (var addressViewModel in temp)
                        {
                            addressViewModel.Groups.Remove(Group);
                            await _api.ChangeAddress(addressViewModel.Address);
                        }
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    // We need a temp list because _api.DeleteAddress changes the collection
                    var temp = SelectedAddresses.ToList();
                    foreach (AddressViewModel address in temp)
                    {
                        try
                        {
                            await _api.DeleteAddress(address.Address);
                        }
                        catch (ApiException e)
                        {
                            if (e.Content.Contains("Not found"))
                            {
                                // We're fine, the address didn't exist anyway. keep going.
                            }
                        }
                    }
                }
            }
        }

        public void SendEmail()
        {
            string mailto = "mailto:?bcc=";
            int i = 0;
            if (SelectedAddresses != null && SelectedAddresses.Count > 0)
            {
                foreach (Address selectedAddress in SelectedAddresses.Select(o => o.Address))
                {
                    // Microsoft does not seem to care about standards and uses ", " as a separator
                    // instead of just ",", as RFC 8086 says.
                    mailto += selectedAddress.Email + ", ";
                    i++;
                }
                System.Diagnostics.Process.Start(mailto);
                Console.WriteLine("Sent email to " + i + " recipients");
                Console.WriteLine(mailto);
            }
        }

        public async void SaveAddresses()
        {
            var changedAddresses = Addresses.Where(address => address.Changed).ToList();
            foreach (AddressViewModel address in changedAddresses)
            {
                try
                {

                    if (_api.GetAddress(address.Email, AddressBook.Id) != null)
                    {
                        address.Id = _api.GetAddress(address.Email, AddressBook.Id).Id;
                        address.Groups = _api.GetAddress(address.Email, AddressBook.Id).Groups;
                        if (Group != null)
                        {
                            address.Groups.Add(Group);
                        }
                        await _api.ChangeAddress(address.Address);
                        address.Changed = false;
                    }

                    if (address.Id == 0 && Group != null)
                    {
                        if (address.Groups == null)
                        {
                            address.Groups = new List<Group>();
                        }
                        address.Groups.Add(Group);
                        await _api.CreateAddress(address.Address);
                        address.Changed = false;
                    }
                }
                catch (Exception e)
                {
                    if (e is ApiException && ((ApiException) e).Content.Contains("Duplicate"))
                    {
                        Console.WriteLine(_api.GetAddress(address.Email, AddressBook.Id));
                        _windowManager.ShowMessageBox("Email duplicado");
                    }
                    else
                    {
                        _windowManager.ShowMessageBox("Error inesperado, mensaje: " + e.Message);
                    }
                }
            }
        }

        public void DeselectGroup()
        {
            Group = null;
            NotifyDataSetChanged();
        }

        public async Task CheckPermissions()
        {
            var user = await _api.GetLoggedInUser();
            CanAddAddress = user.CanAddAddress;
            CanChangeAddress = user.CanChangeAddress;
            CanDeleteAddress = user.CanDeleteAddress;
        }

        public async void NotifyDataSetChanged()
        {
            await CheckPermissions();

            foreach (var addressBook in _api.GetAddressBooks())
            {
                if (!AddressBooks.Contains(addressBook))
                {
                    AddressBooks.Add(addressBook);
                }
                else
                {
                    var oldBook = AddressBooks.First(o => o.Id.Equals(addressBook.Id));
                    oldBook.Id = addressBook.Id;
                    oldBook.Name = addressBook.Name;
                    oldBook.Groups = addressBook.Groups;
                    oldBook.Owner = addressBook.Owner;
                    oldBook.SharedWith = addressBook.SharedWith;
                }
            }

            AddressBooks.RemoveRange(AddressBooks.Except(_api.GetAddressBooks()).ToList());

            if (AddressBook != null)
            {
                foreach (var group in AddressBook.Groups)
                {
                    if (!Groups.Contains(group))
                    {
                        Groups.Add(group);
                    }
                    else
                    {
                        var oldGroup = Groups.First(o => o.Id.Equals(group.Id));
                        oldGroup.Id = group.Id;
                        oldGroup.Name = group.Name;
                        oldGroup.Addresses = group.Addresses;
                    }
                }

                Groups.RemoveRange(Groups.Except(AddressBook.Groups).ToList());
            }
            else
            {
                Groups.Clear();
            }

            if (Group != null)
            {
                foreach (Address address in Group.Addresses)
                {
                    AddressViewModel addressViewModel = new AddressViewModel(address);
                    if (addressViewModel.Changed) break;
                    if (!Addresses.Contains(addressViewModel))
                    {
                        Addresses.Add(addressViewModel);
                    }
                    else
                    {
                        var oldAddress = Addresses.First(o => o.Id.Equals(address.Id));
                        if (!oldAddress.Changed)
                        {
                            oldAddress.Id = address.Id;
                            oldAddress.Email = address.Email;
                            oldAddress.Name = address.Name;
                            oldAddress.Changed = false;
                        }
                    }
                }

                // Remove everything except the changed addresses and the addresses in the group
                Addresses.RemoveRange(
                    Addresses.Except(Group.Addresses.Select(o => new AddressViewModel(o)))
                        .Where(o => !o.Changed)
                        .ToList());

                if (SearchText != null)
                {
                    var result =
                        Addresses.Where(
                            o =>
                                o.Email.ToLower().Contains(SearchText.ToLower()) ||
                                o.Name.ToLower().Contains(SearchText.ToLower())).ToList();
                    Addresses.Clear();
                    Addresses.AddRange(result);
                }
            }
            else if (AddressBook != null && Group == null)
            {
                var allAddresses = AddressBook.Groups.SelectMany(group => group.Addresses).ToList();
                foreach (Address address in allAddresses)
                {
                    AddressViewModel addressViewModel = new AddressViewModel(address);
                    if (addressViewModel.Changed) break;
                    if (!Addresses.Contains(addressViewModel))
                    {
                        Addresses.Add(addressViewModel);
                    }
                    else
                    {
                        var oldAddress = Addresses.First(o => o.Id.Equals(address.Id));
                        if (!oldAddress.Changed)
                        {
                            oldAddress.Id = address.Id;
                            oldAddress.Email = address.Email;
                            oldAddress.Name = address.Name;
                            oldAddress.Changed = false;
                        }
                    }
                }

                var distinct = Addresses.Distinct().ToList();
                Addresses.Clear();
                Addresses.AddRange(distinct);
                var invalidAddresses =
                    Addresses.Except(allAddresses.ToList().Select(o => new AddressViewModel(o)))
                        .ToList()
                        .Where(o => !o.Changed);
                Addresses.RemoveRange(invalidAddresses);

                if (SearchText != null)
                {
                    var result =
                        Addresses.Where(
                            o =>
                                o.Email.ToLower().Contains(SearchText.ToLower()) ||
                                o.Name.ToLower().Contains(SearchText.ToLower())).ToList();
                    Addresses.Clear();
                    Addresses.AddRange(result);
                }
                CanAddAddress = false;
            }
        }


    }
}
