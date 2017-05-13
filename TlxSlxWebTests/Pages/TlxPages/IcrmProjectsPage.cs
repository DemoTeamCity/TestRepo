using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using TlxSlxWebTests.DTOs;

namespace TlxSlxWebTests.Pages.TlxPages
{
    public class IcrmProjectsPage : TlxBaseDetailsPage
    {
        string PROJECT_NAME_FIELD_ID = "ContentPlaceHolderMain_MainContent_ProjectInsert_ctlProjectName";

        public IcrmProjectsPage(IWebDriver aDriver) : base(aDriver)
        {
            SearchResultGrid = new IcrmGridPage(myDriver);
            ToolBar = new TlxToolBarPage(myDriver);
        }

        public IcrmProjectsPage SearchProject(string searchBy, string searchOperator, string searchValue)
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

            var newProjectsPage = new IcrmProjectsPage(myDriver);
            return newProjectsPage;
        }

        public string CreateNewProject(ProjectDTO projectData)
        {
            string newProjectId = string.Empty;

            var searchConditions = new List<Tuple<LookUpSearchInput>>();

            SetFieldValue(PROJECT_NAME_FIELD_ID, projectData.ProjectName);

            var billFieldElement = myDriver.FindElement(By.Id("ContentPlaceHolderMain_MainContent_ProjectInsert_ctlMode-SingleSelectPickList-Combo"));
            DropDownTypeableFieldInput(billFieldElement, projectData.BillMode);
            WaitForAsyncPostbackIndicator();

            var invoiceField = myDriver.FindElement(By.Id("ContentPlaceHolderMain_MainContent_ProjectInsert_pklInvoiceType-SingleSelectPickList-Combo"));
            DropDownTypeableFieldInput(invoiceField, projectData.InvoiceType);

            if (!string.IsNullOrEmpty(projectData.AccountName))
            {
                ClickButton("ContentPlaceHolderMain_MainContent_ProjectInsert_ctlAccount_LookupBtn");
                searchConditions = new List<Tuple<LookUpSearchInput>> { Tuple.Create(
                                    new LookUpSearchInput
                                    {
                                        SearchBy = "Account",
                                        SearchOperator = "Equal to",
                                        Value = projectData.AccountName
                                    })};
                LookupField(searchConditions);
                WaitForAsyncPostbackIndicator();
            }

            newProjectId = GetFieldValue("ContentPlaceHolderMain_MainContent_ProjectInsert_ctlProjectNo");

            if (!string.IsNullOrEmpty(projectData.ProjectManager))
            {
                ClickButton("ContentPlaceHolderMain_MainContent_ProjectInsert_ctlProjectMgr_LookupBtn");
                searchConditions = new List<Tuple<LookUpSearchInput>> { Tuple.Create(
                                    new LookUpSearchInput
                                    {
                                        SearchBy = "User Name",
                                        SearchOperator = "Equal to",
                                        Value = projectData.ProjectManager
                                    }) };
                LookupField(searchConditions);
            }

            if (!string.IsNullOrEmpty(projectData.SoldBy))
            {
                ClickButton("ContentPlaceHolderMain_MainContent_ProjectInsert_ctlSoldBy_LookupBtn");
                searchConditions = new List<Tuple<LookUpSearchInput>> { Tuple.Create(
                                    new LookUpSearchInput
                                    {
                                        SearchBy = "UserName",
                                        SearchOperator = "Equal to",
                                        Value = projectData.SoldBy
                                    }) };
                LookupField(searchConditions);
            }

            ClickButton("ContentPlaceHolderMain_MainContent_ProjectInsert_btnSave");
            WaitForAsyncPostbackIndicator();
            //WaitForLoader();
            WaitForElementToBeVisible(By.CssSelector("div[class='mainContentContent']"));
            return newProjectId;
        }

