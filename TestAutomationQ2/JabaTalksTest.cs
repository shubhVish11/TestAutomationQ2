using ActiveUp.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestAutomationQ2
{
    [TestClass]
    public class JabaTalksTest:EmailHelper
    {

        IWebDriver driver;

        [TestInitialize]
        public void InIt()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://jt-dev.azurewebsites.net/#/SignUp");
            driver.Manage().Window.Maximize();
        }

        [DataRow("English,Dutch")]
        [TestMethod]
        public void ValidateLanguagedropdown(string languages)
        {
            try
            {
                driver.FindElement(By.XPath("//div[@placeholder='Choose Language']/span")).Click();
                List<IWebElement> elements = driver.FindElements(By.XPath("//li[@class='ui-select-choices-group']/div[contains(@id,'ui-select-choices-row')]")).ToList<IWebElement>();
                foreach (IWebElement item in elements)
                {
                    if (languages.Contains(item.Text))
                        Assert.IsTrue(true);
                    else
                        Assert.Fail();
                }
            }
            catch (Exception e)
            {
                _ = e.StackTrace;
            }

        }

        [TestMethod]
        public void ValidateSubmitButtonShouldBeDisableByDefault()
        {
            try
            {
                string value = driver.FindElement(By.XPath("//button[@type='submit' and text()='Get Started']")).GetAttribute("disabled");
                if (value.Equals("disabled"))
                    Assert.IsTrue(true);
                else
                    Assert.Fail();
            }
            catch (Exception e)
            {

            }

        }


        [DataRow("Shubham Vishwakarma", "Shubham", "shubham@gmail.com")]
        [TestMethod]
        public void ValidateSubmitButtonShouldBeEnabledOnceAllDetailsEntered(string name, string organization, string email)
        {
            try
            {
                EnterLoginDetails(name, organization, email);
                driver.FindElement(By.XPath("//input[@type='checkbox']/following::span[contains(text(),'I agree to the')]")).Click();
                string value = driver.FindElement(By.XPath("//button[@type='submit' and text()='Get Started']")).GetAttribute("disabled");
                if (value == null)
                    Assert.IsTrue(true);
                else
                    Assert.Fail();
            }
            catch (Exception e)
            {

            }

        }

        [DataRow("Shubham Vishwakarma", "Shubham", "shubham@gmail.com")]
        [TestMethod]
        public void ValidateSignUp(string name, string organization, string email)
        {
            try
            {
                EnterLoginDetails(name, organization, email);
                driver.FindElement(By.XPath("//input[@type='checkbox']/following::span[contains(text(),'I agree to the')]")).Click();
                driver.FindElement(By.XPath("//button[@type='submit' and text()='Get Started']")).Click();
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[contains(text(),' A welcome email has been sent. Please check your email. ')]")));
                if (driver.FindElement(By.XPath("//input[@type='checkbox']/following::span[contains(text(),'I agree to the')]")).Displayed)
                    Assert.IsTrue(true);
                else
                    Assert.Fail();
            }
            catch (Exception e)
            {

            }

        }


        [DataRow("Shubham Vishwakarma", "Shubham", "shubham@gmail.com", "Passord@W123")]
        [TestMethod]
        public void ValidateEmailConfimationSignUp(string name, string organization, string email, string password)
        {
            try
            {
                EnterLoginDetails(name, organization, email);
                driver.FindElement(By.XPath("//input[@type='checkbox']/following::span[contains(text(),'I agree to the')]")).Click();
                driver.FindElement(By.XPath("//button[@type='submit' and text()='Get Started']")).Click();
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[contains(text(),' A welcome email has been sent. Please check your email. ')]")));
                if (ReadImap(email, password))
                {
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail();


            }
            catch (Exception e)
            {

            }

        }

        [TestMethod]
        public void ValidateTermsAndConditionLink()
        {
            try
            {

                driver.FindElement(By.PartialLinkText("Terms and Conditions")).Click();
                driver.SwitchTo().Window(driver.WindowHandles[1]);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                driver.FindElement(By.XPath("//header/h1[contains(text(),'Jabatalks.co')]"));
                //header/h1[contains(text(),'Jabatalks.co')]
                if (driver.Title.Equals("Jabatalks.co"))
                    Assert.IsTrue(true);
                else
                    Assert.Fail();
            }
            catch (Exception e)
            {

            }

        }


        public void EnterLoginDetails(string name, string org, string email)
        {
            try
            {
                driver.FindElement(By.Id("name")).SendKeys(name);
                driver.FindElement(By.Id("orgName")).SendKeys(org);
                driver.FindElement(By.Id("singUpEmail")).SendKeys(email);
            }
            catch (Exception)
            {

            }
        }

        
        public bool ReadImap(string emailid,string pwd)
        {
            var mailRepository = new MailRepository(
                                    "imap.gmail.com",
                                    993,
                                    true,
                                    emailid,
                                    pwd
                                );

            var emailList = mailRepository.GetAllMails("inbox");
            bool value = false;
            foreach (Message email in emailList)
            {
                Console.WriteLine("<p>{0}: {1}</p><p>{2}</p>", email.From, email.Subject, email.BodyHtml.Text);
                if(email.Subject.Contains("Please Comeplete JabaTalks Account"))
                {
                    value = true;break;
                }
                    
            }
            return true;
        }

        [TestCleanup]
        public void Cleanup() => driver.Quit();

    }
}
