using OpenQA.Selenium;

namespace MvcMusicStore.FunctionalTests.PageObject.Pages
{
    public class RegisterPage : Page
    {
        public HomePage SubmitRegistration()
        {
            return NavigateTo<HomePage>(By.CssSelector("input[type='submit']"));
        }

        public string Username          { set { Execute(By.Name("UserName"), e => { e.Clear(); e.SendKeys(value);}); } }
        public string Email             { set { Execute(By.Name("Email"), e => { e.Clear(); e.SendKeys(value);}); } }
        public string ConfirmPassword   { set { Execute(By.Name("ConfirmPassword"), e => { e.Clear(); e.SendKeys(value);}); } }
        public string Password          { set { Execute(By.Name("Password"), e => { e.Clear(); e.SendKeys(value);}); } }
    }
}