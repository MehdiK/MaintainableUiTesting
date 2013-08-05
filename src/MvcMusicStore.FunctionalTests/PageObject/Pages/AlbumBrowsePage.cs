using OpenQA.Selenium;

namespace MvcMusicStore.FunctionalTests.PageObject.Pages
{
    public class AlbumBrowsePage : Page
    {
        public AlbumDetailPage SelectAlbumByName(string name)
        {
            string selector = string.Format("img[alt=\"{0}\"]", name);
            return NavigateTo<AlbumDetailPage>(By.CssSelector(selector));
        }
    }
}