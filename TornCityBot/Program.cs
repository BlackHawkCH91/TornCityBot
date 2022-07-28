#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
//Libraries
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.DevTools;
using NAudio.Wave;
using System.Diagnostics;
using System.Linq;
using Vosk;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using TornActions = TornCityBot.TornActions;

//testing vars
bool headless = false;
Vosk.Vosk.SetLogLevel(-1);

//icon15-sidebar <- hospital
//Getting info:

/*
ToolTipPortal
    Div
        Div[2] (second div)
            p[2] (second para)

*/

//Console.WriteLine(SpeechRec());

//Start selenium
ChromeOptions options = new ChromeOptions();
string chromeTest = File.ReadAllText(@"WindowChrome.txt");
string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/AppData/Local/Google/Chrome/User Data/Profile 4";
options.AddArguments("user-data-dir=" + userProfile);
options.AddArgument("--profile-directory=Default");
options.AddArgument("--disable-blink-features=AutomationControlled");
options.AddArgument("--mute-audio");
options.AddArgument("window-size=1920,1080");
options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
options.AddExcludedArgument("enable-automation");

if (headless)
{
    options.AddArgument("headless");
}

Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

using var driver = new ChromeDriver(@"C:\ChromeDrivers\103\", options);
WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
//IDevTools devTools = driver as IDevTools;
driver.Manage().Window.Maximize();
Dictionary<string, object> cmdParams = new Dictionary<string, object>();
var loadScript = @"window.chrome = " + chromeTest + @";
                    const originalQuery = window.navigator.permissions.query;
                    window.navigator.permissions.query = (parameters) => (
                        parameters.name === 'notifications' ?
                        Promise.resolve({ state: Notification.permission }) :
                        originalQuery(parameters)
                    );
                    Object.defineProperty(navigator, 'plugins', {
                        get: () => [1, 2, 3, 4, 5],
                    });
                    const getParameter = WebGLRenderingContext.getParameter;
                    WebGLRenderingContext.prototype.getParameter = function(parameter) {
                        if (parameter === 37445) {
                            return 'Google Inc. (Intel)';
                        }
                        if (parameter === 37446) {
                            return 'ANGLE (Intel, Intel(R) UHD Graphics Direct3D11 vs_5_0 ps_5_0, D3D11)';
                        }
                        return getParameter(parameter);
                    };";

if (headless)
{
    cmdParams.Add("source", loadScript);
    driver.ExecuteCdpCommand("Page.addScriptToEvaluateOnNewDocument", cmdParams);
}

IJavaScriptExecutor loadChrome = (IJavaScriptExecutor)driver;

TornActions.Init(driver, wait);

//driver.Navigate().GoToUrl("https://intoli.com/blog/not-possible-to-block-chrome-headless/chrome-headless-test.html");
//driver.Navigate().GoToUrl("https://intoli.com/blog/making-chrome-headless-undetectable/chrome-headless-test.html");
//string thing = driver.FindElement(By.TagName("table")).GetAttribute("innerHTML");
//Console.WriteLine(thing);

TornActions.LogIn("christian.hensman1@gmail.com", "romeo007");
//TornActions.GymTrain("defense", 200);
//TornActions.Crimes("Grand Theft Auto", "Steal a Parked Car", 46);
TornActions.Fly("airstrip", "china");
TornActions.BuyAbroad("Panda Plushie", 44);

//CaptchaSolver();

Console.ReadLine();

