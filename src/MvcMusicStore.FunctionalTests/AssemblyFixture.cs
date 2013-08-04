using NUnit.Framework;

namespace MvcMusicStore.FunctionalTests
{
    public class Host
    {
        internal static readonly SeleniumApplication Instance = new SeleniumApplication();
    }

    [SetUpFixture]
    public class AssemblyFixture
    {
        [SetUp]
        public void SetUp()
        {
            Host.Instance.Run("MvcMusicStore", 12345);
        }
    }
}