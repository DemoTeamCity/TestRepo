using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Threading;


namespace TlxSlxWebTests.Pages.TlxPages
{
    public class TlxBasePage : BasePage 
    {
        private string LOOKUP_SEARCH_ICON_ID = "GroupLookupButton";
        
        private string MENU = "_menu";

        public IcrmGridPage SearchResultGrid { get; set; }

        public TlxToolBarPage ToolBar { get; set; }

        public TlxBasePage(IWebDriver aDriver) : base(aDriver)
        {
            
        }

        protected bool GetCheckboxChecked(string dialogId, string ckeckboxId)
        {
            var chkId = string.Format("DialogWorkspace_{0}_{1}", dialogId, ckeckboxId);
            return GetCheckboxChecked(chkId);
        }

        protected void DxContextMenuClick(string tabId, string dxmenuId, string menuItemName)
        {
            string menuId = string.Format("TabControl_element_{0}_element_view_{0}_{0}_{1}", tabId, dxmenuId);
            DxContextMenuClick(menuId, menuItemName);
        }

        protected void DxContextMenuClick(string menuId, string menuItemName)
        {
            var byEle = By.Id(menuId);
            WaitForElement(byEle);
            byEle = By.XPath(string.Format("//div[@id='{0}']/table/tbody/tr/td/table/tbody/tr", menuId));
            WaitForElement(byEle);
            var elements = myDriver.FindElements(byEle);
            IWebElement returnItem = null;
            foreach (var item in elements)
            {
                var innerText = item.GetAttribute("innerText");
                //LogWriter.Write(innerText);
                if (innerText.Contains(menuItemName))
                {
                    returnItem = item;
                    break;
                }
            }
            returnItem.Click();
        }

        protected void ClickCheckbox(string dialogId, string ckeckboxId)
        {
            var chkId = string.Format("DialogWorkspace_{0}_{1}", dialogId, ckeckboxId);
            ClickCheckbox(chkId);
        }

        protected void ChooseFirstRowOfLookup(string lookupFullName, string searchCondition = "")
        {
            var byEle = By.Id(string.Format("{0}_LookupBtn", lookupFullName));
            ClickElement(byEle);
            if (!string.IsNullOrEmpty(searchCondition))
            {
                string inputField = string.Format("{0}_Lookup-ConditionManager-SearchCondition0-TextValue", lookupFullName);
                SetFieldValue(inputField, searchCondition);
            }

            byEle = By.Id(string.Format("{0}_Lookup-ConditionManager-Search_label", lookupFullName));
            myDriver.FindElement(byEle).Click();
            byEle = By.XPath(string.Format("//div[@id='{0}_Lookup-Grid']/div[2]/div/div/div/div/div/table/tbody/tr/td[2]",
                lookupFullName));
            WaitForElement(byEle);
            var element = myDriver.FindElement(byEle);
            var action = new OpenQA.Selenium.Interactions.Actions(myDriver);
            action.DoubleClick(element);
            action.Perform();
            byEle = By.Id(string.Format("{0}_Lookup-Dialog", lookupFullName));
            var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
            wait.Until((d) => { return IsElementVisible(byEle) == false; });
        }

        protected string GetDojoCheckedValue(string controlId)
        {
            var byEle = By.Id(controlId);
            var element = myDriver.FindElement(byEle);
            string val = element.GetAttribute("aria-checked");
            return val;
        }

        protected string GetDojoStatusValue(string controlId)
        {
            var byEle = By.Id(controlId);
            var element = myDriver.FindElement(byEle);
            string val = element.GetAttribute("aria-disabled");
            return val;
        }

        protected void ClearLookup(string lookupFullName)
        {
            try
            {
                string lookupTbId = string.Format("{0}_LookupText", lookupFullName);
                var byEle = By.Id(string.Format("{0}_btnClearResult", lookupFullName));
                ClickElement(byEle);

                var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(ShortTimeout));
                wait.Until((d) => { return GetFieldValue(lookupTbId) == ""; });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected void ClickButton(string buttonId, bool withloadercheck = false)
        {
            if (withloadercheck)
            {
                var loaderEle = By.Id("loader");
                var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
                wait.Until((d) => { return IsElementVisible(loaderEle) == false; });
            }

            //ClickButton(buttonId);
            base.ClickButton(buttonId);
        }

        protected void WaitForLoaderToBe(bool isPresent)
        {
            var loaderEle = By.Id("loader");
            //var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(15));
            //wait.Until((d) => { return IsElementVisible(loaderEle) == isPresent; });
            WaitForElement(loaderEle);
            var element = myDriver.FindElement(loaderEle);
            try
            {
                var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
                wait.Until((d) => { return element.Displayed == isPresent; });
            }
            catch (StaleElementReferenceException e)
            {

            }
        }

        protected void WaitForLookUpLoadingDataToBe(bool isPresent)
        {
            var loaderEle = By.ClassName("dgrid-loading");
            var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(30));
            wait.Until((d) => { return IsElementVisible(loaderEle) == isPresent; });
        }

