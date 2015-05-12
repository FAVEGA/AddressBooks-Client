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

namespace AddressBooks
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

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddressesTab.IsSelected)
            {
                var addressBooks = await RestApi.GetAddressBooks();
                foreach (AddressBook book in addressBooks)
                {
                    AddressesAddressBookComboBox.Items.Add(book.Name);
                }
            }
        }
    }
}
