using System;
using System.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using TlxSlxWebTests.Pages.TlxPages;
using System.Collections.Generic;
using System.Threading;
using TlxSlxWebTests.DTOs;
using OpenQA.Selenium.Chrome;


namespace TlxSlxWebTests.TestSuites
{
    [TestFixture]
    public class IcrmSmokeTests
    {
        //This comment is to check Teamcity checkin build works or not
        private const string BASE_URL = "BaseUrl";
        private const string WEBDRIVER_TYPE = "WebDriverType";
        private const string ACCOUNT_NAME_TO_SEARCH = "Tag.Ltd";
        private const string ProjectNoToSearch = "Tag.Ltd-8";
        private IWebDriver driver;
        private TlxBasePage currentPage;
        private string expectedProjectId ;

        #region Setup and Teardown

        [OneTimeSetUp]
        public void OneTimeSetup() 
        {
            SetupDriver();
            
           driver.Navigate().GoToUrl(ConfigurationManager.AppSettings[BASE_URL]);
            driver.Manage().Window.Maximize();
        }

        [OneTimeTearDown]
        public void OntTimeTearDown()
        {
            driver.FindElement(By.Id("btnLogOff")).Click();
            if (driver != null)
            {
                driver.Quit();

                driver.Dispose();
            }
        }

        #endregion

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(1)]
        public void Icrm_LoginWithValidCredentials_LoginShouldBeSuccessful()
        {
            //Arrange
            IcrmLoginPage loginPage = new IcrmLoginPage(driver);

            //Act
            currentPage = loginPage.Login(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);

            //Assert
            Assert.IsTrue(loginPage.IsLoginSuccessful(), "Login is failed.");
            Assert.IsNotNull(currentPage, "Error when login.");
            Assert.IsTrue(currentPage is IcrmAccountsPage, "After login, Accounts page is not returned");
            Assert.IsFalse(currentPage.myDriver.Url.EndsWith("Login.aspx"));

            loginPage.Dispose();
        }

