using MvcMusicStore.FunctionalTests.Framework;
using OpenQA.Selenium;

namespace MvcMusicStore.FunctionalTests.PageObject.Pages
{
    public class LogonPage : Page
    {
        public RegisterPage GoToRegisterPage()
        {
            return NavigateTo<RegisterPage>(By.LinkText("Register"));
        }
    }
}