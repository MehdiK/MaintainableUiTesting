A few years ago I was very skeptical about automated UI testing. This skeptisicm was born out of few failed attempts at automating UI tests. I would write some test automation for desktop or web applications and I would end up ripping them out of the codebase because the cost of maintaining them was too high. So my temporary conclusion was that UI testing is hard and while it provides a lot of benefit it's best to keep it to a minimum and only test the most complex workflows in a system through UI testing and leave the rest to unit tests. I remember telling my team that in a typical system 80% of the tests should be unit tests, 10% to 15% should be integration and only around 5% should be UI tests. 

After wasting so much time writing bad UI tests and even more time fixing the broken tests I finally got it. Sure! UI testing is hard. It takes a fair bit of time to write UI tests properly. Also UI tests are more brittle than unit tests because they cross the class boundaries, they hit the browser, they involve UI elements (e.g. html, javascript) which are constantly changing, they hit database, file system and potentially network services. If any of these moving parts don't play nicely you have a broken test; but that's also the beauty of UI automation: they test your system end-to-end. No other test gives you as much and thorough coverage. Automated UI tests, if done right, could be the best elements in your regression suite. So in the past few projects my UI tests have formed over 80% of my tests! I should also mention that these projects have mostly been CRUD applications with not much business logic. The business logic should still be unit tested; but the rest of the application can be thoroughly tested through UI automation. 