        protected string GetDojoDialogTextAndClose()
        {
            var byEle = By.Id("queryDialog");
            WebDriverWait wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
            wait.Until((d) => { return IsElementVisible(byEle); });
            byEle = By.Id("queryDialog-questionDiv");
            var element = myDriver.FindElement(byEle);
            var contents = element.Text;
            byEle = By.XPath("//div[@id='queryDialog-buttonDiv']/span[@widgetid='qry_yesButton']");
            myDriver.FindElement(byEle).Click();
            byEle = By.Id("queryDialog");
            wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(3));
            wait.Until((d) => { return IsElementVisible(byEle) == false; });
            return contents;
        }

        protected void MoveMainContentSplitter()
        {
            var byEle = By.Id("mainContentDetails_splitter");
            WaitForElement(byEle);
            var element = myDriver.FindElement(byEle);
            var point = element.Location;
            //LogWriter.Write("x : " + point.X);
            //LogWriter.Write("y : " + point.Y);
            if (point.Y > 300)
            {
                var action = new OpenQA.Selenium.Interactions.Actions(myDriver);
                action.DragAndDropToOffset(element, 0, -150);
                action.Perform();

            }
        }

        protected void RenderTab(string tabId)
        {
            By byEle;
            WebDriverWait wait;
            IWebElement element;
            try
            {

                byEle = By.XPath(string.Format("//div[@class='tws-main-section']/div/ul/li[@id='show_{0}']", tabId));
                wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(ShortTimeout));
                wait.Until((d) => { return IsElementPresent(byEle); });
                element = myDriver.FindElement(byEle);
                bool active = element.GetAttribute("class").ToLower().Contains("tws-active-tab-button");
                if (!active) element.Click();
            }
            catch (Exception ex)
            {
                //element not found in the main tab so we have to look in the MoreTabs
                byEle = By.XPath("//div[@class='tws-main-section']/div/ul/li[@id='show_More']");
                wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(ShortTimeout));
                wait.Until((d) => { return IsElementPresent(byEle); });
                element = myDriver.FindElement(byEle);
                element.Click();
                byEle = By.XPath(string.Format("//div[@class='tws-more-tab-buttons-container']/ul/li[@id='more_{0}']", tabId));
                wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(ShortTimeout));
                wait.Until((d) => { return IsElementPresent(byEle); });
                element = myDriver.FindElement(byEle);
                bool active = element.GetAttribute("class").ToLower().Contains("tws-active-tab-button");
                if (!active) element.Click();

            }
            //wait for the tab to finish loading
            byEle = By.XPath(string.Format("//div[@id='element_{0}']/div/div/div[@class='tws-tab-view-body']", tabId));
            wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
            wait.Until((d) => { return IsElementPresent(byEle); });
            element = myDriver.FindElement(byEle);
            try
            {
                wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
                wait.Until((d) => { return element.Text.StartsWith("Loading...") == false; });
            }
            catch (StaleElementReferenceException)
            {
                LogWriter.Write("catch for Loading element...");
            }
        }

        protected int GetRowCountOfDxGrid(string tabId, string gridName)
        {
            var byEle = By.XPath(string.Format("//table[@id='TabControl_element_{0}_element_view_{0}_{0}_{1}_DXMainTable']/tbody/tr[1]", tabId, gridName));
            WaitForElement(byEle);
            try
            {
                byEle = By.XPath(string.Format("//table[@id='TabControl_element_{0}_element_view_{0}_{0}_{1}_DXMainTable']/tbody/tr[contains(@class,'dxgvDataRow_')]", tabId, gridName));
                WaitForElement(byEle, ShortTimeout);
                return myDriver.FindElements(byEle).Count;
            }
            catch (Exception)
            {
                LogWriter.Write("No DataRows found");
                return 0;
            }
        }

        protected List<string> GetDisplayedColumns(string tabId, string gridName)
        {
            var byEle = By.XPath(string.Format("//table[@id='TabControl_element_{0}_element_view_{0}_{0}_{1}_DXHeaderTable']/tbody/tr[contains(@id,'_DXHeadersRow')]", tabId, gridName));
            WaitForElement(byEle);
            var element = myDriver.FindElement(byEle);
            //LogWriter.Write(element.Text);
            //LogWriter.Write(element.GetAttribute("innerHTML"));
            //LogWriter.Write(element.GetAttribute("innerText"));
            string text = element.GetAttribute("innerText");
            string[] arr = text.Split('\n');
            var retList = new List<string>();
            for (int i = 0; i < arr.Length; i++)
            {
                retList.Add(arr[i].TrimEnd());
            }
            return retList;
        }

        protected IWebElement GetGridRowByColumnData(string tabId, string gridId, int columnIndex, string columnValue)
        {
            var byEle = By.XPath(string.Format("//table[@id='TabControl_element_{0}_element_view_{0}_{0}_{1}_DXMainTable']/tbody/tr[1]", tabId, gridId));
            WaitForElement(byEle);
            try
            {
                byEle = By.XPath(string.Format("//table[@id='TabControl_element_{0}_element_view_{0}_{0}_{1}_DXMainTable']/tbody/tr[contains(@class,'dxgvDataRow_')]/td[{2}]", tabId, gridId, columnIndex + 1));
                WaitForElement(byEle, ShortTimeout);
                var elements = myDriver.FindElements(byEle);
                IWebElement returnItem = null;
                foreach (var item in elements)
                {
                    var innerText = item.GetAttribute("innerText");
                    //LogWriter.Write(innerText);
                    if (innerText.Contains(columnValue))
                    {
                        returnItem = item;
                        break;
                    }
                }
                return returnItem;
            }
            catch (Exception)
            {
                LogWriter.Write("No Cell Found");
                return null;
            }
        }

        protected void WaitForDialogWorkspaceWindowToShow()
        {
            var byEle = By.Id("DialogWorkspace_window");
            WaitForElement(byEle);
        }

        protected void WaitForDialogWorkspaceWindowToHide()
        {
            var byEle = By.Id("DialogWorkspace_window");
            WaitForElement(byEle);
            var element = myDriver.FindElement(byEle);
            try
            {
                var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
                wait.Until((d) => { return element.Displayed == false; });
            }
            catch (StaleElementReferenceException e)
            {

            }
        }

        protected void ClickDialogWorkspaceWindowButton(string dialogId, string btnId)
        {
            var btnName = string.Format("DialogWorkspace_{0}_{1}", dialogId, btnId);
            ClickButton(btnName);
        }

        protected void WaitforAsyncPostbackIndicatorTo(bool toVisible)
        {
            var byEle = By.Id("asyncpostbackindicator");
            WaitForElement(byEle);
            var element = myDriver.FindElement(byEle);
            try
            {
                var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
                wait.Until((d) => { return element.Displayed == toVisible; });
            }
            catch (StaleElementReferenceException e)
            {

            }
        }

        protected void WaitForTimeGridInternalTabLoader()
        {
            try
            {
                WaitForTimeGridInternalTabLoaderToBe(true);
                WaitForTimeGridInternalTabLoaderToBe(true);
            }
            catch (Exception)
            {
                //no problem. Just continue.
            }
        }

        protected void WaitForTimeGridInternalTabLoaderToBe(bool toVisible)
        {
            var byEle = By.Id("grdAccountProjects_TL");
            WaitForElement(byEle);
            //myDriver.SwitchTo().Frame(myDriver.FindElement(By.Id("MainContent_frmTLXTimeGrid_frameTimeGrid")));
            var element = myDriver.FindElement(byEle);
            try
            {
                var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
                wait.Until((d) => { return element.Displayed == toVisible; });
            }
            catch (StaleElementReferenceException e)
            {

            }
        }

        protected void WaitForDGridLoaderToBe(bool toVisible)
        {
            var byEle = By.ClassName("dgrid-loading");
            WaitForElement(byEle);

            var element = myDriver.FindElement(byEle);
            try
            {
                var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(DriverDefaultTimeout));
                wait.Until((d) => { return element.Displayed == toVisible; });
            }
            catch (StaleElementReferenceException e)
            {

            }
        }

        protected void WaitForDGridLoader()
        {
            try
            {
                WaitForDGridLoaderToBe(true);
                WaitForDGridLoaderToBe(false);
            }
            catch (Exception)
            {
                //no problem. Just continue.
            }
        }

        protected void WaitForLoader()
        {
            try
            {
                WaitForLoaderToBe(true);
                WaitForLoaderToBe(false);
            }
            catch (Exception)
            {
                //no problem. Just continue.
            }
        }

        protected void WaitForLookUpLoadingData()
        {
            try
            {
                WaitForLookUpLoadingDataToBe(true);
                WaitForLookUpLoadingDataToBe(false);
            }
            catch (Exception)
            {
                //no problem. Just continue.
            }
        }

        protected void WaitForTabLoadingToBe(bool isPresent)
        {
            var loaderEle = By.ClassName("tws-tab-view-loading");
            var wait = new WebDriverWait(myDriver, TimeSpan.FromSeconds(10));
            wait.Until((d) => { return IsElementVisible(loaderEle) == isPresent; });
        }

        protected void WaitForTabLoading()
        {
            try
            {
                WaitForTabLoadingToBe(true);
                WaitForTabLoadingToBe(false);
            }
            catch (Exception)
            {
                //no problem. Just continue.
            }
        }

        protected void WaitForAsyncPostbackIndicator()
        {
            try
            {
                WaitforAsyncPostbackIndicatorTo(true);
                WaitforAsyncPostbackIndicatorTo(false);
            }
            catch (Exception)
            {
                //no problem. Just continue.
            }
        }

        protected void SetDojoTimeField(string controlId, string val)
        {
            var byEle = By.XPath(string.Format("//div[@id='widget_{0}']/div", controlId));
            ClickElement(byEle);
            string popInput = string.Format("{0}_Popout-TimeTextBox", controlId);
            SetFieldValue(popInput, val);
            byEle = By.Id(popInput);
            ClickElement(byEle);
            byEle = By.XPath(string.Format("//span[@widgetid='{0}_Popout-OKButton']", controlId));
            ClickElement(byEle);
        }

        protected List<IWebElement> GetAllMainTabs()
        {
            return myDriver.FindElements(By.ClassName("dijitAccordionInnerContainer")).ToList();
        }

        protected void SelectItemFromDropDown(string dropDownId, string itemToSelect)
        {
            //click the dropdown control first
            //ClickElement(By.Id(dropDownId));
            myDriver.FindElement(By.Id(dropDownId)).SendKeys(Keys.ArrowDown);
           
           WaitForElementToBeVisible(By.Id(dropDownId + MENU));

            myDriver.FindElement(By.Id(dropDownId + MENU)).
                FindElements(By.TagName("tr")).
                Single(menuItems => menuItems.Text.ToLower().Equals(itemToSelect.ToLower())).
                Click();
        }

        protected void LookupSearchBy(List<Tuple<LookUpSearchInput>> searchConditions)
        {
            ClickButton(LOOKUP_SEARCH_ICON_ID);

            FillConditionsForLookUp(searchConditions);
            
            WaitForAsyncPostbackIndicator();
            WaitForDGridLoader();

            SearchResultGrid = new IcrmGridPage(myDriver);
        }

        protected override void Dispose(bool disposing)
        {
            if (SearchResultGrid != null)
            {
                SearchResultGrid.Dispose();
            }

            if(ToolBar != null)
            {
                ToolBar.Dispose();
            }

            base.Dispose(disposing);
        }

        protected void DoubleClick(IWebElement element)
        {
            Actions action = new Actions(myDriver);
            action.DoubleClick(element);
            action.Perform();

        }

        public virtual TlxBasePage NavigateMainTabTo(string tabName)
        {
            var requiredTab = GetAllMainTabs().Single(e => e.Text.ToUpper().Equals(tabName.ToUpper()));

            requiredTab.Click();

            return this;
        }

        public virtual bool IsMainTabExpanded(string tabName)
        {
            var requiredTab = GetAllMainTabs().Single(e => e.Text.ToUpper().Contains(tabName.ToUpper()));
            requiredTab = requiredTab.FindElement(By.ClassName("dijitAccordionTitleFocus"));

            var isExpanded = Convert.ToBoolean(requiredTab.GetAttribute("aria-expanded"));

            return isExpanded;
        }

        public virtual bool IsMainTabExpanded(IWebElement tabElement)
        {
            var requiredTab = tabElement.FindElement(By.ClassName("dijitAccordionTitleFocus"));

            var isExpanded = Convert.ToBoolean(requiredTab.GetAttribute("aria-expanded"));

            return isExpanded;
        }

        public TlxBasePage SelectMainMenu(string menuName)
        {
            TlxBasePage requiredPage = null;

            var currentExpandedMainTab = GetAllMainTabs().Single(mainTab => IsMainTabExpanded(mainTab));
            //var listOfMainMenu = currentExpandedMainTab.FindElements(By.TagName("li"));
            //var clickElement = listOfMainMenu.Single(menuElement => menuElement.Text.ToLower().Equals(menuName.ToLower()));
            //clickElement.Click();
            //WaitForLoader();
           
            //WaitForDGridLoader();
            switch (menuName.ToLower())
            {
                case "projects":
                    currentExpandedMainTab.FindElement(By.Id("navTlxProjects")).Click();

                    //WaitForLoader();
                    WaitForDGridLoader();
                    WaitForElementToBeVisible(By.CssSelector("div[id^='listGrid-row-']"), 15);
                    requiredPage = new IcrmProjectsPage(myDriver);
                    break;

                case "tasks":
                    currentExpandedMainTab.FindElement(By.Id("navTlxProjectTasks")).Click();

                    //WaitForLoader();
                    WaitForDGridLoader();
                    WaitForElementToBeVisible(By.CssSelector("div[id^='listGrid-row-']"), 15);
                    requiredPage = new IcrmTaskPage(myDriver);
                    break;

                case "timegrid":
                    currentExpandedMainTab.FindElement(By.Id("navTLXTimeGrid")).Click();

                    //WaitForLoader();
                    myDriver.SwitchTo().Frame(myDriver.FindElement(By.Id("MainContent_frmTLXTimeGrid_frameTimeGrid")));

                    WaitForElementToBeVisible(By.Id("ASPxPageControl2"),15);
                    //WaitForTimeGridLoader();
                    myDriver.SwitchTo().DefaultContent();

                    requiredPage = new IcrmTaskPage(myDriver);
                    break;

                case "webcalendar":
                    currentExpandedMainTab.FindElement(By.Id("navWebCalender")).Click();
                    WaitForElementToBeVisible(By.ClassName("mainContentHeaderTable"));
                    requiredPage = new TlxBasePage(myDriver);
                    break;



            }

            return requiredPage;
        }

        public void LookupField(List<Tuple<LookUpSearchInput>> searchConditions, int rowNumberToClick = 0)
        {
            //fill all conditions
            FillConditionsForLookUp(searchConditions);

            //wait for search results
            WaitForAnyOneElementToBeVisible(By.CssSelector("div[id*='_Lookup-Grid-row']"),15);

            //get all search result row elements
            myDriver.FindElements(By.CssSelector("div[id*='_Lookup-Grid-row']")).Where(e => e.Displayed == true).ElementAt(rowNumberToClick).Click();

            //click Okay Button
            myDriver.FindElements(By.CssSelector("span[id$='Lookup-GridSelectButton'")).Single(e => e.Displayed == true).Click();
        }

        protected IWebElement ClickTab(string tabName)
        {
            //get the main tab control
            var tabContainerElement = myDriver.FindElement(By.Id("ContentPlaceHolderMain_TabControl"));

            //look for displayed tab elements. Middle and active tab in Main section is covered.
            var activeTabElements = tabContainerElement.FindElements(By.CssSelector("div[id^='element_'")).Where(e => e.Displayed == true).ToList();
            var foundTabs = activeTabElements.Where(tabElement => tabElement.FindElement(By.ClassName("tws-tab-view-title")).Text.Equals(tabName)).ToList();

            //if the tab is visible, it must be ONE. 
            if (foundTabs.Count == 1)
            {

                return foundTabs.Single();

            }

            //here, the tab is not available in middle section and the active tab in main section 
            //hence check whether the tab is in main section, if found, click it.
            var tabToClick = GetTabToClick(tabName, "show_", tabContainerElement);
            if(tabToClick != null)
            {
                //activate the tab
                tabToClick.Click();
                WaitForTabLoading();
                //wait for the tab to be visible
                string tabElementId = tabToClick.GetAttribute("id").Replace("show_", "element_");
                WaitForElementToBeVisible(By.Id(tabElementId));

                //return the tab
                return tabContainerElement.FindElement(By.Id(tabElementId));
            }

            //here, the required tab is in More Tabs. Click on More Tabs and search for the required tab
            var moreTab = GetTabToClick("More Tabs", "show_", tabContainerElement);
            moreTab.Click();
            WaitForElementToBeVisible(By.Id("element_more"));

            tabToClick = GetTabToClick(tabName, "more_", tabContainerElement);
            if(tabToClick != null)
            {
                //activate the tab
                tabToClick.Click();
                WaitForTabLoading();
                WaitForAsyncPostbackIndicator();

                string tabElementId = tabToClick.GetAttribute("id").Replace("more_", "element_");
                WaitForElementToBeVisible(By.Id(tabElementId));

                //return the tab
                return tabContainerElement.FindElement(By.Id(tabElementId));
            }

            return tabToClick;
        }
            
        public void SelectMenuByRightClickingOnTab(string tabName,IWebElement tabElement, string mainMenuToSelect,string subMenuToSelect = "null")
        {
            string gridName = tabName;
            
            //Remove space between the tab name to get the menu id if space is in between the tab name
            if (tabName.Any(e => char.IsWhiteSpace(e)))
            {
               gridName = tabName.Replace(" ", "");
            }
            
            //Get the div element in which we right click
            WaitForTabLoading();

            var divElementToRightClick = tabElement.FindElement(By.CssSelector(string.Format("table[id$='_grd{0}']",gridName)));


            //IJavaScriptExecutor js = (IJavaScriptExecutor)myDriver;
            //js.ExecuteScript("var element = arguments[0];var eve = element.ownerDocument.createEvent('MouseEvents');eve.initMouseEvent('contextmenu', true, true, element.ownerDocument.defaultView, 1, 0, 0, 0, 0, false,false, false, false, 2, null)", divElementToRightClick);
           
           

           RightClick(divElementToRightClick);
            //slect the menu value from context menu
            MenuOrSubMenuSelectionFromContextMenu(tabElement, gridName, mainMenuToSelect, subMenuToSelect);

           
           // return new TlxBasePage(myDriver);
        }

        public void MenuOrSubMenuSelectionFromContextMenu(IWebElement tabElement, string gridName, string mainMenuToSelect, string subMenuToSelect = "null")
        {
            Actions action = new Actions(myDriver);
            
            //get the menu element for the required tab 
            var menuContainer = tabElement.FindElement(By.CssSelector("div[class='dxmLite dxm-ltr']"));
            var menuId = string.Format("div[id$='_mnu{0}']", gridName);
           
            //WaitForElementToBeVisible(By.CssSelector(menuId));
            var menuElement = menuContainer.FindElement(By.CssSelector(menuId));

            //wait for the menu to display
            WaitForElementToBeVisible(By.CssSelector(menuId));

            //Get the list of menu Elements
            var listOfMenuElemenets = menuElement.FindElements(By.TagName("li")).Where(element => element.Text != "").ToList();

            //if there is no submenu to slect then  perform the below step
            if (subMenuToSelect == "null")
            {
                listOfMenuElemenets.Single(menuClickElement => menuClickElement.Text.Equals(mainMenuToSelect)).Click();
                //return new TlxBasePage(myDriver);
            }
            //if there is a sub menu then perform the below steps
            else
            {

                var mainMenuElementToClick = listOfMenuElemenets.Single(menuHoverElement => menuHoverElement.Text.Equals(mainMenuToSelect));

                //perform action


                //get the menu id to get the sub menu div element
                var menuElementToClickId = mainMenuElementToClick.GetAttribute("id");

                string subMenuDivId = string.Format("div[id*='{0}']", menuElementToClickId.Replace("_DXI", "_DXM"));
                //mainMenuElementToClick.SendKeys(Keys.ArrowLeft);

                action.MoveToElement(mainMenuElementToClick).Perform();
                //wait for the submenu to be visible
                 WaitForElementToBeVisible(By.CssSelector(subMenuDivId));

                //get the list of sub menu's
                var listOfsubMenu = menuContainer.FindElement(By.CssSelector(subMenuDivId)).FindElements(By.TagName("li")).Where(element => element.Text != "").ToList();
                //get the sub menu to click
                var subMenuToClick = listOfsubMenu.Single(subMenu => subMenu.Text.Equals(subMenuToSelect));
                action.MoveToElement(myDriver.FindElement(By.CssSelector(subMenuDivId)));
                action.MoveToElement(subMenuToClick).Click().Build().Perform();
                // action.MoveToElement(subMenuToClick).Click().Perform();
            }
            WaitForAsyncPostbackIndicator();


        }

        private void FillConditionsForLookUp(List<Tuple<LookUpSearchInput>> searchConditions)
        {
            WaitForAnyOneElementToBeVisible(By.CssSelector("div[id$='-ConditionManager']"));

            var conditionManager = myDriver.FindElements(By.CssSelector("div[id$='-ConditionManager']")).Single(e => IsElementVisible(By.Id(e.GetAttribute("Id"))));
            var addButton = conditionManager.FindElement(By.CssSelector("span[id*='-AddImageButton']"));
            var searchButton = conditionManager.FindElement(By.CssSelector("span[id*='-Search_label'"));

            IWebElement conditionRow;
            for(int conditionNumber=0;conditionNumber<searchConditions.Count;conditionNumber++)
            {
                var rowCss = string.Format("div[id*='-ConditionManager-SearchCondition{0}']", conditionNumber);
                conditionRow = conditionManager.FindElement(By.CssSelector(rowCss));

                var lookUpFieldId = conditionRow.GetAttribute("id") + "_fieldName";
                var lookUpOperatorId = conditionRow.GetAttribute("id") + "_operators";
                var lookUpValueId = conditionRow.GetAttribute("id") + "-TextValue";

                searchConditions.ForEach(condition =>
                {
                    SelectItemFromDropDown(lookUpFieldId, condition.Item1.SearchBy);
                    SelectItemFromDropDown(lookUpOperatorId, condition.Item1.SearchOperator);
                    SetFieldValue(lookUpValueId, condition.Item1.Value);

                    if (conditionNumber > 0)
                    {
                        ClickButton(addButton.GetAttribute("id"));
                    }
                });
            }
            ClickButton(searchButton.GetAttribute("id"));

        }

        private IWebElement GetTabToClick(string tabName, string idStartsWith, IWebElement tabContainer)
        {
            string tabElementPartialId = string.Format("li[id^='{0}']", idStartsWith);

            var tabToClick = tabContainer.FindElements(By.CssSelector(tabElementPartialId))
                                .Where(clickElement => clickElement.Displayed && clickElement.GetAttribute("title").Equals(tabName))
                                .ToList();

            if (tabToClick.Count == 1)
            {
                return tabToClick.Single();
            }

            return null;
        }

        public void DialogueWorkSpaceDataSelection(int rowNumBerToClick = 0)
        {
            var dialogueDivElement = myDriver.FindElement(By.Id("DialogWorkspace_window"));

            WaitForElement(By.ClassName("dxeListBoxItemRow_Office2010Silver"));
            var listOfRowElements = dialogueDivElement.FindElements(By.ClassName("dxeListBoxItemRow_Office2010Silver")).ToList();
          
            listOfRowElements[rowNumBerToClick].Click();

        }

        public List<IWebElement> GetTabRowElemets(IWebElement tabContentElement,string tabName)
        {
            string gridName = tabName;
            //Remove space between the tab name to get the menu id if space is in between the tab name
            if (tabName.Any(e => char.IsWhiteSpace(e)))
            {
               gridName = tabName.Replace(" ","");
            }

            
            var contentDivElement = tabContentElement.FindElement(By.ClassName("dxgvCSD"));
            
            var rowElementList = contentDivElement.FindElements(By.CssSelector(string.Format("tr[id*='grd{0}_DXDataRow']",gridName))).ToList();

            return rowElementList;

        }

        public IWebElement TabRowClick(string tabName,int rowNumberTOClick = 0)
        {

            var clickedTab = ClickTab(tabName);
            var rowElements = GetTabRowElemets(clickedTab, tabName);
            rowElements[rowNumberTOClick].Click();
            return clickedTab;
        }

        protected void DropDownTypeableFieldInput(IWebElement element, string data)
        {
            element.Clear();
            element.SendKeys(data);
            element.SendKeys(Keys.ArrowDown);
            element.SendKeys(Keys.Enter);

        }

        protected void SendKeysUsingAction(IWebElement element,string data)
        {
            Actions action = new Actions(myDriver);
            action.MoveToElement(element);
            action.Click(element);
            action.SendKeys(data);
            action.Build().Perform();
        }
    }
}