        public bool AddNewTask(TaskDTO taskData)
        {

            
           var contentOfTab =  ClickTab("Tasks");
            
           var initialRowCount = GetTabRowElemets(contentOfTab, "Tasks").Count();
            SelectMenuByRightClickingOnTab("Tasks", contentOfTab, "Add Task");

            var searchConditions = new List<Tuple<LookUpSearchInput>>();



            if (!string.IsNullOrEmpty(taskData.TaskContact))
            {
                ClickButton("DialogWorkspace_ProjectTaskQuickAdd_lueContact_LookupBtn");
                searchConditions = new List<Tuple<LookUpSearchInput>> { Tuple.Create(
                                    new LookUpSearchInput
                                    {
                                        SearchBy = "Name",
                                        SearchOperator = "Equal to",
                                        Value = taskData.TaskContact
                                    })};
                LookupField(searchConditions);
                WaitForAsyncPostbackIndicator();
            }


            if (!string.IsNullOrEmpty(taskData.ParentTask))
            {
                ClickButton("DialogWorkspace_ProjectTaskQuickAdd_lueParentTask_LookupBtn");
                searchConditions = new List<Tuple<LookUpSearchInput>> { Tuple.Create(
                                    new LookUpSearchInput
                                    {
                                        SearchBy = "Seq No",
                                        SearchOperator = "Equal to",
                                        Value = taskData.ParentTask
                                    })};
                LookupField(searchConditions);
                WaitForAsyncPostbackIndicator();
            }


            SetFieldValue("dijit_form_ComboBox_0", taskData.TaskName);

            SetFieldValue("DialogWorkspace_ProjectTaskQuickAdd_pklProjectTaskCategory_ctl00-SingleSelectPickList-Combo", taskData.Category);

            SetFieldValue("DialogWorkspace_ProjectTaskQuickAdd_pklDepartment_ctl00-SingleSelectPickList-Combo", taskData.Department);

            SetFieldValue("DialogWorkspace_ProjectTaskQuickAdd_pklPhase_ctl00-SingleSelectPickList-Combo", taskData.Phase);

            ClickButton("DialogWorkspace_ProjectTaskQuickAdd_btnOk");
            WaitForAsyncPostbackIndicator();

            WaitForElementToBeVisible(By.CssSelector(string.Format("tr[id*='_grdTasks_DXDataRow{0}']", initialRowCount)),20);
            var rowCountAfterInsertingTheTask = GetTabRowElemets(contentOfTab, "Tasks").Count();
            if(rowCountAfterInsertingTheTask == initialRowCount+1)
            {
                return true;
            }
            return false;
        }

        public bool AddNewTeamMemberTOAProject(string departments, string teamMemberName)
        {
            var requiredElement = ClickTab("Team");
            var initialTeamRowCount = GetTabRowElemets(requiredElement, "Team").Count();
            SelectMenuByRightClickingOnTab("Team", requiredElement, "Add Resource");

            WaitForDialogWorkspaceWindowToShow();

            var departmentSelectionField = myDriver.FindElement(By.Id("DialogWorkspace_ProjectTeamInsert_pklTLXDepartment"));
            DropDownTypeableFieldInput(departmentSelectionField, "Graphics");

            //departmentSlectionField.Clear();
            //SelectItemFromDialogueWorkspaceDropDown("widget_DialogWorkspace_ProjectTeamInsert_pklTLXDepartment", "Graphics");
            //SetFieldValue("DialogWorkspace_ProjectTeamInsert_pklTLXDepartment", "Graphics");
            //departmentSlectionField.SendKeys("Graphics");
            //departmentSlectionField.SendKeys(Keys.ArrowDown);
            //departmentSlectionField.SendKeys(Keys.Enter);
            WaitForAsyncPostbackIndicator();
            
            DialogueWorkSpaceDataSelection(); 

            ClickButton("DialogWorkspace_ProjectTeamInsert_btnOk");

            WaitForAsyncPostbackIndicator();

            WaitForElementToBeVisible(By.CssSelector(string.Format("tr[id*='_grdTeam_DXDataRow{0}']", initialTeamRowCount)), 20);

            var rowCountAfterInsertingTheTeamMember = GetTabRowElemets(requiredElement, "Team").Count();
            if (rowCountAfterInsertingTheTeamMember == initialTeamRowCount + 1)
            {
                return true;
            }
            return false;
          
        }

