using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Selenium.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Selenium
{
    /// <summary>
    /// UI test builder helps you to build a correct Web driver
    /// </summary>
    public class UITestBuilder
    {
        private bool _hideBrowser = false;
        private bool _hideInfoBar = true;
        private bool _disableExtensions = true;
        private bool _disableGpu = true;
        private bool _startMaximizedWindow = false;
        private LogLevel _logLevel = LogLevel.Severe;
        private SeleniumObject _driver;
        private string _baseUrl;
        private List<string> _popupListElementId;
        private List<string> _loaderListElementId;
        private int _timeout = 30;

        /// <summary>
        /// If true the browser is not opened
        /// </summary>
        /// <returns></returns>
        public UITestBuilder SetHideBrowser(bool set)
        {
            _hideBrowser = set;
            return this;
        }

        /// <summary>
        /// If true the bar "browser is controller by..." is not showed
        /// </summary>
        /// <returns></returns>
        public UITestBuilder SetHideInfoBar(bool set)
        {
            _hideInfoBar = set;
            return this;
        }

        /// <summary>
        /// If true all browser extensions are disabled
        /// </summary>
        /// <returns></returns>
        public UITestBuilder SetDisableAllBrowserExtensions(bool set)
        {
            _disableExtensions = set;
            return this;
        }

        /// <summary>
        /// If true the Gpu is disabled on browser
        /// </summary>
        /// <returns></returns>
        public UITestBuilder SetDisableGpu(bool set)
        {
            _disableGpu = set;
            return this;
        }

        /// <summary>
        /// If true the browser start in full window
        /// </summary>
        /// <returns></returns>
        public UITestBuilder SetStartWithBrowserFullWindow(bool set)
        {
            _startMaximizedWindow = set;
            return this;
        }

        /// <summary>
        /// Setting the log level from browser console
        /// It can be used to debug javascript errors
        /// </summary>
        /// <param name="loglevel"></param>
        /// <returns></returns>
        public UITestBuilder SetLogLevel(LogLevel loglevel)
        {
            _logLevel = loglevel;
            return this;
        }

        /// <summary>
        /// Setting the base url where the browser go
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public UITestBuilder SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }

        /// <summary>
        /// List of all element id for popup
        /// </summary>
        /// <param name="popupList"></param>
        /// <returns></returns>
        public UITestBuilder SetPopupListId(List<string> popupList)
        {
            _popupListElementId = popupList;
            return this;
        }

        /// <summary>
        /// List of all element id for loaders
        /// </summary>
        /// <param name="loaderList"></param>
        /// <returns></returns>
        public UITestBuilder SetLoaderListId(List<string> loaderList)
        {
            _loaderListElementId = loaderList;
            return this;
        }

        /// <summary>
        /// Set the selenium TimeOut
        /// </summary>
        /// <param name="loaderList"></param>
        /// <returns></returns>
        public UITestBuilder SetTimeout(int timeoutVal)
        {
            _timeout = timeoutVal;
            return this;
        }

        /// <summary>
        /// Creating driver with options
        /// </summary>
        /// <returns></returns>
        public UITestBuilder Create()
        {
            ChromeOptions options = new ChromeOptions();

            if (_hideInfoBar) {
                options.AddArgument("--disable-infobars");
                options.AddExcludedArgument("--enable-automation");
                options.AddAdditionalCapability("useAutomationExtension", false);
            }

            if (_hideBrowser)
                options.AddArgument("--headless");

            if (_disableExtensions)
                options.AddArgument("--disable-extensions");

            if (_disableGpu)
                options.AddArgument("--disable-gpu");

            if (_startMaximizedWindow)
                options.AddArgument("--start-maximized");


            options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
            //options.AddArgument("--disable-blink-features=AutomationControlled");


            ////var perfLogPrefs = new ChromePerformanceLoggingPreferences(); 
            ////perfLogPrefs.AddTracingCategories("devtools.network");

            ////options.PerformanceLoggingPreferences = perfLogPrefs;
            ////options.AddAdditionalCapability(CapabilityType.EnableProfiling, true);


            //options.SetLoggingPreference("performance", _logLevel);
            //options.SetLoggingPreference(LogType.Browser, _logLevel);

            options.AddArgument("–disable-dev-shm-usage");
            options.AddArgument("--disable-extensions");
            options.AddArgument("no-sandbox");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-setuid-sandbox");
            var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));


            var seleniumConfiguration = new SeleniumConfiguration
            {
                BaseURL = _baseUrl,
                Driver = driver,
                ListLoaderElementId = _loaderListElementId,
                ListPopupElementId = _popupListElementId,
                Timeout = _timeout
            };

            _driver = new SeleniumObject(seleniumConfiguration);

            return this;
        }

        /// <summary>
        /// Validation and run the driver
        /// </summary>
        /// <returns></returns>
        public SeleniumObject ValidateAndRun()
        {
            StartValidation();
            return _driver;
        }

        /// <summary>
        /// Some validation to not run an invalid driver
        /// </summary>
        /// <returns></returns>
        private bool StartValidation()
        {
            if (_driver == null || _driver.BrowserControl() == null)
                throw new Exception("Driver is null, you must first run .Create()");

            if (string.IsNullOrEmpty(_baseUrl))
                throw new Exception("BaseUrl is null, set with SetBaseUrl(string)");

            return true;
        }


    }
}
