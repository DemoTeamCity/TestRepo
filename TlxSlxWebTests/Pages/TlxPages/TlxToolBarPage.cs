using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace TlxSlxWebTests.Pages.TlxPages
{
    public class TlxToolBarPage : TlxBasePage
    {
        public TlxToolBarPage(IWebDriver webDriver) : base(webDriver)
        {
            
        }

        public void SelectToolMenu(string mainMenuText, string subMenuText)
        {
            WaitForElementToBeVisible(By.Id("inforLogoPanel"));
            var mainMenuElement = SelectMainToolMenu(mainMenuText);

            SelectSubToolMenu(mainMenuElement.GetAttribute("id"), subMenuText);
        }

        private IWebElement SelectMainToolMenu(string menuText)
        {
            
            var toolBarElement = myDriver.FindElement(By.Id("ToolBar"));
            WaitForElementToBeVisible(By.Id("ToolBar"));
            var menuElement = toolBarElement.FindElements(By.TagName("div")).Single(me => me.Text.Equals(menuText));

            menuElement.Click();

            return menuElement;
        }

        private void SelectSubToolMenu(string mainMenuID, string subMenuText)
        {
            WaitForElementToBeVisible(By.Id(mainMenuID + "_dropdown"));

            var toolBarSubMenu = myDriver.FindElement(By.Id(mainMenuID + "_dropdown"));

            var subMenuElement = toolBarSubMenu.FindElements(By.TagName("tr")).Single(menuElement => menuElement.Text.Equals(subMenuText));

            subMenuElement.Click();

            WaitForLoader();
        }
    }
}