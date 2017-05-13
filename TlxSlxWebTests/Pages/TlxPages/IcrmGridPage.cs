using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace TlxSlxWebTests.Pages.TlxPages
{
    public class IcrmGridPage : TlxBasePage
    {
        private const string COLUMN_CLASS_NAME = "dgrid-resize-header-container";

        public IcrmGridPage(IWebDriver aDriver) : base(aDriver)
        {
        }

        public List<string> GetColumnNames()
        {
            WaitForElement(By.ClassName(COLUMN_CLASS_NAME), 60);

            return myDriver.FindElements(By.ClassName(COLUMN_CLASS_NAME)).
                            Where(columnElement => !string.IsNullOrEmpty(columnElement.Text)).
                            Select(columnElement => columnElement.Text).
                            ToList();
        }

        public int GetTotalNumberOfSearchResult()
        {
            var numberOfRecordsText = myDriver.FindElement(By.Id("Sage_UI_ToolBarLabel_0")).Text;
            numberOfRecordsText = numberOfRecordsText.Substring(numberOfRecordsText.LastIndexOf(':') + 1).Trim();
            var resultCount = Convert.ToInt32(numberOfRecordsText);

            return resultCount;
        }

        public IWebElement GetRowELementByIndex(int rowIndex)
        {
            //get list of row values
            IWebElement rowElement = null;
            for (int i = rowIndex; i <= (rowIndex + 1); i++)
            {
                try
                {
                    string rowIdentifyingClassName = string.Format("div#listGrid-row-{0}", i);
                    rowElement = myDriver.FindElement(By.CssSelector(rowIdentifyingClassName));

                    if (rowElement != null)
                    {
                        break;
                    }
                }
                catch (Exception)
                {

                }
            }
            return rowElement;
        }

        public Dictionary<string, string> GetRowByIndex(int rowIndex)
        {
            var requiredRow = new Dictionary<string, string>();

            var requiredRowElement = GetRowELementByIndex(rowIndex);
            var rowValues = requiredRowElement.FindElements(By.CssSelector("tr>td")).Select(e => e.Text).ToList();

            //get list of columns
            var columnNames = GetColumnNames();

            //create row
            for (int i = 0; i < columnNames.Count; i++)
            {
                requiredRow.Add(columnNames[i], rowValues[i]);
            }

            return requiredRow;
        }

        public List<Dictionary<string, string>> GetAllRows()
        {
            var listOfRows = new List<Dictionary<string, string>>();

            var totalNumberOfResults = GetTotalNumberOfSearchResult();

            for (int i = 0; i < totalNumberOfResults; i++)
            {
                listOfRows.Add(GetRowByIndex(i));
            }

            return listOfRows;
        }

        public void ClickOn(int rowIndex, string columnText)
        {
            var columnClassName = GetColumnClassByColumnText(columnText);

            var requiredRow = GetRowELementByIndex(rowIndex);

            var requiredColumToClick = requiredRow.FindElements(By.TagName("td")).Single(column => columnClassName.Contains(column.GetAttribute("class")));

            requiredColumToClick.FindElement(By.TagName("a")).Click();

            WaitForElementToBeVisible(By.CssSelector("div[class='mainContentContent']"));
           // WaitForLoader();
        }

        private string GetColumnClassByColumnText(string columnText)
        {
            string resultClass = string.Empty;

            var requiredColumn = myDriver.FindElements(By.ClassName(COLUMN_CLASS_NAME)).Single(column => column.Text.ToLower().Equals(columnText.ToLower()));

            resultClass = requiredColumn.FindElement(By.XPath("..")).GetAttribute("class");

            return resultClass;
        }
    }
}