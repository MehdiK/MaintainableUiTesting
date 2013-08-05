using OpenQA.Selenium;

namespace MvcMusicStore.FunctionalTests.PageObject.Pages
{
    public class AlbumDetailPage : Page
    {
        public ShoppingCart AddToCart()
        {
            return NavigateTo<ShoppingCart>(By.LinkText("Add to cart"));
        }
    }
}