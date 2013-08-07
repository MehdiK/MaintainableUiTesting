﻿using MvcMusicStore.FunctionalTests.StronglyTypedPageObjects;
using MvcMusicStore.Models;
using NUnit.Framework;

namespace MvcMusicStore.FunctionalTests
{
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

}
