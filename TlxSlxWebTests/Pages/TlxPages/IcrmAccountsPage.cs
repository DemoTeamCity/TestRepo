using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace TlxSlxWebTests.Pages.TlxPages
{
    public class IcrmAccountsPage : TlxBaseDetailsPage
    {
        public IcrmAccountsPage(IWebDriver aDriver) : base(aDriver)
        {
            SearchResultGrid = new IcrmGridPage(myDriver);
            ToolBar = new TlxToolBarPage(myDriver);
        }

        public IcrmAccountsPage SearchAccount(string searchBy, string searchOperator, string searchValue)
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

            var newAccountsPage = new IcrmAccountsPage(myDriver);
            return newAccountsPage;
        }
    }
}