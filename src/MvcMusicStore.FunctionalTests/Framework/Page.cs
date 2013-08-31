using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;

namespace MvcMusicStore.FunctionalTests.Framework
{
    public class Page
    {
        protected RemoteWebDriver WebDriver
        {
            get { return Host.Instance.WebDriver; }
        }

        private void Capture()
        {
            var capturer = new Capturer(WebDriver);
            capturer.CaptureScreenshot();
            capturer.CapturePageSource();
        }

        public TPage NavigateTo<TPage>(By by) where TPage:Page, new()
        {
            try
            {
                WebDriver.FindElement(by).Click();
                return Activator.CreateInstance<TPage>();
            }
            catch
            {
                //Capture();
                throw;
            }
        }

        public void Execute(By by, Action<IWebElement> action)
        {
            try
            {
                var element = WebDriver.FindElement(by);
                action(element);
            }
            catch
            {
                //Capture();
                throw;
            }
        }

        public string Title { get { return WebDriver.Title; } }

        public void SetText(string elementName, string newText)
        {
            Execute(By.Name(elementName), e =>
                {
                    e.Clear();
                    e.SendKeys(newText);
                } );
        }
    }
}