Before we get started, you can access the code for this article [here](https://github.com/MehdiK/MaintainableUiTesting).

## UI Testing Gone Wrong
I would like to quickly touch on what I did wrong which also seems to be very typical amongst developers and testers starting with UI automation.

**So what goes wrong and why?**
A lot of teams start UI automation with screen recorders. If you are doing web automation with Selenium you have most likely used [Selenium IDE](http://docs.seleniumhq.org/docs/02_selenium_ide.jsp). From the Selenium IDE home page:

> The Selenium-IDE (Integrated Development Environment) is the tool you use to develop your Selenium test cases.

This is one of the reasons UI testing turns into a horrible experience: you download and fire up a screen recorder and navigate to your website and go click, click, type, click, type, tab, type, tab, space, click and assert. Then you replay the recording and it works like magic; so you export the actions as a test script, put it into your code, wrap it in a test and execute the test and see the browser coming alive before your eyes and your tests running very smoothly. You get very excited, share your findings with your colleagues and show it off to your boss and they go: "Automate ALL THE THINGS"

A week later and you have 10 automated UI tests and everything seems sweet. Then the business asks you to replace the username with email address as it's caused some confusion amongst users, and so you do. Then like any other great programmer you run your UI test suite, only to find out 90% of your tests are broken and it takes you 2 hours to replace all the references to username in your tests with email address and to get the tests green again. The same thing happens over and over again to the point that you find yourself spending several hours a day fixing broken tests: tests that didn't break because something went wrong with your code; but because you changed a field name in your database/model or you restructured your page slightly.

A few weeks later you stop running your tests because they take too long to run and every day or so you have to spend hours fixing broken tests, and that's the end of your UI automation story. Does that sound familiar?

> You should NOT use Selenium IDE or any other screen recorder to develop your test cases.

All that said screen recorders are not the real reason we end up with a brittle test suite. The code they generate has inherent maintainability issues. Many developers still end up with a brittle UI test suite even without using screen recorders just because their tests exhibit the same attributes.

> All the tests in this article are written againt [Mvc Music Store](http://mvcmusicstore.codeplex.com/) website - although the website had some issues that would make UI testing rather hard so I ported the code and fixed the issues. You can find the actual code I am writing these tests againt on the GitHub Repo for this article [here](https://github.com/MehdiK/MaintainableUiTesting/tree/master/src/MvcMusicStore)

What does a brittle test look like? It looks something like this:

    class BrittleTest
    {
        [Test]
        public void Can_buy_an_Album_when_registered()
        {
            var driver = Host.Instance.Application.Browser;
            driver.Navigate().GoToUrl(driver.Url);
            driver.FindElement(By.LinkText("Admin")).Click();
            driver.FindElement(By.LinkText("Register")).Click();
            driver.FindElement(By.Id("UserName")).Clear();
            driver.FindElement(By.Id("UserName")).SendKeys("HJSimpson");
            driver.FindElement(By.Id("Password")).Clear();
            driver.FindElement(By.Id("Password")).SendKeys("!2345Qwert");
            driver.FindElement(By.Id("ConfirmPassword")).Clear();
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys("!2345Qwert");
            driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            driver.FindElement(By.LinkText("Disco")).Click();
            driver.FindElement(By.CssSelector("img[alt=\"Le Freak\"]")).Click();
            driver.FindElement(By.LinkText("Add to cart")).Click();
            driver.FindElement(By.LinkText("Checkout >>")).Click();
            driver.FindElement(By.Id("FirstName")).Clear();
            driver.FindElement(By.Id("FirstName")).SendKeys("Homer");
            driver.FindElement(By.Id("LastName")).Clear();
            driver.FindElement(By.Id("LastName")).SendKeys("Simpson");
            driver.FindElement(By.Id("Address")).Clear();
            driver.FindElement(By.Id("Address")).SendKeys("742 Evergreen Terrace");
            driver.FindElement(By.Id("City")).Clear();
            driver.FindElement(By.Id("City")).SendKeys("Springfield");
            driver.FindElement(By.Id("State")).Clear();
            driver.FindElement(By.Id("State")).SendKeys("Kentucky");
            driver.FindElement(By.Id("PostalCode")).Clear();
            driver.FindElement(By.Id("PostalCode")).SendKeys("123456");
            driver.FindElement(By.Id("Country")).Clear();
            driver.FindElement(By.Id("Country")).SendKeys("United States");
            driver.FindElement(By.Id("Phone")).Clear();
            driver.FindElement(By.Id("Phone")).SendKeys("2341231241");
            driver.FindElement(By.Id("Email")).Clear();
            driver.FindElement(By.Id("Email")).SendKeys("chunkylover53@aol.com");
            driver.FindElement(By.Id("PromoCode")).Clear();
            driver.FindElement(By.Id("PromoCode")).SendKeys("FREE");
            driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();

            Assert.IsTrue(driver.PageSource.Contains("Checkout Complete"));
        }
    }

You can find BrittleTest class [here](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/BrittleTest.cs) and [Host](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/Framework/Host.cs) is a static class, with a single static property `Instance`, which upon instantiation fires up IIS Express on the website under test and binds Firefox WebDriver to the browser instance. 

This test fires up web browser, goes to the home page of the Mvc Music Store website, registers a new user, browses to an album, adds it to the cart, and checks out. There are different schools of thoughts on UI tests and how much a test should cover. Some believe this test is doing too much and some think it's good. Nonetheless the size of the test is not the reason it's brittle; it's how it's written that makes it a nightmare to maintain. 

**So what is wrong with that code?**

 - This is procedural code. One of the main issues of this style of coding is readability or lack thereof. If you want to change the test or if it breaks because one of the involved pages has changed you will have a hard time figuring out what to change; because it's all a bit pile of code where we get the 'driver' to find an element on the page and to do something with it!
 - This one test on itself might not have much duplication but a few more tests like this and you will have a lot of duplicated selector and logic to interact with web pages from different tests. For example `By.Id("UserName")` selector will be duplicated in all tests that require registration, and `driver.FindElement(By.Id("UserName")).Clear();` and `driver.FindElement(By.Id("UserName")).SendKeys("<some user name>");` are duplicated anywhere you want to interact with UserName textbox. Then there is the whole registration form, and checkout form ETC that will be repeated in all tests needing to interact with them! Then the business asks me to change that to `UserName` to `Email` (this very same thing happened to me in a project about two years ago)! This simple change means too many broken tests because I have used UserName anywhere in my tests I needed registration.
 - There is a lot of magic strings everywhere. Even if I had used `UserName` in all my tests but instead of using magic strings I had somehow extracted that out into a single place and referenced that from everywhere I would really have to change where these "fields" were defined instead of everywhere in my code and then refactoring tools would allow me to simply rename the references.
 
"What does it take to fix this test?", I hear you ask. A "magic" ingredient is what we need.

## The Magic Ingredient: Test Code Is Code!
You either write tests or you don't. If you do, then much like your actual code, you are going have to maintain your tests. So give them the same treatment. What is it about tests that makes us think we can forego quality in them? If anything a bad test suite is going to take a lot longer to maintain than bad code. I have had bad peices of working code in production for years which never broke and I never had to touch them. Sure it was ugly as hell but it worked and it didn't need change. The situation is not quite the same for bad tests though: because bad tests are going to break and fixing them is going to be hard. 

> If you have a test (good or bad) that never fails even in the face of change then you may as well delete it because it's not doing you any good.

So if you have to maintain your tests like you maintain your code, then you should give it some love. Test code is code. Do you apply [SRP](http://www.objectmentor.com/resources/articles/srp.pdf) on your code? Then you should apply it on your tests too. Is your code [DRY](http://c2.com/cgi/wiki?DontRepeatYourself)? Then DRY up your tests too. If you don't write good tests (UI or otherwise) you will waste a lot of time maintaining them. I cannot even count the number of times I have seen developers avoid testing because they think writing tests is a huge waste of time because it takes too much time to maintain. 

There are also patterns that allow you to write more maintainable UI test suite and many of these patterns have been out there for quite some time. These patterns are platform agnostic. I have used these very same ideas and patterns to write UI tests for WPF applications and web applications written in ASP.Net and Ruby on Rails. So regardless of your technology stack you should be able to make your UI tests a lot more maintainable by applying these patterns. 

### Introducing Page Object Pattern
A lot of the abovementioned issues are due to the fact that I have duplication in my code. I have duplicated logic and selectors. How do we get rid of duplication elsewhere in our code? Object Orientation. So let's see how we can apply that to our UI tests.

Page Object is a pattern used to apply object orientation to UI tests. From the [Selenium wiki](https://code.google.com/p/selenium/wiki/PageObjects):

> Within your web app's UI there are areas that your tests interact with. A Page Object simply models these as objects within the test code. This reduces the amount of duplicated code and means that if the UI changes, the fix need only be applied in one place.

The idea is that for each page in your application/website you want to create one Page Object. So Page Objects are basically the UI automation equivalent of your views. 

I have gone ahead and refactored the logics and interactions out of the BrittleTest into a few page objects and created a new test that uses the page objects. You can find the new test [here](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObject/TestWithPageObject.cs). The code is copied here for your reference:

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
    
Admittedly the test body hasn't decreased much in size and in fact I had to create seven new classes to support this test. Despite the more lines of code required for Page Object, at least in the beginning, it helps fix a lot of issues the original brittle test had. More on this further down. For now let's dive a bit deeper into page object pattern and what we did here.

When you implement Page Objects, a textbox in your web page becomes a string property on the Page Object and to fill that textbox your just set that text property to the desired value. So instead of

	driver.FindElement(By.Id("Email")).Clear();
	driver.FindElement(By.Id("Email")).SendKeys("chunkylover53@aol.com");

we can write

	registerPage.Email = "chunkylover53@aol.com";

where registerPage is an instance of the [RegisterPage](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObject/Pages/RegisterPage.cs). Likewise a link on the web page becomes a method on the Page Object and clicking the link turns into calling the method on the Page Object. So instead of

    driver.FindElement(By.LinkText("Admin")).Click();
 
we can write 

	homepage.GoToAdminForAnonymousUser();

In fact any action on our web page becomes a method in our page object and in response to taking that action, i.e. calling the method on the page object, you get an instance of another page object back that points at the web page you just navigated to by taking the action (e.g. submitting a form or clicking a link). This way you can easily chain your view interaction in your test script: 

	var shippingPage = homepage
                .SelectGenreByName("Disco")
                .SelectAlbumByName("Le Freak")
                .AddToCart()
                .Checkout();

On an instance of the [HomePage](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObject/Pages/HomePage.cs) class I call `SelectGenreByName` which clicks on a 'Disco' link on the page which returns an instance of [AlbumBrowsePage](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObject/Pages/AlbumBrowsePage.cs) and then on that page I call `SelectAlbumByName` which clicks on the 'Le Freak' album and returns an instance of [AlbumDetailsPage](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObject/Pages/AlbumDetailPage.cs) and on that page I click the `AddToCart` button which adds the CD to my shopping cart and takes me to the shopping cart where I can `Checkout`. I admit it it is a lot of classes for what used to be no class at all; but we gained a lot of benefits from this practice. Firstly the code is no longer procedural. We have a well contained testing model where each object provides nice encapsulation of interaction with a page. So for example if something changes in your registration logic the only place you have to change is your RegisterPage class instead of having to go through your entire test suite and change every single interaction with the registration view. This modularity also provides for nice reuse; for example you can reuse your Shopping Cart page everywhere you need to interact with the shopping cart. So in a simple practice of moving from procedural to object oriented test code we **almost** eliminated three of the four issues with the initial brittle test which were procedural code, and logic and selector duplication. We still have a little bit of duplication though which we will fix shortly.

**How did we actually implemented those page objects?** 
A page object in it's root is nothing but a wrapper around the normal interaction you have with the page. So in your implementation you could just extract UI interactions our of the brittle tests and put them into their own page objects. For example you could write a RegisterPage that looked like this:

    public class RegisterPage : Page
    {
        public HomePage SubmitRegistration()
        {
            return NavigateTo<HomePage>(By.CssSelector("input[type='submit']"));
        }

        public string Username          { set { Execute(By.Name("UserName"), e => { e.Clear(); e.SendKeys(value);}); } }
        public string Email             { set { Execute(By.Name("Email"), e => { e.Clear(); e.SendKeys(value);}); } }
        public string ConfirmPassword   { set { Execute(By.Name("ConfirmPassword"), e => { e.Clear(); e.SendKeys(value);}); } }
        public string Password          { set { Execute(By.Name("Password"), e => { e.Clear(); e.SendKeys(value);}); } }
    }

a checkbox on the view becomes a bool property on the Page Object and ticking and untickying the checkbox is just a matter of setting that boolean property to true or false.

I have created a Page superclass that takes care of a few things for us and abstract some of the nitty gritty bits away from page objects.

### Page Components!
Page object is great as it encapsulates all the logic and selectors required to interact with a web page into one class; but some web pages are very big and complex. Earlier I said test code is code and we should treat it as such. We normally break big and complex web pages into smaller and in some cases reusable (partial) components. This allows us to compose a web page from smaller more manageable components. We should do the same for our test. To do this we can use Page Components. Page component is very much like a page object: it's a class that encapsulates interaction with some elements on a page. The difference is that it interacts with a small part of a web page. A good example for a page component is a menu bar. A menu bar usually appears on all pages of a web application. You don't really want to keep repeating the code required to interact with the menu in every single page object. Instead you can create a MenuComponent and use it from your page objects. You could also use page components to deal with grids of data on your pages, and to take it a step further the grid page component itself could be composed of grid row page components. 

## BDD and UI Testing - a match made in heaven
UI testing works really well with Behavior Driven Development because you can write your UI tests based on the acceptance criteria provided (hopefully) by the business and BDD helps write the tests in a human readable way and [avoid a few pitfalls](http://www.mehdi-khalili.com/bdd-to-the-rescue).


## Some Frameworks For UI Testing
Although the patterns are platform agnostic using some frameworks could make your UI tests easier to write. Here are the frameworks that I highly recommend:

**Browser automation:**

 - [Selenium](http://docs.seleniumhq.org/) simply automates browsers
 - [PhantomJS](http://phantomjs.org/): a headless WebKit with JavaScript API

**.Net testing frameworks:**

 - [Seleno](http://teststack.github.com/pages/Seleno.html) for testing web applications
 - [White](http://teststack.github.com/pages/white.html) for rich client applications
 
And for BDD:
  
 - [BDDfy](http://teststack.github.com/TestStack.BDDfy/) if you don't want [executable specs](http://mehdi-khalili.com/executable-requirements)
 - [SpecFlow](http://www.specflow.org/specflownew/) if you want executable specs

**Ruby testing frameworks:**
 
 - [Capybara](https://github.com/jnicklas/capybara) for testing web applications.
 - [Poltergeist](https://github.com/jonleighton/poltergeist): a PhantomJS driver for Capybara
 - [Cucumber](http://cukes.info/) for BDD

