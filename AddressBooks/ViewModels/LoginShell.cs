using AddressBooks.Api;
using AddressBooks.Models;
using AddressBooks.Views;
using Refit;
using Stylet;
using StyletIoC;
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
        private IAuthenticatedAddressBooksApi addressBooksApi;

        public string Username { get; set; }

        public string Password{ get; set; }

        public LoginShell(IWindowManager windowManager, IAuthenticatedAddressBooksApi addressBooksApi)
        {
            this.windowManager = windowManager;
            this.addressBooksApi = addressBooksApi;
        }

        public async void Login()
        {
            try
            {
                await this.addressBooksApi.Authenticate(Username, Password);
                User user = await ((CachedAddressBooksApi)this.addressBooksApi).GetLoggedInUser();
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
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
