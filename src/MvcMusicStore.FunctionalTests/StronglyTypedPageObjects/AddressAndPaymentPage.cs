using MvcMusicStore.FunctionalTests.Framework;
using MvcMusicStore.Models;
using OpenQA.Selenium;

namespace MvcMusicStore.FunctionalTests.StronglyTypedPageObjects
{
    public class AddressAndPaymentPage : Page<Order>
    {
        public Page SubmitShippingInfo(Order order, string promoCode)
        {
            Model(order);
            SetText("PromoCode", promoCode);
            return NavigateTo<Page>(By.CssSelector("input[type=submit]"));
        }
    }
}