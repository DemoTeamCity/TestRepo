using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using TlxSlxWebTests.DTOs;

namespace TlxSlxWebTests.Pages.TlxPages
{
    public class IcrmTaskPage : TlxBaseDetailsPage
    {
        public IcrmTaskPage(IWebDriver aDriver) : base(aDriver)
        {
            SearchResultGrid = new IcrmGridPage(myDriver);
            ToolBar = new TlxToolBarPage(myDriver);
        }

        public IcrmTaskPage SearchTask(string searchBy, string searchOperator, string searchValue)
        {
            var searchConditions = new List<Tuple<LookUpSearchInput>>();

            searchConditions.Add(Tuple.Create(
                                    new LookUpSearchInput
                                    {
                                        SearchBy = searchBy,
                                        SearchOperator = searchOperator,
                                        Value = searchValue
                                    }));

            LookupSearchBy(searchConditions);

            var newTasksPage = new IcrmTaskPage(myDriver);
            return newTasksPage;
        }

        public IcrmTaskPage AssignTeamMemberToTask(int taskRowNumber, string teamMemberName)
        {
            var tabElement = TabRowClick("Tasks");
            SelectMenuByRightClickingOnTab("Tasks", tabElement, "Assign Team Member");
            var listOfTeamRows = GetTabRowElemets(myDriver.FindElement(By.Id("DialogWorkspace_window")), "Team Member");
            var selectedRow = listOfTeamRows.Where(element => element.Text.Equals(teamMemberName));
            if(selectedRow.Count() == 1)
            {
                selectedRow.Single().FindElement(By.CssSelector("span[id$=_chkAssigned_0_S_D")).Click();
            }
            ClickButton("DialogWorkspace_TLXAssignTeamMember_btnOk");
            WaitForAsyncPostbackIndicator();
            return new IcrmTaskPage(myDriver);
        }

        public IcrmTaskPage AddTimeToTask(TimeDTO timeEntryData)
        {

            var tabElement = TabRowClick("Tasks");
            SelectMenuByRightClickingOnTab("Tasks", tabElement, "Add Time");
            //WaitForAsyncPostbackIndicator();
            TimeEntry(timeEntryData);
            return new IcrmTaskPage(myDriver);
        }

