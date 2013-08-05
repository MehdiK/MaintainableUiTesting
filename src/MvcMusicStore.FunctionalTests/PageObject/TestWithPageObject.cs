using MvcMusicStore.FunctionalTests.Framework;
using MvcMusicStore.FunctionalTests.PageObject.Pages;
using NUnit.Framework;

namespace MvcMusicStore.FunctionalTests.PageObject
{
    public class PageObject
    {
        [Test]
        public void Can_buy_an_Album_when_registered()
        {
            var driver = Host.Instance.WebDriver;
            driver.Navigate().GoToUrl(driver.Url);
            var homepage = new HomePage();
            var registerPage = homepage
                .GoToAdminForAnonymousUser()
                .GoToRegisterPage();

            registerPage.Username = "HJSimpson";
            registerPage.Email = "chunkylover53@aol.com";
            registerPage.Password = "!2345Qwert";
            registerPage.ConfirmPassword = "!2345Qwert";

            homepage = registerPage.SubmitRegistration();
            var shippingPage = homepage
                .SelectGenreByName("Disco")
                .SelectAlbumByName("Le Freak")
                .AddToCart()
                .Checkout();

            shippingPage.FirstName = "Homer";
            shippingPage.LastName = "Simpson";
            shippingPage.Address = "742 Evergreen Terrace";
            shippingPage.City = "Springfield";
            shippingPage.State = "Kentucky";
            shippingPage.PostalCode = "123456";
            shippingPage.Country = "United States";
            shippingPage.Phone = "2341231241";
            shippingPage.Email = "chunkylover53@aol.com";
            shippingPage.PromoCode = "FREE";
            var orderPage = shippingPage.SubmitOrder();
            Assert.AreEqual(orderPage.Title, "Checkout Complete");
        }
    }
}
