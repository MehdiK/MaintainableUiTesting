using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace MvcMusicStore.FunctionalTests.Framework
{
    public class Capturer
    {
        public static string OutputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FailedTests");
        private readonly RemoteWebDriver _webDriver;

        public Capturer(RemoteWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public void CaptureScreenshot(string fileName = null)
        {
            var camera = (ITakesScreenshot) _webDriver;
            var screenshot = camera.GetScreenshot();

            var screenShotPath = GetOutputFilePath(fileName, "png");
            screenshot.SaveAsFile(screenShotPath, ImageFormat.Png);
        }

        public void CapturePageSource(string fileName = null)
        {
            var filePath = GetOutputFilePath(fileName, "html");
            File.WriteAllText(filePath, _webDriver.PageSource);
        }

        private string GetOutputFilePath(string fileName, string fileExtension)
        {
            if (!Directory.Exists(OutputFolder))
                Directory.CreateDirectory(OutputFolder);

            var windowTitle = _webDriver.Title;
            fileName = fileName ??
                       string.Format("{0}{1}.{2}", windowTitle, DateTime.Now.ToFileTime(), fileExtension).Replace(':', '.');
            var outputPath = Path.Combine(OutputFolder, fileName);
            var pathChars = Path.GetInvalidPathChars();
            var stringBuilder = new StringBuilder(outputPath);

            foreach (var item in pathChars)
                stringBuilder.Replace(item, '.');

            var screenShotPath = stringBuilder.ToString();
            return screenShotPath;
        }
    }
}