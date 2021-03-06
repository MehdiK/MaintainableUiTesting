﻿using System;
using MvcMusicStore.FunctionalTests.Framework;
using OpenQA.Selenium;

namespace MvcMusicStore.FunctionalTests.PageObjects
{
    public class HomePage : Page
    {
        public static HomePage Initiate()
        {
            var driver = Host.Instance.WebDriver;
            Host.Instance.WebDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            driver.Navigate().GoToUrl(driver.Url);
            return new HomePage();
        }

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
