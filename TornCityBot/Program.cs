#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
//Libraries
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using NAudio.Wave;
using System.Diagnostics;
using Vosk;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using System.Net;

//testing vars
bool enableSelenium = true;
Vosk.Vosk.SetLogLevel(-1);

TornCityBot.Proxy.StartProxy();

var seleniumProxy = new OpenQA.Selenium.Proxy 
{
    HttpProxy = "http://localhost:18882",
    SslProxy = "http://localhost:18882",
    FtpProxy = "http://localhost:18882"
};


//Start selenium
ChromeOptions options = new ChromeOptions();
string chromeTest = "{loadTimes: ƒ, csi: ƒ}app: ObjectInstallState: {DISABLED: 'disabled', INSTALLED: 'installed', NOT_INSTALLED: 'not_installed'}DISABLED: \"disabled\"INSTALLED: \"installed\"NOT_INSTALLED: \"not_installed\"[[Prototype]]: ObjectRunningState: {CANNOT_RUN: 'cannot_run', READY_TO_RUN: 'ready_to_run', RUNNING: 'running'}getDetails: ƒ getDetails()getIsInstalled: ƒ getIsInstalled()installState: ƒ installState()isInstalled: (...)runningState: ƒ runningState()get isInstalled: ƒ ()[[Prototype]]: Objectcsi: ƒ ()arguments: nullcaller: nulllength: 0name: \"\"prototype: {constructor: ƒ}[[FunctionLocation]]: ​[[Prototype]]: ƒ ()[[Scopes]]: Scopes[0]loadTimes: ƒ ()get app: ƒ ()set app: ƒ ()[[Prototype]]: Object";
string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/AppData/Local/Google/Chrome/User Data/Profile 4";
options.AddArguments("user-data-dir=" + userProfile);
options.AddArgument("--profile-directory=Default");
options.AddArgument("--disable-blink-features=AutomationControlled");
options.AddArgument("--mute-audio");
//options.AddArgument("headless");
options.AddArgument("ignore-certificate-errors");
options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
options.AddExcludedArgument("enable-automation");
options.Proxy = seleniumProxy;


Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

IWebDriver driver = new ChromeDriver(@"C:\ChromeDrivers\103\", options);
driver.Manage().Window.Maximize();

IJavaScriptExecutor loadChrome = (IJavaScriptExecutor)driver;


if (enableSelenium)
{
    //driver.Navigate().GoToUrl("https://intoli.com/blog/not-possible-to-block-chrome-headless/chrome-headless-test.html");
    //await Navigate(driver, "");
    //((IJavaScriptExecutor)driver).ExecuteAsyncScript("window.chrome = {runtime: {}};");
    driver.Navigate().GoToUrl("https://intoli.com/blog/not-possible-to-block-chrome-headless/chrome-headless-test.html");
    /*await Task.Run(() =>
    {
    });*/
    string thing = driver.FindElement(By.TagName("table")).GetAttribute("innerHTML");
    Console.WriteLine(thing);
    //driver.Navigate().GoToUrl("https://www.torn.com/");
    //driver.Navigate().GoToUrl("https://www.google.com/search?q=torn+city&sxsrf=ALiCzsZqGtZSjv_wU_hn-WCGoU0qA0KI8g%3A1658213089644&source=hp&ei=4VLWYuSrJIPD4-EP2b-1-Ac&iflsig=AJiK0e8AAAAAYtZg8VBsoiuG5qQNXZqLHl8t6129ARAv&ved=0ahUKEwikoKuRrYT5AhWD4TgGHdlfDX8Q4dUDCAk&uact=5&oq=torn+city&gs_lcp=Cgdnd3Mtd2l6EAMyBAgjECcyBAgjECcyBAgjECcyCwguEIAEELEDEIMBMgUIABCABDIFCAAQgAQyBQgAEIAEMgUIABCABDIFCAAQgAQyBQgAEIAEOgcIIxDqAhAnOgUIABCRAjoRCC4QgAQQsQMQgwEQxwEQ0QM6CwgAEIAEELEDEIMBOgQIABBDOggILhCABBCxAzoKCAAQsQMQgwEQQzoECC4QQzoHCAAQsQMQQzoKCAAQsQMQyQMQQzoLCC4QgAQQsQMQ1AI6BwgAEIAEEAo6CgguELEDEIMBEEM6CAgAEIAEELEDOgUILhCABFCvBFjgEWDrE2gCcAB4AIABmwKIAe4QkgEFMC41LjWYAQCgAQGwAQo&sclient=gws-wiz");
    //ThreadRandomWait(2, 3);
    //driver.FindElement(By.TagName("h3")).FindElement(By.XPath("./..")).Click();

    //CaptchaSolver();

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

void ImpRandomWait(int a, int b)
{
    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(a + new Random().NextDouble() * 5);
}

void ThreadRandomWait(int a, int b)
{
    Thread.Sleep(Convert.ToInt32((a + new Random().NextDouble() * 5) * 1000));
}