        #region Fine Test Cases

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(2)]
        public void Icrm_SearchForAnAccount_AccountShouldBeDisplayed()
        {
            int expectedNumberOfResults = 1;

            //Arrange
            IcrmAccountsPage accountsPage = currentPage as IcrmAccountsPage;

            //Act
            currentPage = accountsPage.SearchAccount("Account", "Equal to", ACCOUNT_NAME_TO_SEARCH);

            //Assert
            Assert.AreEqual(expectedNumberOfResults, accountsPage.SearchResultGrid.GetTotalNumberOfSearchResult(), "Expected total number of results are wrong.");

            var allRows = accountsPage.SearchResultGrid.GetAllRows();
            Assert.AreEqual(expectedNumberOfResults, allRows.Count, "Returned rows are not correct.");

            var actualAccountName = allRows[0][accountsPage.SearchResultGrid.GetColumnNames()[0]];
            Assert.AreEqual(ACCOUNT_NAME_TO_SEARCH, actualAccountName, "Searched account is not returned.");

            accountsPage.Dispose();
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(3)]
        public void Icrm_ClcikOnTimeLinxTab_TimeLinxTabShouldExpand()
        {
            //Arrange
            //Precondition check
            var isTimeLinxMainTabExpanded = currentPage.IsMainTabExpanded("timelinx");
            Assert.IsFalse(isTimeLinxMainTabExpanded, "TimeLinx main tab is expanded by default.");

            //Act
            currentPage.NavigateMainTabTo("timelinx");

            //Assert
            isTimeLinxMainTabExpanded = currentPage.IsMainTabExpanded("timelinx");
            Assert.IsTrue(isTimeLinxMainTabExpanded, "TimeLinx main tab is NOT expanded after clicking on it.");
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(4)]
        public void Icrm_ClickOnProjects_ProjectsShouldBeDisplayed()
        {
            //Arrange
            var accountsPage = currentPage as IcrmAccountsPage;

            //Act
            currentPage = accountsPage.SelectMainMenu("Projects");

            //Assert 
            var projectsPage = currentPage as IcrmProjectsPage;
            Assert.IsNotNull(projectsPage, "Clicking on Projects menu does not return Projects Page");
            Assert.IsTrue(projectsPage.myDriver.Url.EndsWith("TLXProject.aspx"), "URL of Projects Page does not end with correct name.");
            Assert.IsTrue(projectsPage.SearchResultGrid.GetTotalNumberOfSearchResult() > 0, "Projects are not listed properly in porjects page.");

            accountsPage.Dispose();
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(5)]
        public void Icrm_SearchForAProject_ProjectShouldBeDisplayed()
        {
            //Arrange
            IcrmProjectsPage projectsPage = currentPage as IcrmProjectsPage;
            //IcrmProjectsPage projectsPage = new IcrmProjectsPage(currentPage.myDriver);
            //Act
            currentPage = projectsPage.SearchProject("Project #", "Equal to", ProjectNoToSearch);

            //Assert
            Assert.AreEqual(1, projectsPage.SearchResultGrid.GetTotalNumberOfSearchResult(), "Expected total number of results are wrong.");

            var allRows = projectsPage.SearchResultGrid.GetAllRows();
            Assert.AreEqual(1, allRows.Count, "Returned rows are not correct.");

            var actualProjectName = allRows[0][projectsPage.SearchResultGrid.GetColumnNames()[0]];
            Assert.AreEqual(ProjectNoToSearch, actualProjectName, "Searched project is not returned.");

            projectsPage.Dispose();
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(6)]
        public void Icrm_ClickForAProject_ProjectDetailShouldBeDisplayed()
        {
            //Arrange
            IcrmProjectsPage projectsPage = currentPage as IcrmProjectsPage;

            //Act
            projectsPage.SearchResultGrid.ClickOn(0, "Project #");

            //Assert
            Assert.IsTrue(projectsPage.myDriver.Title.Contains("Project - " + ProjectNoToSearch), "Project Details page is not opened correctly.");
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(7)]
        public void Icrm_ClickOnTasks_TasksShouldBeDisplayed()
        {
            //Act
            currentPage = currentPage.SelectMainMenu("Tasks");

            //Assert
            var tasksPage = currentPage as IcrmTaskPage;
            Assert.IsTrue(tasksPage.myDriver.Url.EndsWith("TLXProjectTask.aspx"), "Clicking on Tasks is not opening Tasks page correctly");
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(8)]
        public void Icrm_SearchForTask_TaskShouldBeDisplayed()
        {
            //Arrange
            var tasksPage = currentPage as IcrmTaskPage;

            //Act
            currentPage = tasksPage.SearchTask("Project #", "Equal to", "Tag.L-2");

            //Assert
            Assert.AreEqual(1, tasksPage.SearchResultGrid.GetTotalNumberOfSearchResult(), "Search result did not return correct number of tasks.");
            var allTasks = tasksPage.SearchResultGrid.GetRowByIndex(0);
            Assert.AreEqual("Acceptance Testing", allTasks["Task"], "Returned task result is not as expected.");

            tasksPage.Dispose();
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(9)]
        public void Icrm_ClickForATask_TaskDetailsShouldBeDisplayed()
        {
            //Act
            currentPage.SearchResultGrid.ClickOn(0, "Task");

            //Assert
            Assert.IsTrue(currentPage.myDriver.Title.Contains("Task - 1 - Acceptance Testing"), "Task Details page is not opened correctly.");
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(10)]
        public void Icrm_CreateNewProject_NewProjectShouldBeSaved()
        {
            //Arrange
            currentPage.ToolBar.SelectToolMenu("New", "Project...");
            var projectPage = new IcrmProjectsPage(currentPage.myDriver);
            currentPage.Dispose();

            var projectData = new ProjectDTO
            {
                AccountName = "Tag.Ltd",
                ProjectName = "TLXQA Automation Testing Project",
                ProjectManager = "Hogan, Lee",
                SoldBy = "Lee Hogan",
                BillMode = "Task",
                InvoiceType = "On Completion"
            };

            //Act
            expectedProjectId = projectPage.CreateNewProject(projectData);

            //Assert
            Assert.IsFalse(string.IsNullOrEmpty(expectedProjectId), "New Project is not created successfully.");
            Assert.IsTrue(expectedProjectId.Contains(projectData.AccountName), "New project name format is wrong.");

            currentPage = projectPage;
        }

        #endregion

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(11)]
        public void Icrm_CreateNewTask_NewTaskShouldBeSaved()
        {
            var projectTaskPage = new IcrmProjectsPage(currentPage.myDriver);
            currentPage.Dispose();

            var taskData = new TaskDTO
            {
                TaskContact = "Gtag, Gowtham",
                TaskName = "Demo Task " + expectedProjectId,
                Category = "testing",
                Department = "Test",
                Phase = "1",
                ParentTask = null
            };
            var taskInsertionStatus = projectTaskPage.AddNewTask(taskData);
            currentPage = projectTaskPage;
            Assert.IsTrue(taskInsertionStatus, "Task for the project is not created properly");
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(12)]
        public void Icrm_AddTeamMemberToAProject_TeamMemberShouldBeAdded()
        {

            var projectPage = new IcrmProjectsPage(currentPage.myDriver);

            var teamMemberInsertionStatus = projectPage.AddNewTeamMemberTOAProject("Graphics", "Gtag");
            currentPage = projectPage;
            Assert.IsTrue(teamMemberInsertionStatus, "Team Member for the project is not added properly");

        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(13)]
        public void Icrm_AssignTeamMemberToATask_TeamMemberShouldBeAssignedToTheTask()
        {

            var taskPage = new IcrmTaskPage(currentPage.myDriver);

            currentPage = taskPage.AssignTeamMemberToTask(1, "Gtag,");
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(14)]
        public void Icrm_AddTimeToATask_TimeShouldBeAddedToTheTask()
        {

            var taskPage = new IcrmTaskPage(currentPage.myDriver);

            var timeEntryData = new TimeDTO
            {
                Location = "office",
                ServiceCategory = "Emergency Work",
                Consultant = "Lee Hogan",
                Duration = "2",
                BillingComments = "This is to test the time entry through automation"
            };
            currentPage = taskPage.AddTimeToTask(timeEntryData);
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(15)]
        public void Icrm_NavigateToTimeGrid_TimeGridShouldBeOpenedInMainContentWindow()
        {
            var taskPage = new IcrmTaskPage(currentPage.myDriver);
            if (!taskPage.IsMainTabExpanded("timelinx"))
            {
                taskPage.NavigateMainTabTo("timelinx");
            }
            currentPage = taskPage.SelectMainMenu("timegrid");
            Assert.IsTrue(currentPage.myDriver.Title.Contains("TimeGrid"), "TimeGrid not opened correctly");
        }



        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(16)]
        public void Icrm_SelectATaskAndAddTime_TimeForTheTaskShouldBeAdded()
        {

            var taskPage = new IcrmTaskPage(currentPage.myDriver);
            var timeEntryData = new TimeDTO
            {
                Location = "office",
                ServiceCategory = "Emergency Work",
                Consultant = "Lee Hogan",
                Duration = "2",
                BillingComments = "This is to test the time entry in timesheet through automation"
            };
            currentPage = taskPage.AddTimeToTaskInTimeGrid("Project/Task", "Tag.Ltd", expectedProjectId, timeEntryData);
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(17)]
        public void Icrm_RunAServiceQueryForThatDay_ResultShouldBeDisplayed()
        {
            var projectPage = new IcrmProjectsPage(currentPage.myDriver);

            projectPage.ToolBar.SelectToolMenu("Project Tools", "Service Query");
            currentPage = projectPage.RunServiceQuery(/*DateTime.Today.Date.ToString("MM/dd/yyyy")*/"4/6/2017", 5);
            projectPage.Dispose();
        }

        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(18)]
        public void Icrm_GivingManagerApprovelToARecord_ManagerApprovelShouldBeGiven()
        {
            var projectPage = new IcrmProjectsPage(currentPage.myDriver);
            var managerApprovelResult = projectPage.GiveManagerApprovelToARecord(3);
            Assert.IsTrue(managerApprovelResult, "Manager approvel is not given");
            currentPage = projectPage;
            projectPage.Dispose();
        }


        [Category("ICRM WebClient SmokeTest")]
        [Test, Order(19)]
        public void Icrm_LaunchWebCalender_WebCalenderShouldBeLaunched()
        {
            if (!currentPage.IsMainTabExpanded("timelinx"))
            {
                currentPage.NavigateMainTabTo("timelinx");
            }
            currentPage=currentPage.SelectMainMenu("WebCalendar");
            
        }


        //[Category("ICRM WebClient SmokeTest")]
        //[Test, Order(19)]
        //public void Icrm_ViewProjectGantt_ProjectGanttShouldBeDisplayed()
        //{
        //    var projectPage = new IcrmProjectsPage(currentPage.myDriver);

        //    currentPage= projectPage.ViewProjectGantt();
        //}

        //[Category("ICRM WebClient SmokeTest")]
        //[Test, Order(20)]
        //public void Icrm_CreateNewTask2_NewTaskShouldBeSave()
        //{
        //    var projectTaskPage = new IcrmProjectsPage(currentPage.myDriver);
        //    currentPage.Dispose();

        //    var taskData = new TaskDTO
        //    {
        //        TaskContact = "Tag, Gman",
        //        TaskName = "Demo Task 2 " + expectedProjectId,
        //        Category = "testing",
        //        Department = "Test",
        //        Phase = "2",
        //        ParentTask = "1"
        //    };
        //    var taskInsertionStatus = projectTaskPage.AddNewTask(taskData);
        //    currentPage = projectTaskPage;
        //    Assert.IsTrue(taskInsertionStatus, "Task for the project is not created properly");
        //}


        //[Category("Cleaning the Automated data ")]
        //[Test, Order(3)]
        //public void DeleteTheCreatedTestProject()
        //{
        //    IcrmProjectsPage projectsPage = new IcrmProjectsPage(currentPage.myDriver);

        //    if (!projectsPage.IsMainTabExpanded("timelinx"))
        //    {
        //        projectsPage.NavigateMainTabTo("timelinx");
        //    }
        //    projectsPage.SelectMainMenu("Projects");
        //    projectsPage.SearchProject("Project #", "Equal to", "Tag.Ltd-13");
        //    projectsPage.SearchResultGrid.ClickOn(0, "Project #");
        //    projectsPage.DeleteProject("Tag.Ltd-13");


        //}



        private void SetupDriver()
        {
            string requiredWebDriverType = ConfigurationManager.AppSettings[WEBDRIVER_TYPE];

            switch (requiredWebDriverType)
            {
                case "Firefox":
                   
                    driver = new FirefoxDriver();
                    break;

                case "Chrome":
                    driver = new ChromeDriver();
                    break;
                
                default:
                    driver = null;
                    break;
            }

            if (driver == null)
            {
                throw new Exception("No Valid web driver is created.");
            }
        }
    }
}