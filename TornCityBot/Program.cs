#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
//Libraries
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using NAudio.Wave;

//testing vars
bool enableSelenium = false;

string RecognisedText = "test";

if (enableSelenium)
{
    //Start selenium
    ChromeOptions options = new ChromeOptions();
    options.AddArgument("--disable-blink-features=AutomationControlled");
    options.AddExcludedArgument("enable-automation");
    string username = "chook";
    string userProfile = "C:/Users/" + username + "/AppData/Local/Google/Chrome/User Data";
    //options.AddArguments("user-data-dir=" + userProfile);
    options.AddArgument("--profile-directory=" + "Profile 2");

    IWebDriver driver = new ChromeDriver(@"C:\ChromeDrivers\103\", options);

    driver.Navigate().GoToUrl("https://www.google.com/recaptcha/api2/demo");

    //Captcha solver

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

    //id = audio-source
    //title = recaptcha challenge expires in two minutes
}



void SpeechRec(string file)
{
    using (Mp3FileReader mp3 = new Mp3FileReader(file + "audio.mp3"))
    {
        using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
        {
            WaveFileWriter.CreateWaveFile(file + "audio.wav", pcm);
        }
    }
}