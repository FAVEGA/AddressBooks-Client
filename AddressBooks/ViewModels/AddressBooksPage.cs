using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AddressBooks.Api;
using AddressBooks.Models;
using Refit;
using Stylet;

namespace AddressBooks.ViewModels
{
    class AddressBooksPage : Screen, IRegisterable
    {
        private readonly IWindowManager _windowManager;
        private IAuthenticatedAddressBooksApi _api;

        private string _newAddressBookName;
        public string NewAddressBookName
        {
            get { return _newAddressBookName; }
            set { SetAndNotify(ref _newAddressBookName, value); }
        }

        private string _addressBookName;
        public string AddressBookName
        {
            get { return _addressBookName; }
            set { SetAndNotify(ref _addressBookName, value); }
        }

        private AddressBook _addressBook;
        public AddressBook AddressBook
        {
            get { return _addressBook; }
            set { SetAndNotify(ref _addressBook, value); }
        }

        private bool _canDeleteAddressBook;
        public bool CanDeleteAddressBook
        {
            get { return _canDeleteAddressBook; }
            set { SetAndNotify(ref _canDeleteAddressBook, value); }
        }

        private bool _canAddAddressBook;
        public bool CanAddAddressBook
        {
            get { return _canAddAddressBook; }
            set { SetAndNotify(ref _canAddAddressBook, value); }
        }

        private bool _canChangeAddressBook;
        public bool CanChangeAddressBook
        {
            get { return _canChangeAddressBook; }
            set { SetAndNotify(ref _canChangeAddressBook, value); }
        }

        private bool _canShareAddressBook;
        public bool CanMoveUserToNotSharedWith
        {
            get { return _canShareAddressBook; }
            set { SetAndNotify(ref _canShareAddressBook, value); }
        }
        public bool CanMoveUserToSharedWith
        {
            get { return _canShareAddressBook; }
            set { SetAndNotify(ref _canShareAddressBook, value); }
        }

        private IObservableCollection<AddressBook> _addressBooks;
        public IObservableCollection<AddressBook> AddressBooks
        {
            get { return _addressBooks; }
            set { SetAndNotify(ref _addressBooks, value); }
        }

        private IObservableCollection<User> _sharedWith;
        public IObservableCollection<User> SharedWith
        {
            get { return _sharedWith; }
            set { SetAndNotify(ref _sharedWith, value); }
        }

        private User _sharedWithSelectedUser;
        public User SharedWithSelectedUser
        {
            get { return _sharedWithSelectedUser; }
            set { SetAndNotify(ref _sharedWithSelectedUser, value); }
        }

        private IObservableCollection<User> _notSharedWith;
        public IObservableCollection<User> NotSharedWith
        {
            get { return _notSharedWith; }
            set { SetAndNotify(ref _notSharedWith, value); }
        }

        private User _notSharedWithSelectedUser;
        public User NotSharedWithSelectedUser
        {
            get { return _notSharedWithSelectedUser; }
            set { SetAndNotify(ref _notSharedWithSelectedUser, value); }
        }

        public AddressBooksPage(IWindowManager windowManager, IAuthenticatedAddressBooksApi api, IRegistrar registrar)
        {
            registrar.Register(this);
            _windowManager = windowManager;
            _api = api;
            DisplayName = "Libretas";
            AddressBooks = new BindableCollection<AddressBook>();
            SharedWith = new BindableCollection<User>();
            NotSharedWith = new BindableCollection<User>();
        }

        public void AddressBooksSelectionChanged()
        {
            if (AddressBook != null)
            {
                AddressBookName = AddressBook.Name;
                SharedWith.Clear();
                SharedWith.AddRange(AddressBook.SharedWith.OrderBy(o => o.Username));
                NotSharedWith.Clear();
                NotSharedWith.AddRange(_api.GetUsers().Except(AddressBook.SharedWith).ToList().OrderBy(o => o.Username));
            }
            else
            {
                AddressBookName = "";
            }

        }

        public async void CheckPermissions()
        {
            var user = await _api.GetLoggedInUser();
            CanDeleteAddressBook = user.CanDeleteAddressBook;
            CanAddAddressBook = user.CanAddAddressBook;
            CanChangeAddressBook = user.CanChangeAddressBook;
            CanMoveUserToSharedWith = user.CanShareAddressBook;
        }

        public void NotifyDataSetChanged()
        {
            CheckPermissions();

            foreach (var addressBook in _api.GetAddressBooks())
            {
                if (!AddressBooks.Contains(addressBook))
                {
                    AddressBooks.Add(addressBook);
                }
                else
                {
                    // Update the current AddressBook if needed
                    var oldBook = AddressBooks.First(o => o.Id.Equals(addressBook.Id));
                    oldBook.Id = addressBook.Id;
                    oldBook.Name = addressBook.Name;
                    oldBook.Groups = addressBook.Groups;
                    oldBook.Owner = addressBook.Owner;
                    oldBook.SharedWith = addressBook.SharedWith;
                }
            }

            AddressBooks.RemoveRange(AddressBooks.Except(_api.GetAddressBooks()).ToList());

            AddressBooksSelectionChanged();
        }

        public async void MoveUserToSharedWith()
        {
            if (AddressBook != null && NotSharedWithSelectedUser != null)
            {
                AddressBook.SharedWith.Add(NotSharedWithSelectedUser);
                SharedWith.Add(NotSharedWithSelectedUser);
                NotSharedWith.Remove(NotSharedWithSelectedUser);
            }
        }

        public async void MoveUserToNotSharedWith()
        {
            if (AddressBook != null && SharedWithSelectedUser != null)
            {
                NotSharedWith.Add(SharedWithSelectedUser);
                AddressBook.SharedWith.Remove(SharedWithSelectedUser);
                SharedWith.Remove(SharedWithSelectedUser);
            }
        }

        public async void ChangeAddressBook()
        {
            if (AddressBook != null)
            {
                AddressBook.Name = AddressBookName;
                await _api.ChangeAddressBook(AddressBook);
            }
        }

        public async void AddAddressBook()
        {
            var addressBook = new AddressBook
            {
                Name = NewAddressBookName,
                Owner = await _api.GetLoggedInUser(),
                Groups = new List<Group>(),
                SharedWith = new List<User>()
            };
            try
            {
                addressBook = await _api.CreateAddressBook(addressBook);
                AddressBook = addressBook;
                NewAddressBookName = "";
                NotifyDataSetChanged();
            }
            catch (ApiException e)
            {
                if (e.Content.Contains("Duplicate address book name"))
                {
                    _windowManager.ShowMessageBox("Ya existe una libreta de direcciones con ese nombre.");
                }
            }
        }

        public async void DeleteAddressBook()
        {
            if (AddressBook == null)
            {
                _windowManager.ShowMessageBox("No ha seleccionado ninguna libreta.");
            }
            MessageBoxResult confirmResult = _windowManager.ShowMessageBox("Esta seguro de que desea eliminar esta libreta?", "Eliminar libreta", MessageBoxButton.YesNo);
            if (confirmResult == MessageBoxResult.Yes)
            {
                try
                {
                    await _api.DeleteAddressBook(AddressBook);
                }
                catch (ApiException)
                {
                    _windowManager.ShowMessageBox("Error eliminando la libreta.");
                }
            }
        }
    }
}
