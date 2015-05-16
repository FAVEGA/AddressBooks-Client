using AddressBooks.Models;
using AddressBooks.Views;
using Refit;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AddressBooks.ViewModels
{
    class LoginShell : Screen
    {

        private readonly IWindowManager windowManager;

        public string Username { get; set; }

        public string Password{ get; set; }

        public LoginShell(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        public async void Initialized()
        {
            try
            {
                var body = new Dictionary<string, string>()
                {
                    {"username", "admin"},
                    {"password", "12481632a"}
                };
                try
                {
                    await AddressBooksApi.PopulateApiToken(body);
                    User user = await AddressBooksApi.GetCurrentUser();
                    windowManager.ShowWindow(new MainShell());
                    this.RequestClose();

                }
                catch (ApiException ex)
                {
                    if (ex.Content == @"{""non_field_errors"":[""Unable to log in with provided credentials.""]}")
                    {
                        windowManager.ShowMessageBox("Error al iniciar sesion. Usuario o contraseña incorrectos.");
                    }
                    else
                    {
                        windowManager.ShowMessageBox("Error inesperado al inciar sesion. Intentelo de nuevo mas tarde.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async void Login()
        {
            var body = new Dictionary<string, string>()
            {
                {"username", Username},
                {"password", Password}
            };
            try
            {
                await AddressBooksApi.PopulateApiToken(body);
                User user = await AddressBooksApi.GetCurrentUser();
                windowManager.ShowWindow(new MainShell());
                this.RequestClose();
                
            }
            catch (ApiException ex)
            {
                if (ex.Content == @"{""non_field_errors"":[""Unable to log in with provided credentials.""]}")
                {
                    windowManager.ShowMessageBox("Error al iniciar sesion. Usuario o contraseña incorrectos.");
                }
                else
                {
                    windowManager.ShowMessageBox("Error inesperado al inciar sesion. Intentelo de nuevo mas tarde.");
                }
            }
        }
    }
}
