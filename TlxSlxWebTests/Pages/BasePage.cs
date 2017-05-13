using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TlxSlxWebTests.Pages
{
    public abstract class BasePage : IDisposable
    {
        public IWebDriver myDriver;

        public readonly double ShortTimeout = 5;

        public double DriverDefaultTimeout = 15;

        public string BaseUrl { get; set; }

        #region Constructors

        public BasePage(IWebDriver aDriver)
        {
            myDriver = aDriver;
            BaseUrl = myDriver.Url;
        }

        #endregion

        protected void WaitForElement(By byEle, double seconds = 5)
        {
            WebDriverWait wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(seconds));
            wait.Until((d) => { return IsElementPresent(byEle); });
        }

        protected void WaitForElementToBeVisible(By byEle, double seconds = 5)
        {
            WebDriverWait wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(seconds));
            wait.Until((d) => { return IsElementPresent(byEle) && IsElementVisible(byEle); });
        }


        protected void WaitForElementToNotToBeVisible(By byEle, double seconds = 5)
        {
            try { 
            WebDriverWait wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(seconds));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(byEle));
            }
            catch(Exception e)
            {
                //Do nothing
            }
        }

        protected void WaitForAnyOneElementToBeVisible(By byEle, double seconds = 5)
        {
            WebDriverWait wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(seconds));
            wait.Until((d) => {
                return myDriver.FindElements(byEle).Any(e => e!= null && e.Displayed);
            });
        }

        protected bool IsElementPresent(By by)
        {
            try
            {
                //TODO: Is Displayed be returned???
                myDriver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        protected bool IsAlertPresent()
        {
            try
            {
                myDriver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        protected string CloseAlertAndGetItsText(bool acceptNextAlert = true)
        {
            try
            {
                IAlert alert = myDriver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }

        protected string GetFieldValue(string controlId)
        {
            var byEle = By.Id(controlId);
            var element = myDriver.FindElement(byEle);
            string val = element.GetAttribute("value");
            return val;
        }

        protected void SetFieldValue(string controlId, string val)
        {
            var byEle = By.Id(controlId);
            WaitForElement(byEle);
            var element = myDriver.FindElement(byEle);
            ClickElement(byEle);
            element.Clear();
            element.SendKeys(val);
        }

        protected void ClickElement(By byEle)
        {
            try
            {
                WaitForElement(byEle);
                var element = myDriver.FindElement(byEle);
                element.Click();
                element = null;
            }
            catch (InvalidOperationException ex)
            {
                //var snoozeAllBtn = By.XPath("//td[@class='snoozeActions']/span[2]/span/span/span[contains(text(),'Snooze All')]");
                //if (IsElementVisible(snoozeAllBtn))
                //{
                //    var snoozeBtnElement = myDriver.FindElement(snoozeAllBtn);
                //    snoozeBtnElement.Click();
                //    var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(ShortTimeout));
                //    wait.Until((d) => { return IsElementVisible(snoozeAllBtn) == false; });
                //}
                //var element = myDriver.FindElement(byEle);
                //element.Click();
            }
            catch (ElementNotVisibleException ex)
            {
                var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(ShortTimeout));
                wait.Until((d) => { return IsElementVisible(byEle) == true; });
                var element = myDriver.FindElement(byEle);
                element.Click();
            }
        }

        protected bool IsElementVisible(By by)
        {
            try
            {
                var element = myDriver.FindElement(by);
                return element.Displayed;
            }
            catch
            {
                return false;
            }
        }

        protected bool GetCheckboxChecked(string ckeckboxId)
        {
            var byEle = By.Id(ckeckboxId);
            WaitForElement(byEle);
            var element = myDriver.FindElement(byEle);
            try
            {
                element.GetAttribute("checked");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected void ClickCheckbox(string ckeckboxId)
        {
            var byEle = By.Id(ckeckboxId);
            WaitForElement(byEle);
            var element = myDriver.FindElement(byEle);
            element.Click();
        }

        protected string RightClick(IWebElement element)
        {
           
             var action = new OpenQA.Selenium.Interactions.Actions(myDriver);
            action.MoveToElement(element).ContextClick(element);
            action.Build().Perform();
            return null;
           
        }

        protected void PressControlF5()
        {
            //TODO: Decide whether this method is required or not.

            //var byEle = By.Id("imgLogo");
            //ClickElement(byEle);
            //Actions actionObject = new Actions(myDriver);
            //actionObject.KeyDown(Keys.Control)
            //    .SendKeys(Keys.F5)
            //    .KeyUp(Keys.Control)
            //    .Build();
            //actionObject.Perform();
        }

        protected bool GetFieldIsEditable(string filedId)
        {
            var byEle = By.Id(filedId);
            var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
            wait.Until((d) => { return IsElementVisible(byEle); });
            try
            {
                Thread.Sleep(1500);
                ClickElement(byEle);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected void KillLeavePagePopup()
        {
            try
            {
                var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(ShortTimeout));
                wait.Until((d) => { return IsAlertPresent(); });
            }
            catch (Exception)
            {
                //Do nothing as this can be disable globally.
            }
            if (IsAlertPresent())
            {
                LogWriter.Write("Got LeavePagePopup - Closing with yes");
                CloseAlertAndGetItsText();
            }
        }

        protected void RefreshScreen()
        {
            myDriver.Navigate().Refresh();
            KillLeavePagePopup();
        }

        protected void ClickButton(string buttonId)
        {
            var byEle = By.Id(buttonId);
            WaitForElement(byEle);

            try
            {
                myDriver.FindElement(byEle).Click();
            }
            catch (Exception e)
            {
                Thread.Sleep(500);
                myDriver.FindElement(byEle).Click();
            }
        }

        protected void AcceptAlert()
        {
            IAlert alert = myDriver.SwitchTo().Alert();
            alert.Accept();
        }

        protected void ScrollToEnd(IWebElement element)
        {
           
            Actions action = new Actions(myDriver);
            try
            {
                action.Click(element).KeyDown(Keys.Control).SendKeys(Keys.End).KeyUp(Keys.Control).Perform();
            }
            catch(Exception e)
            {
                //Do Nothing
            }
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                myDriver = null;
                BaseUrl = string.Empty;
            }
        }
    }
}