        public IcrmProjectsPage DeleteProject(string projectId)
        {

            var projectOpened = GetFieldValue("ContentPlaceHolderMain_MainContent_ProjectDetail_ctlProjectNo");
            if(projectId == projectOpened)
            {
                ClickButton("ContentPlaceHolderMain_MainContent_ProjectDetail_btnDelete");
                AcceptAlert();
                WaitForAsyncPostbackIndicator();
                
                return new IcrmProjectsPage(myDriver);
            }
            return null;
            
        }

        public IcrmProjectsPage RunServiceQuery(string date , int rowElementNoToGiveManagerApprovel)
        {
            WaitForElementToBeVisible(By.Id("TLXServiceQuerycontents"),10);
            var serviceQueryContainer = myDriver.FindElement(By.Id("TLXServiceQuerycontents"));
            var container = serviceQueryContainer.FindElement(By.Id("MainContent_TLXServiceQuery_navBarFilters_GCTC0_tabFilters_0_CC"));

            var startDateInputElement = container.FindElement(By.Id("MainContent_TLXServiceQuery_navBarFilters_GCTC0_tabFilters_0_dteFromRange_0"));
            SendKeysUsingAction(startDateInputElement, date);

            var endDateInputElement = container.FindElement(By.Id("MainContent_TLXServiceQuery_navBarFilters_GCTC0_tabFilters_0_dteToRange_0"));
            SendKeysUsingAction(endDateInputElement, date);

            ClickButton("MainContent_TLXServiceQuery_btnRunQuery");

            WaitForAsyncPostbackIndicator();
           
            

            return new IcrmProjectsPage(myDriver);
        }

        public bool GiveManagerApprovelToARecord(int recordNoToGiveApprovel)
        {
            var serviceQueryContainer = myDriver.FindElement(By.Id("TLXServiceQuerycontents"));
            var serviceQueryContainerElement = myDriver.FindElement(By.Id("MainContent_TLXServiceQuery_tabGrids_CC"));
            var resultRows = GetTabRowElemets(serviceQueryContainerElement, "Service");
            resultRows[recordNoToGiveApprovel].Click();
            RightClick(myDriver.FindElement(By.Id("MainContent_TLXServiceQuery_tabGrids_grdService")));

            MenuOrSubMenuSelectionFromContextMenu(serviceQueryContainerElement, "Service", "Approvals", "Manager Approval");

            //var menuContainer = serviceQueryContainer.FindElement(By.CssSelector("div[class='dxmLite dxm-ltr']"));
            //WaitForElementToBeVisible(By.Id("MainContent_TLXServiceQuery_tabGrids_mnuService"));
            //var menuElement = menuContainer.FindElement(By.Id("MainContent_TLXServiceQuery_tabGrids_mnuService"));
            //var mainMenuToClick = menuElement.FindElements(By.CssSelector("li[id*='MainContent_TLXServiceQuery_tabGrids_mnuService_DXI'][class*='dxm-item']")).Single(element => element.Text.Equals("Approvals"));
            //mainMenuToClick.Click();
            //submenu click to do
            WaitForElementToBeVisible(By.Id("queryDialog"));
            var queryDialogueBox = myDriver.FindElement(By.Id("queryDialog"));
            
            var conditionResult = queryDialogueBox.FindElement(By.Id("queryDialog-questionDiv")).Text.Contains("have been applied");
            queryDialogueBox.FindElement(By.CssSelector("span[class*='dijitReset dijitInline dijitButtonNode']")).Click();
            return conditionResult;
        }

        public IcrmProjectsPage ViewProjectGantt()
        {

           var contentTab =  ClickTab("Gantt");

            WaitForElementToBeVisible(By.Id(contentTab.GetAttribute("id")));


            return null;
        }
    }
}