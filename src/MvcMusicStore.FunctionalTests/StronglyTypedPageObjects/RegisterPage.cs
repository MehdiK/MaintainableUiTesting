using MvcMusicStore.FunctionalTests.Framework;
using MvcMusicStore.Models;
using OpenQA.Selenium;

namespace MvcMusicStore.FunctionalTests.StronglyTypedPageObjects
{
    public class RegisterPage : Page<RegisterModel>
    {
        public HomePage CreateValidUser(RegisterModel model)
        {
            Model(model);
            return NavigateTo<HomePage>(By.CssSelector("input[type='submit']"));
        }
    }
}