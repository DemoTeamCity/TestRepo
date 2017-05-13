using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace TlxSlxWebTests.Pages.TlxPages
{
    public class TlxBaseDetailsPage : TlxBasePage
    {
        public TlxBaseDetailsPage(IWebDriver aDriver) : base(aDriver)
        {
        }

        public IWebElement GetTabByName(string tabName)
        {
            return null;
        }
    }
}
