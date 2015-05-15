using AddressBooks.Models;
using AddressBooks.Views;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AddressBooks.ViewModels
{
    class LoginShell
    {
        public TextBox Username { get; set; }

        public PasswordBox Password { get; set; }

        public async void Login()
        {
            Console.Write(Stylet.ModelValidation.Xaml.Secure.GetPassword(Password));
            Console.Write(Username.Text);
            var body = new Dictionary<string, string>()
            {
                {"username", Username.Text},
                {"password", Stylet.ModelValidation.Xaml.Secure.GetPassword(Password)}
            };
            try
            {
                await RestApi.PopulateApiToken(body);
                User user = await RestApi.GetCurrentUser();
                new MainWindow(user).Show();
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
