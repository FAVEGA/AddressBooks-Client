using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AddressBooks.Api;
using AddressBooks.Models;
using Refit;
using Stylet;

namespace AddressBooks.ViewModels
{
    class GroupsPage : Screen, IRegisterable
    {
        private readonly IWindowManager _windowManager;
        private IAuthenticatedAddressBooksApi _api;

        private bool _canAddGroup;
        public bool CanAddGroup
        {
            get { return _canAddGroup; }
            set { SetAndNotify(ref _canAddGroup, value); }
        }

        private bool _canChangeGroup;
        public bool CanChangeGroup
        {
            get { return _canChangeGroup; }
            set { SetAndNotify(ref _canChangeGroup, value); }
        }

        private bool _canDeleteGroup;
        public bool CanDeleteGroup
        {
            get { return _canDeleteGroup; }
            set { SetAndNotify(ref _canDeleteGroup, value); }
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

        private string _groupName;
        public string GroupName
        {
            get { return _groupName; }
            set { SetAndNotify(ref _groupName, value); }
        }

        private string _newGroupName;
        public string NewGroupName
        {
            get { return _newGroupName; }
            set { SetAndNotify(ref _newGroupName, value); }
        }

        public GroupsPage(IWindowManager windowManager, IAuthenticatedAddressBooksApi api, IRegistrar registrar)
        {
            DisplayName = "Grupos";
            _api = api;
            registrar.Register(this);
            _windowManager = windowManager;
            AddressBooks = new BindableCollection<AddressBook>();
            Groups = new BindableCollection<Group>();
        }

        public async void AddGroup()
        {
            if (AddressBook == null)
            {
                _windowManager.ShowMessageBox("No ha seleccionado ninguna libreta.");
                return;
            }

            try
            {
                var group = new Group
                {
                    Name = NewGroupName,
                    AddressBook = AddressBook,
                    Addresses = new List<Address>()
                };
                group = await _api.CreateGroup(group);
                Group = group;
                NewGroupName = "";
                NotifyDataSetChanged();
            }
            catch (ApiException e)
            {
                if (e.Content.Contains("Duplicate group name"))
                {
                    _windowManager.ShowMessageBox("Ya existe una libreta de direcciones con ese nombre.");
                }
            }
        }

        public async void DeleteGroup()
        {
            if (Group == null)
            {
                _windowManager.ShowMessageBox("No ha seleccionado ningun grupo.");
            }
            MessageBoxResult confirmResult = _windowManager.ShowMessageBox("Esta seguro de que desea eliminar este grupo?", "Eliminar grupo", MessageBoxButton.YesNo);
            if (confirmResult == MessageBoxResult.Yes)
            {
                try
                {
                    await _api.DeleteGroup(Group);
                }
                catch (ApiException)
                {
                    _windowManager.ShowMessageBox("Error eliminando el grupo.");
                }
            }
        }

        public async void ChangeGroup()
        {
            if (Group != null)
            {
                Group.Name = GroupName;
                await _api.ChangeGroup(Group);
            }
        }

        public async void CheckPermissions()
        {
            var user = await _api.GetLoggedInUser();
            CanAddGroup = user.CanAddGroup;
            CanChangeGroup = user.CanChangeGroup;
            CanDeleteGroup = user.CanDeleteGroup;
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

            GroupName = Group != null ? Group.Name : "";
        }
    }
}
