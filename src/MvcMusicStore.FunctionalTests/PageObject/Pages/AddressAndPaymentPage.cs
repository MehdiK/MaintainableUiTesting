using OpenQA.Selenium;

namespace MvcMusicStore.FunctionalTests.PageObject.Pages
{
    public class AddressAndPaymentPage : Page
    {
        public string FirstName { set { Execute(By.Name("FirstName"), e => { e.Clear(); e.SendKeys(value);}); } }
        public string LastName { set { Execute(By.Name("LastName"), e => { e.Clear(); e.SendKeys(value); }); } }
        public string Address { set { Execute(By.Name("Address"), e => { e.Clear(); e.SendKeys(value); }); } }
        public string City { set { Execute(By.Name("City"), e => { e.Clear(); e.SendKeys(value); }); } }
        public string State { set { Execute(By.Name("State"), e => { e.Clear(); e.SendKeys(value); }); } }
        public string PostalCode { set { Execute(By.Name("PostalCode"), e => { e.Clear(); e.SendKeys(value); }); } }
        public string Country { set { Execute(By.Name("Country"), e => { e.Clear(); e.SendKeys(value); }); } }
        public string Phone { set { Execute(By.Name("Phone"), e => { e.Clear(); e.SendKeys(value); }); } }
        public string Email { set { Execute(By.Name("Email"), e => { e.Clear(); e.SendKeys(value); }); } }
        public string PromoCode { set { Execute(By.Name("PromoCode"), e => { e.Clear(); e.SendKeys(value); }); } }

        public Page SubmitOrder()
        {
            return NavigateTo<Page>(By.CssSelector("input[type=submit]"));
        }
    }
}