using MvcMusicStore.FunctionalTests.Framework;
using OpenQA.Selenium;

namespace MvcMusicStore.FunctionalTests.PageObject.Pages
{
    public class ShoppingCart : Page
    {
        public AddressAndPaymentPage Checkout()
        {
            return NavigateTo<AddressAndPaymentPage>(By.LinkText("Checkout >>"));
        }
    }
}