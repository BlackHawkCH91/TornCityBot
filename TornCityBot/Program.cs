#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
//Libraries
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.DevTools;
using NAudio.Wave;
using System.Diagnostics;
using Vosk;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
    
//testing vars
bool headless = true;
Vosk.Vosk.SetLogLevel(-1);
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

//driver.Navigate().GoToUrl("https://intoli.com/blog/not-possible-to-block-chrome-headless/chrome-headless-test.html");
//driver.Navigate().GoToUrl("https://intoli.com/blog/making-chrome-headless-undetectable/chrome-headless-test.html");
//string thing = driver.FindElement(By.TagName("table")).GetAttribute("innerHTML");
//Console.WriteLine(thing);

LogIn("christian.hensman1@gmail.com", "romeo007");
GymTrain("strength", 200);

//CaptchaSolver();

Console.ReadLine();

void LogIn(string username, string password)
{
    //Bypasses browser check
    DriverNavigate("https://www.torn.com/2644601");
    ThreadRandomWait(2, 3);

    //If user is not logged in
    if (driver.Url != "https://www.torn.com/index.php")
    {
        driver.FindElement(By.Id("player")).SendKeys(username);
        driver.FindElement(By.Id("password")).SendKeys(password);
        ThreadRandomWait(7, 10);
        driver.FindElement(By.Id("password")).SendKeys(Keys.Enter);
    }
}

void GymTrain(string stat, int amount)
{
    //Check if user is already in gym
    if (driver.Url != "https://www.torn.com/gym.php")
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[href*='/gym.php']")));
        ThreadRandomWait(1, 2);
        driver.FindElement(By.CssSelector("a[href*='/gym.php']")).Click();
        ThreadRandomWait(1, 1.5);

        //Complete captcha if there is one
        try
        {
            driver.FindElement(By.Id("ui-id-3")).Click();
            ThreadRandomWait(1, 1.5);
            CaptchaSolver();
            ThreadRandomWait(1, 1.5);
            driver.FindElement(By.XPath("//input[@name='reCaptcha']")).Click();
            ThreadRandomWait(1, 1.5);
        } 
        catch (Exception ex) { }
    }

    List<IWebElement> ul = driver.FindElements(By.TagName("ul")).ToList();
    List<IWebElement> training = ul[ul.Count - 1].FindElements(By.TagName("li")).ToList();
    string[] types = new string[] { "strength", "defense", "speed", "dexterity" };
    //training[Array.IndexOf(types, "stat") + 1].FindElement(By.XPath($"//button[@aria-label='Train {types[Array.IndexOf(types, "stat")].ToLower()}']"));
    training[Array.IndexOf(types, stat)].FindElement(By.TagName("input")).SendKeys(Keys.Control + "a");
    ThreadRandomWait(0.5, 1);
    training[Array.IndexOf(types, stat)].FindElement(By.TagName("input")).SendKeys(amount.ToString());
    ThreadRandomWait(0.5, 1);
    training[Array.IndexOf(types, stat)].FindElements(By.TagName("button"))[2].Click();
    ThreadRandomWait(1, 1.5);
}


void DriverNavigate(string url)
{
    try
    {
        driver.Navigate().GoToUrl(url);
    }
    catch (Exception ex) { }
}


void CaptchaSolver()
{
    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
    List<IWebElement> iFrames = driver.FindElements(By.TagName("iframe")).ToList();

    //Open the captcha
    IWebDriver captchaFrame = driver.SwitchTo().Frame(iFrames[0]);
    captchaFrame.FindElement(By.ClassName("recaptcha-checkbox-border")).Click();
    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);


    //Open audio captcha
    driver.SwitchTo().DefaultContent();
    iFrames = driver.FindElements(By.TagName("iframe")).ToList();
    driver.SwitchTo().Frame(iFrames[2]).FindElement(By.Id("recaptcha-audio-button")).Click();
    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
    //recaptcha-audio-button

    //Get link for audio and download it
    driver.SwitchTo().DefaultContent();
    iFrames = driver.FindElements(By.TagName("iframe")).ToList();
    driver.SwitchTo().Frame(iFrames[iFrames.Count - 1]);
    string audioUrl = driver.FindElement(By.Id("audio-source")).GetAttribute("src");

    using (var client = new System.Net.WebClient())
    {
        client.DownloadFile(audioUrl, "audio.mp3");
    }

    string audioResponse = SpeechRec();

    if (string.IsNullOrEmpty(audioResponse) || driver.FindElement(By.ClassName("rc-audiochallenge-error-message")).Displayed || audioResponse.Split(" ").Length <= 1)
    {
        driver.Navigate().Refresh();
        CaptchaSolver();
    }
    else
    {
        driver.FindElement(By.Id("audio-response")).SendKeys(audioResponse);
        ThreadRandomWait(5, 7);
        driver.FindElement(By.Id("audio-response")).SendKeys(Keys.Enter);
    }
    driver.SwitchTo().DefaultContent();
}

string SpeechRec()
{
    int bitRate;
    //Convert mp3 to wav
    using (Mp3FileReader mp3 = new Mp3FileReader("audio.mp3"))
    {
        using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
        {
            WaveFileWriter.CreateWaveFile("audio.wav", pcm);
        }
    }

    //Convert the wav from stereo to mono
    using (var waveFileReader = new WaveFileReader("audio.wav"))
    {
        var outFormat = new WaveFormat(waveFileReader.WaveFormat.SampleRate, 1);
        using (var resampler = new MediaFoundationResampler(waveFileReader, outFormat))
        {
            WaveFileWriter.CreateWaveFile("audio2.wav", resampler);
        }

        bitRate = waveFileReader.WaveFormat.SampleRate;
    }

    //Create Vosk STT
    Model model = new Model("model");
    VoskRecognizer rec = new VoskRecognizer(model, bitRate);
    rec.SetMaxAlternatives(0);
    rec.SetWords(true);

    using (Stream source = File.OpenRead("audio2.wav"))
    {
        byte[] buffer = new byte[4096];
        int bytesRead;
        while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
        {
            if (rec.AcceptWaveform(buffer, bytesRead))
            {
                rec.Result();
            }
            else
            {
                rec.PartialResult();
            }
        }
    }
    return (dynamic)JObject.Parse(rec.FinalResult())["text"];
}

void ImpRandomWait(double a, double b)
{
    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(a + new Random().NextDouble() * 5);
}

void ThreadRandomWait(double a, double b)
{
    Thread.Sleep(Convert.ToInt32((a + new Random().NextDouble() * 5) * 1000));
}