        public IcrmTaskPage AddTimeToTaskInTimeGrid(string tabToClick, string accountName,string taskPartialName,TimeDTO timeEntryData)
        {
            //wait for time grid iframe to be loaded
            WaitForElementToBeVisible(By.Id("MainContent_frmTLXTimeGrid_frameTimeGrid"), 30);

            //switch into the timegrid iframe 
            myDriver.SwitchTo().Frame(myDriver.FindElement(By.Id("MainContent_frmTLXTimeGrid_frameTimeGrid")));

            //wait for the control element to be loaded
            WaitForElementToBeVisible(By.Id("ASPxPageControl1"));
            var leadingContainerElement = myDriver.FindElement(By.Id("leadingContainer"));
           
            //get the list of tabs in the time grid control element and click the required element 
            var timeGridMenuTabElementsDiv = leadingContainerElement.FindElement(By.Id("ASPxPageControl1"));
            var tabElementTOSelect = timeGridMenuTabElementsDiv.FindElements(By.TagName("li")).Single(element => element.Text.Equals(tabToClick));
            tabElementTOSelect.Click();
            //WaitForTimeGridInternalTabLoader();

            //wait for the tab to be loaded
            WaitForElementToBeVisible(By.Id("ASPxPageControl1_CC"));
            var projectTaskContainerElement = timeGridMenuTabElementsDiv.FindElement(By.Id("ASPxPageControl1_CC"));

            //click on the lookup account in the time grid account filter 
            WaitForElementToBeVisible(By.Id("lueAccountAccProjectFilter_LookupBtn"));
            projectTaskContainerElement.FindElement(By.Id("lueAccountAccProjectFilter_LookupBtn")).Click();
            
            //wait for the account lookup
            WaitForElementToBeVisible(By.Id("lueAccount_LookupAccProjectFilter-Dialog"));

            //select the value from the account lookup
            var lookUpDialogue = myDriver.FindElement(By.Id("lueAccount_LookupAccProjectFilter-Dialog"));
            var lookUpContainer = lookUpDialogue.FindElement(By.CssSelector("div[class = 'dojoxGridScrollbox']"));
            //lookUpContainer.SendKeys(accountName);

            WaitForElementToBeVisible(By.CssSelector("div[class^='dojoxGridRow']"));
            //select the account from the lookup
            var lookUpRowList = lookUpContainer.FindElements(By.CssSelector("div[class^='dojoxGridRow']")).ToList();
            var lookUpRowToClick = lookUpRowList.Single(element => element.FindElement(By.TagName("td")).Text.Equals(accountName));
            DoubleClick(lookUpRowToClick.FindElement(By.TagName("td")));
            WaitForTimeGridInternalTabLoader();
            //select the task from the leadingContainer tab box 

            var listGridTableElement = projectTaskContainerElement.FindElement(By.Id("grdAccountProjects"));
            //WaitForElementToBeVisible(By.ClassName("dxgvCSD"));
            ScrollToEnd(listGridTableElement.FindElement(By.ClassName("dxgvCSD")));
            WaitForTimeGridInternalTabLoader();
           // WaitForElementToNotToBeVisible(By.Id("grdTimeSheets_DXHFP_TL"));

            var listOfTasksInProject = GetTabRowElemets(listGridTableElement, "AccountProjects");
            var taskToClick = listOfTasksInProject.Single(e => e.Text.Contains(taskPartialName));
            //right click on the task
            //RightClick(listOfTasksInProject[taskElementRowNoToClick]);
            RightClick(taskToClick);
            
            //wait for the menu and click on add time 
            var menuContainer = projectTaskContainerElement.FindElement(By.CssSelector("div[class='dxmLite dxm-ltr']"));
            WaitForElementToBeVisible(By.Id("mnuAccountProjects"));
            var menuElement = menuContainer.FindElement(By.Id("mnuAccountProjects"));
            var menuToClick = menuElement.FindElements(By.TagName("li")).Single(element => element.Text.Equals("Add Time"));
            menuToClick.Click();
            
            //switch to default content 
            myDriver.SwitchTo().DefaultContent();
            //enter the time entry details in time entry form
            TimeEntry(timeEntryData);

            return new IcrmTaskPage(myDriver);
        }

        public void TimeEntry(TimeDTO timeEntryData)
        {

            WaitForElementToBeVisible(By.Id("dijit_Dialog_0"));
            var dijitContainer = myDriver.FindElement(By.CssSelector("div[class*='dijitDialogPaneContent dijitAlignCenter']"));
            WaitForLoader();
            myDriver.SwitchTo().Frame(dijitContainer.FindElement(By.TagName("iFrame")));

            var elementContainer = myDriver.FindElement(By.Id("uniqName_1_0"));
           
            DropDownTypeableFieldInput(elementContainer.FindElement(By.Id("pklLocation")), timeEntryData.Location);
            DropDownTypeableFieldInput(elementContainer.FindElement(By.Id("txtStartTime")), DateTime.Now.Hour.ToString());
            DropDownTypeableFieldInput(elementContainer.FindElement(By.Id("pklServiceTaskCategory")), timeEntryData.ServiceCategory);
            DropDownTypeableFieldInput(elementContainer.FindElement(By.Id("txtActualDuration")), timeEntryData.Duration);
            DropDownTypeableFieldInput(elementContainer.FindElement(By.Id("selConsultant")), timeEntryData.Consultant);
            elementContainer.FindElement(By.Id("txtBillingComments")).SendKeys(timeEntryData.BillingComments);
            elementContainer.FindElement(By.Id("txtInternalComments")).Click();
            ClickButton("dijit_form_Button_0");
            WaitForLoader();
            // myDriver.FindElement(By.CssSelector("span[class='dijitReset dijitInline dijitButtonNode']")).Click();
            myDriver.SwitchTo().DefaultContent();
        }
    }
}
