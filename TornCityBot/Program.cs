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

//testing vars
bool enableSelenium = true;
Vosk.Vosk.SetLogLevel(-1);
//Console.WriteLine(SpeechRec());

//Start selenium
ChromeOptions options = new ChromeOptions();
options.AddArgument("--disable-blink-features=AutomationControlled");
options.AddExcludedArgument("enable-automation");
options.AddArgument("--mute-audio");
//string username = "chook";
//string userProfile = "C:/Users/" + username + "/AppData/Local/Google/Chrome/User Data";
//options.AddArguments("user-data-dir=" + userProfile);
options.AddArgument("--profile-directory=" + "Profile 2");

IWebDriver driver = new ChromeDriver(@"C:\ChromeDrivers\103\", options);
driver.Manage().Window.Maximize();

if (enableSelenium)
{
    
    driver.Navigate().GoToUrl("https://www.google.com/recaptcha/api2/demo");
    CaptchaSolver();

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

    driver.FindElement(By.Id("audio-response")).SendKeys(SpeechRec());
    driver.FindElement(By.Id("audio-response")).SendKeys(Keys.Enter);
    RandomWait(8, 12);
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

void RandomWait(int a, int b)
{
    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(a + new Random().NextDouble() * 5);
}