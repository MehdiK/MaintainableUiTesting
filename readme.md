A few years ago I was very sceptical about automated UI testing and this scepticism was born out of a few failed attempts. I would write some automated UI tests for desktop or web applications and a few weeks later I would rip them out of the codebase because the cost of maintaining them was too high. So I thought that UI testing was hard and that, while it provided a lot of benefit, it was best to keep it to a minimum and only test the most complex workflows in a system through UI testing and leave the rest to unit tests. I remember telling my team about [Mike Cohn's testing pyramid](http://www.mountaingoatsoftware.com/blog/the-forgotten-layer-of-the-test-automation-pyramid), and that in a typical system over 70% of the tests should be unit tests, around 5% UI tests and the rest integration tests.

I was wrong! Sure, UI testing can be hard. It takes a fair bit of time to write UI tests properly. They are much slower and more brittle than unit tests because they cross the class and process boundaries, they hit the browser, they involve UI elements (e.g. HTML, JavaScript) which are constantly changing, they hit the database, file system and potentially network services. If any of these moving parts don't play nicely you have a broken test; but that's also the beauty of UI tests: they test your system end-to-end. No other test gives you as much or as thorough coverage. Automated UI tests, if done right, could be the best elements in your regression suite. So in the past few projects my UI tests have formed over 80% of my tests! I should also mention that these projects have mostly been CRUD applications with not much business logic and let's face it - the vast majority of software projects fall into this category. The business logic should still be unit tested; but the rest of the application can be thoroughly tested through UI automation. 

## UI Testing Gone Wrong
I would like to touch on what I did wrong, which also seems to be very typical amongst developers and testers starting with UI automation.

**So what goes wrong and why?**
A lot of teams start UI automation with screen recorders. If you are doing web automation with Selenium you have most likely used [Selenium IDE](http://docs.seleniumhq.org/docs/02_selenium_ide.jsp). From the Selenium IDE home page:

> The Selenium-IDE (Integrated Development Environment) is the tool you use to develop your Selenium test cases.

This is actually one of the reasons UI testing turns into a horrible experience: you download and fire up a screen recorder and navigate to your website and go click, click, type, click, type, tab, type, tab, click and assert. Then you replay the recording and it works. Sweet!! So you export the actions as a test script, put it into your code, wrap it in a test and execute the test and see the browser come alive before your eyes and your tests run very smoothly. You get very excited, share your findings with your colleagues and show it off to your boss and they get very excited and go: "[Automate ALL THE THINGS](http://qkme.me/3vg2h6)"

A week later and you have 10 automated UI tests and everything seems great. Then the business asks you to replace the username with email address as it has caused some confusion amongst users, and so you do. Then like any other great programmer you run your UI test suite, only to find 90% of your tests are broken because for each test you are logging the user in with username and the field name has changed and it takes you two hours to replace all the references to `username` in your tests with `email` and to get the tests green again. The same thing happens over and over again and at some point you find yourself spending hours a day fixing broken tests: tests that didn't break because something went wrong with your code; but because you changed a field name in your database/model or you restructured your page slightly. A few weeks later you stop running your tests because of this huge maintenance cost, and you conclude that UI testing sucks. 

**You should NOT use Selenium IDE or any other screen recorder to develop your test cases.** That said, it's not the screen recorder itself that leads to a brittle test suite; it's the code they generate that has inherent maintainability issues. Many developers still end up with a brittle UI test suite even without using screen recorders just because their tests exhibit the same attributes.

> All the tests in this article are written against the [Mvc Music Store](http://mvcmusicstore.codeplex.com/) website. The website as is has some issues that makes UI testing rather hard so I ported the code and fixed the issues. You can find the actual code I am writing these tests against on the GitHub repo for this article [here](https://github.com/MehdiK/MaintainableUiTesting/tree/master/src/MvcMusicStore)

So what does a brittle test look like? It looks something like this:

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

<small>You can find `BrittleTest` class [here](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/BrittleTest.cs). </small>

<small>[Host](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/Framework/Host.cs) is a static class, with a single static property `Instance`, which upon instantiation fires up IIS Express on the website under test and binds Firefox WebDriver to the browser instance. When the test is finished it then closes the browser and IIS Express automatically.</small>

This test fires up web browser, goes to the home page of the Mvc Music Store website, registers a new user, browses to an album, adds it to the cart, and checks out. 

One might argue this test is doing too much and that's why it's brittle; but the size of this test is not the reason it's brittle - it's how it's written that makes it a nightmare to maintain. 

> There are different schools of thought on UI testing and how much each test should cover. Some believe this test is doing too much and some think a test should cover a real scenario end to end and consider this a perfect test (maintainability aside)

**So what is wrong with this test?**

 - This is procedural code. One of the main issues of this style of coding is readability, or lack thereof. If you want to change the test, or if it breaks because one of the involved pages has changed, you will have a hard time figuring out what to change and to draw a line between functionality sections; because it's all a big pile of code where we get the 'driver' to find an element on the page and to do something with it. No modularity.
 - This one test by itself might not have much duplication but a few more tests like this and you will have a lot of duplicated selector and logic to interact with web pages from different tests. For example `By.Id("UserName")` selector will be duplicated in all tests that require registration, and `driver.FindElement(By.Id("UserName")).Clear()` and `driver.FindElement(By.Id("UserName")).SendKeys("<some user name>")` are duplicated anywhere you want to interact with UserName textbox. Then there is the whole registration form, and checkout form etc. that will be repeated in all tests needing to interact with them! Duplicated code leads to maintainability nightmares.
 - There is a lot of magic strings everywhere which again is a maintainability issue.
 
## Test Code Is Code!
**Much like your actual code, you are going to have to maintain your tests. So give them the same treatment. **

What is it about tests that makes us think we can forego quality in them? If anything, a bad test suite in my opinion is a lot harder to maintain than bad code. I have had bad pieces of working code in production for years which never broke and I never had to touch them. Sure it was ugly and hard to read and maintain but it worked and it didn't need change so the real maintenance cost was zero. The situation is not quite the same for bad tests though: because bad tests are going to break and fixing them is going to be hard. I cannot count the number of times I have seen developers avoid testing because they think writing tests is a huge waste of time because it takes too much time to maintain. 

**Test code is code**: <br /> 
Do you apply [SRP](http://www.objectmentor.com/resources/articles/srp.pdf) on your code? Then you should apply it on your tests too. Is your code [DRY](http://c2.com/cgi/wiki?DontRepeatYourself)? Then DRY up your tests too. If you don't write good tests (UI or otherwise) you will waste a lot of time maintaining them. 

There are also patterns that allow you to write more maintainable UI tests. These patterns are platform agnostic: I have used these very same ideas and patterns to write UI tests for WPF applications and web applications written in ASP.Net and Ruby on Rails. So regardless of your technology stack you should be able to make your UI tests a lot more maintainable by following a few simple steps. 

### Introducing the Page Object Pattern
A lot of the abovementioned issues are rooted in the procedural nature of the test script and the solution is easy: Object Orientation. 

Page Object is a pattern used to apply object orientation to UI tests. From the [Selenium wiki](https://code.google.com/p/selenium/wiki/PageObjects):

> Within your web app's UI there are areas that your tests interact with. A Page Object simply models these as objects within the test code. This reduces the amount of duplicated code and means that if the UI changes, the fix need only be applied in one place.

The idea is that for each page in your application/website you want to create one Page Object. Page Objects are basically the UI automation equivalent of your web pages. 

I have gone ahead and refactored the logic and interactions out of the BrittleTest into a few page objects and created a new test that uses them instead of hitting the web driver directly. You can find the new test [here](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObjectTest.cs). The code is copied here for your reference:

    public class TestWithPageObject
    {
        [Test]
        public void Can_buy_an_Album_when_registered()
        {
            var registerPage = HomePage.Initiate()
                .GoToAdminForAnonymousUser()
                .GoToRegisterPage();

            registerPage.Username = "HJSimpson";
            registerPage.Email = "chunkylover53@aol.com";
            registerPage.Password = "!2345Qwert";
            registerPage.ConfirmPassword = "!2345Qwert";

            var shippingPage = registerPage
                .SubmitRegistration()
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
    
Admittedly the test body hasn't decreased much in size and in fact I had to create seven new classes to support this test. Despite the more lines of code required, we just fixed a lot of issues the original brittle test had (more on this further down). For now, let's dive a bit deeper into the page object pattern and what we did here.

With the Page Object pattern you typically create a page object class per web page under test where the class models and encapsulates interactions with the page. So a textbox in your web page becomes a string property on the Page Object and to fill that textbox you just set that text property to the desired value: instead of

	driver.FindElement(By.Id("Email")).Clear();
	driver.FindElement(By.Id("Email")).SendKeys("chunkylover53@aol.com");

we can write

	registerPage.Email = "chunkylover53@aol.com";

where `registerPage` is an instance of the [RegisterPage](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObjects/RegisterPage.cs) class. A checkbox on the page becomes a bool property on the Page Object and ticking and unticking the checkbox is just a matter of setting that boolean property to true or false. Likewise, a link on the web page becomes a method on the Page Object and clicking the link turns into calling the method on the Page Object. So instead of

    driver.FindElement(By.LinkText("Admin")).Click();
 
we can write 

	homepage.GoToAdminForAnonymousUser();

In fact, any action on our web page becomes a method in our page object and in response to taking that action (i.e. calling the method on the page object) you get an instance of another page object back that points at the web page you just navigated to by taking the action (e.g. submitting a form or clicking a link). This way you can easily chain your view interactions in your test script: 

    var shippingPage = registerPage
                .SubmitRegistration()
                .SelectGenreByName("Disco")
                .SelectAlbumByName("Le Freak")
                .AddToCart()
                .Checkout();

Here, after registering the user I get taken to the home page (an instance of its page object is returned by `SubmitRegistration` method). So on the [HomePage](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObjects/HomePage.cs) instance I call `SelectGenreByName` which clicks on a 'Disco' link on the page which returns an instance of [AlbumBrowsePage](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObjects/AlbumBrowsePage.cs) and then on that page I call `SelectAlbumByName` which clicks on the 'Le Freak' album and returns an instance of [AlbumDetailsPage](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObjects/AlbumDetailPage.cs) and so on and so forth. I admit it: it is a lot of classes for what used to be no class at all; but we gained a lot of benefits from this practice. Firstly the code is no longer procedural. We have a well contained testing model where each object provides nice encapsulation of interaction with a page. So for example if something changes in your registration logic the only place you have to change is your RegisterPage class instead of having to go through your entire test suite and change every single interaction with the registration view. This modularity also provides for nice reusability: you can reuse your `ShoppingCartPage` everywhere you need to interact with the shopping cart. So in a simple practice of moving from procedural to object oriented test code we **almost** eliminated three of the four issues with the initial brittle test which were procedural code, and logic and selector duplication. We still have a little bit of duplication though which we will fix shortly.

**How did we actually implement those page objects?** 
A page object in its root is nothing but a wrapper around the interactions you have with the page. Here I just extracted UI interactions our of the brittle tests and put them into their own page objects. For example the registration logic was extracted into it's own class called `RegisterPage` that looked like this:

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

I have created a `Page` superclass that takes care of a few things, like `NavigateTo` which helps navigate to a new page by taking an action and `Execute` that executes some actions on an element. The `Page` class looked like:

    public class Page
    {
        protected RemoteWebDriver WebDriver
        {
            get { return Host.Instance.WebDriver; }
        }

        public string Title { get { return WebDriver.Title; }}

        public TPage NavigateTo<TPage>(By by) where TPage:Page, new()
        {
            WebDriver.FindElement(by).Click();
            return Activator.CreateInstance<TPage>();
        }

        public void Execute(By by, Action<IWebElement> action)
        {
            var element = WebDriver.FindElement(by);
            action(element);
        }
    }

In the `BrittleTest`, to interact with an element we did `FindElement` once per action. The `Execute` method, apart from abstracting web driver's interaction, has an added benefit that allows selecting an element, which could be an expensive action, once and taking multiple actions on it:

    driver.FindElement(By.Id("Password")).Clear();
    driver.FindElement(By.Id("Password")).SendKeys("!2345Qwert");

was replaced with 

    Execute(By.Name("Password"), e => { e.Clear(); e.SendKeys("!2345Qwert");})

Taking a second look at the `RegisterPage` page object above we still have a bit of duplication in there. Test code is code and we don't want duplication in our code; so let's refactor that. We can extract the code required to fill in a textbox into a method on the `Page` class and just call that from page objects. The method could be implemented as:

    public void SetText(string elementName, string newText)
    {
        Execute(By.Name(elementName), e =>
            {
                e.Clear();
                e.SendKeys(newText);
            } );
    }

And now the properties on `RegisterPage` can be shrunk to:
    
    public string Username { set { SetText("UserName", value); } }

You could also make a fluent API for it to make the setter read better (e.g. `Fill("UserName").With(value)`) but I'll leave that to you.

We're not doing anything extraordinary here. Just simple refactoring on our test code like we've always done for our, errrr, "other" code!!

You can see the complete code for `Page` and `RegisterPage` classes [here](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/Framework/Page.cs) and [here](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/PageObjects/RegisterPage.cs).

### Strongly Typed Page Object
We resolved procedural issues with the brittle test which made the test more readable, modular, DRYer and effectively maintainable. There is one last issue we didn't fix: there are still a lot of magic strings everywhere. Not quite a nightmare but still an issue we could fix. Enter strongly typed Page Objects!

This approach is practical if you're using an MV* framework for your UI. In our case we are using ASP.Net MVC. 

Let's take another look at the `RegisterPage`:

    public class RegisterPage : Page
    {
        public HomePage SubmitRegistration()
        {
            return NavigateTo<HomePage>(By.CssSelector("input[type='submit']"));
        }

        public string Username          { set { SetText("UserName", value); } }
        public string Email             { set { SetText("Email", value); } }
        public string ConfirmPassword   { set { SetText("ConfirmPassword", value); } }
        public string Password          { set { SetText("Password", value); } }
    }

This page models the [Register](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore/Views/Account/Register.cshtml) view in our web app (just copying the top bit here for your convenience):

	@model MvcMusicStore.Models.RegisterModel
	
	@{
	    ViewBag.Title = "Register";
	}

Hmmm, what's that `RegisterModel` there? It's the View Model for the page: the `M` in the `MVC`. [Here](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore/Models/AccountModels.cs#L41) is the code (I removed the attributes to reduce the noise):

    public class RegisterModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

That looks very familiar, doesn't it? It has the same properties as the `RegisterPage` class which is not surprising considering `RegisterPage` was created based on this view and view model. Let's see if we can take advantage of view models to simplify our page objects.

I have created a new `Page` superclass; but a generic one. You can see the code [here](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/Framework/Page%601.cs): 

	public class Page<TViewModel> : Page where TViewModel: class, new()
    {
        public void FillWith(TViewModel viewModel, IDictionary<Type, Func<object, string>> propertyTypeHandling = null)
        {	
          // removed for brevity
        }
    }
    
The `Page<TViewModel>` class subclasses the old `Page` class and provides all it's functionality; but it also has one extra method called `FillWith` which fills in the page with provided view model instance! So now my `RegisterPage` class looks like:

    public class RegisterPage : Page<RegisterModel>
    {
        public HomePage CreateValidUser(RegisterModel model)
        {
            FillWith(model);
            return NavigateTo<HomePage>(By.CssSelector("input[type='submit']"));
        }
    }

<small>I duplicated all page objects to show both variations and also to make the codebase easier to follow for you; but in reality you will need one class for each page object.</small>

After converting my page objects to [generic ones](https://github.com/MehdiK/MaintainableUiTesting/tree/master/src/MvcMusicStore.FunctionalTests/StronglyTypedPageObjects) now [the test](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/StronglyTypedPageObjectTest.cs) looks like:

    public class StronglyTypedPageObjectWithComponent
    {
        [Test]
        public void Can_buy_an_Album_when_registered()
        {
            var orderedPage = HomePage.Initiate()
                .GoToAdminForAnonymousUser()
                .GoToRegisterPage()
                .CreateValidUser(ObjectMother.CreateRegisterModel())
                .SelectGenreByName("Disco")
                .SelectAlbumByName("Le Freak")
                .AddAlbumToCart()
                .Checkout()
                .SubmitShippingInfo(ObjectMother.CreateShippingInfo(), "Free");

            Assert.AreEqual("Checkout Complete", orderedPage.Title);
        }
    }

That's it - the entire test! A lot more readable, DRY and maintainable, isn't it?

The `ObjectMother` class that I am using in the test is an [Object Mother](http://martinfowler.com/bliki/ObjectMother.html) that provides test data (code can be found [here](https://github.com/MehdiK/MaintainableUiTesting/blob/master/src/MvcMusicStore.FunctionalTests/StronglyTypedPageObjectTest.cs#L26
)), nothing fancy:

    public class ObjectMother
    {
        public static Order CreateShippingInfo()
        {
            var shippingInfo = new Order
            {
                FirstName = "Homer",
                LastName = "Simpson",
                Address = "742 Evergreen Terrace",
                City = "Springfield",
                State = "Kentucky",
                PostalCode = "123456",
                Country = "United States",
                Phone = "2341231241",
                Email = "chunkylover53@aol.com"
            };

            return shippingInfo;
        }

        public static RegisterModel CreateRegisterModel()
        {
            var model = new RegisterModel
            {
                UserName = "HJSimpson",
                Email = "chunkylover53@aol.com",
                Password = "!2345Qwert",
                ConfirmPassword = "!2345Qwert"
            };
            
            return model;
        }
    }

### Don't stop at page object
Some web pages are very big and complex. Earlier I said test code is code and we should treat it as such. We normally break big and complex web pages into smaller and, in some cases, reusable (partial) components. This allows us to compose a web page from smaller, more manageable components. We should do the same for our tests. To do this we can use Page Components. 

A Page Component is pretty much like a Page Object: it's a class that encapsulates interaction with some elements on a page. The difference is that it interacts with a small part of a web page: it models a user control or a partial view, if you will. A good example for a page component is a menu bar. A menu bar usually appears on all pages of a web application. You don't really want to keep repeating the code required to interact with the menu in every single page object. Instead you can create a menu page component and use it from your page objects. You could also use page components to deal with grids of data on your pages, and to take it a step further the grid page component itself could be composed of grid row page components. In the case of Mvc Music Store we could have a `TopMenuComponent` and a `SideMenuComponent` and use them from our `HomePage`.

Like in your web application, you could also create a, say, `LayoutPage` page object which models your layout/master page and use that as a superclass for all your other page objects. The layout page would then be composed of menu page components so all pages can hit the menus. I guess a good rule of thumb would be to have a page component per partial view, a layout page object per layout and a page object per web page. That way you know your test code is as granualar and well composed as your code.

## Some Frameworks For UI Testing
What I showed above was a very simple and contrived sample with a few [supporting classes](https://github.com/MehdiK/MaintainableUiTesting/tree/master/src/MvcMusicStore.FunctionalTests/Framework) as infrastructure for tests. In reality the requirements for UI testing are a lot more complex than that: there are complex controls and interactions, you have to write to and read from your pages, you have to deal with network latencies and have control over AJAX and other Javascript interactions, need to fire off different browsers and so on which I didn't explain in this article. Although it's possible to code around all these, using some frameworks could save you a lot of time. Here are the frameworks that I highly recommend:

**Frameworks for .Net:**

 - [Seleno](https://github.com/TestStack/TestStack.Seleno) is an open source project from [TestStack](http://teststack.net/) which helps you write automated UI tests with Selenium. It focuses on the use of Page Objects and Page Components and by reading from and writing to web pages using strongly typed view models. If you liked what I did in this article, then you will also like Seleno as most of the code shown here was borrowed from the Seleno codebase.
 - [White](https://github.com/TestStack/White) is an open source framework from [TestStack](http://teststack.net/) for automating rich client applications based on Win32, WinForms, WPF, Silverlight and SWT (Java) platforms.
 
Disclosure: I am a co-founder and a member of the development team in the TestStack organization.

**Frameworks for Ruby:**
 
 - [Capybara](https://github.com/jnicklas/capybara) is an acceptance test framework for web applications that helps you test web applications by simulating how a real user would interact with your app. 
 - [Poltergeist](https://github.com/jonleighton/poltergeist) is a driver for Capybara. It allows you to run your Capybara tests on a headless WebKit browser, provided by [PhantomJS](http://www.phantomjs.org/).
 - [page-object](https://github.com/cheezy/page-object) (I haven't personally used this gem) is a simple gem that assists in creating flexible page objects for testing browser based applications. The goal is to facilitate creating abstraction layers in your tests to decouple the tests from the item they are testing and to provide a simple interface to the elements on a page. It works with both watir-webdriver and selenium-webdriver. 
 
##Conclusion
We started with a typical UI automation experience, explained why UI testing fails, provided an example of a brittle test and discussed it's issues and resolved them using a few ideas and patterns. 

If you want to take one point from this article it should be: **Test Code Is Code**. If you think about it, all I did in this article was to apply the good coding and object oriented practices you already know on a UI test.

There is still a lot to learn about UI testing and I will try to discuss some of more advanced tips in a future article. 

Happy Testing!
