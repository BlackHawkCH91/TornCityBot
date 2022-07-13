//Libraries
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

ChromeOptions options = new ChromeOptions();
options.AddArgument("--disable-blink-features=AutomationControlled");

IWebDriver driver = new ChromeDriver(@"C:\ChromeDrivers\103\", options);
driver.Navigate().GoToUrl("https://www.google.com");



Console.WriteLine("Hello, World!");