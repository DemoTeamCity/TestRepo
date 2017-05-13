using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TlxSlxWebTests.Pages.TlxPages
{
    public class IcrmLoginPage : TlxBasePage
    {
        private const string USERNAME_CONTROL_ID = "UserName";
        private const string PASSWORD_CONTROL_ID = "Password";
        private const string SIGNIN_BUTTON_ID = "btnLogin";

        public IcrmLoginPage(IWebDriver aDriver) : base(aDriver)
        {

        }

        public IcrmAccountsPage Login(string userName, string password = "")
        {
            SetFieldValue(USERNAME_CONTROL_ID, userName);
            SetFieldValue(PASSWORD_CONTROL_ID, password);

            ClickButton(SIGNIN_BUTTON_ID);

            //  WaitForLoader();
            //WaitForDGridLoader();
            WaitForElementToBeVisible(By.CssSelector("div[id^='listGrid-row-']"),15);

            return new IcrmAccountsPage(myDriver);
        }

        public bool IsLoginSuccessful()
        {
            return !IsElementPresent(By.ClassName("failureTextStyle"));
        }
    }
}
