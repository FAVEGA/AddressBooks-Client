using System;
using AddressBooks.Api;
using AddressBooks.Models;
using Refit;
using Stylet;

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
                await addressBooksApi.Authenticate(Username, Password);
                User user = await ((CachedAddressBooksApi)addressBooksApi).GetLoggedInUser();
                RequestClose();
                
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
