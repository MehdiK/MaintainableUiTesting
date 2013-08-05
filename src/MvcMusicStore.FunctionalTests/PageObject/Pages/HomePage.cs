using MvcMusicStore.FunctionalTests.Framework;
using OpenQA.Selenium;

namespace MvcMusicStore.FunctionalTests.PageObject.Pages
{
    public class HomePage : Page
    {
        public LogonPage GoToAdminForAnonymousUser()
        {
            return NavigateTo<LogonPage>(By.LinkText("Admin"));
        }

        public AlbumBrowsePage SelectGenreByName(string genre)
        {
            return NavigateTo<AlbumBrowsePage>(By.LinkText(genre));
        }
    }
}
