using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AddressBooks.Models;
using AddressBooks.Views;

namespace AddressBooks.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        User user;

        public MainWindow(User user)
        {
            InitializeComponent();
            this.user = user;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddressesTab.IsSelected)
            {
                AddressesAddressBookComboBox.Items.Clear();
                AddressesAddressBookComboBox.Items.Add("Cargando...");
                AddressesAddressBookComboBox.Text = "Cargando...";
                var addressBooks = RestApi.GetAddressBooks().OrderBy(o => o.Name).ToList();
                AddressesAddressBookComboBox.Items.Clear();
                foreach (AddressBook book in addressBooks)
                {
                    AddressesAddressBookComboBox.Items.Add(book);
                }
                e.Handled = true;
            }
        }

        private void AddressesAddressBookComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine(JsonConvert.SerializeObject(AddressesAddressBookComboBox.SelectedItem));
            AddressesGroupsComboBox.Items.Clear();
            if (AddressesAddressBookComboBox.SelectedItem is AddressBook)
            {
                Console.WriteLine("Groups: " + JsonConvert.SerializeObject(((AddressBook)AddressesAddressBookComboBox.SelectedItem).Groups));
                foreach (Group group in ((AddressBook)AddressesAddressBookComboBox.SelectedItem).Groups)
                {
                    AddressesGroupsComboBox.Items.Add(group.Name);
                }
            }
            e.Handled = true;
        }

        private void AddressesGroupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (RestApi.LastUpdate != null && DateTime.Now.Subtract(RestApi.LastUpdate).TotalMinutes > 5)
            {
                // Time for an update
                RestApi.LastUpdate = DateTime.Now;
                RestApi.PopulateAddressBooks();
                Console.Write("Window is deactivated and it's time for a data update.");
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            RestApi.PopulateAddressBooks();
        }
    }
}
