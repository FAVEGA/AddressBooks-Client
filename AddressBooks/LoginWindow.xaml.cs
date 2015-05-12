using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AddressBooks
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            var body = new Dictionary<string, string>()
            {
                {"username", Username.Text},
                {"password", Password.Password}
            };
            try
            {
                await RestApi.PopulateApiToken(body);
                User user = await RestApi.GetCurrentUser();
                new MainWindow(user).Show();
                this.Close();
            }
            catch (ApiException ex)
            {
                if (ex.Content == @"{""non_field_errors"":[""Unable to log in with provided credentials.""]}")
                {
                    MessageBox.Show("Error al iniciar sesion. Usuario o contraseña incorrectos.");
                }
                else
                {
                    MessageBox.Show("Error inesperado al inciar sesion. Intentelo de nuevo mas tarde.");
                }
            }
        }
    }
}