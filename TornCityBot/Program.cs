#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
//Libraries
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Speech.Recognition;
using NAudio.Wave;


string RecognisedText = "test";
SpeechRec("");

while (true)
{
    Console.WriteLine("ree");
}


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



void SpeechRec(string file)
{
    using (Mp3FileReader mp3 = new Mp3FileReader(file + "audio.mp3"))
    {
        using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
        {
            WaveFileWriter.CreateWaveFile(file + "audio.wav", pcm);
        }
    }


    using (SpeechRecognitionEngine recogniser = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US")))
    {
        #pragma warning restore CS8622
        bool completed = false;
        recogniser.LoadGrammar(new DictationGrammar());

        recogniser.SetInputToWaveFile(file + "audio.wav");


        recogniser.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recogniser_SpeechRecognized);
        recogniser.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(recogniser_RecognisedCompleted);

        //recogniser.SetInputToDefaultAudioDevice();

        recogniser.RecognizeAsync();

        while (completed)
        {
            Console.ReadLine();
        }
    }

}

static void recogniser_RecognisedCompleted(object sender, RecognizeCompletedEventArgs e)
{
    if (e.Error != null)
    {
        Console.WriteLine($"Error encountered {e.Error.GetType().Name}: {e.Error.Message}");
    }

    if (e.Cancelled)
    {
        Console.WriteLine("Operation cancelled");
    }
    if (e.InputStreamEnded)
    {
        Console.WriteLine("End of stream encountered");
    }
} 


void recogniser_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
{

    if (e.Result != null && e.Result.Text != null) 
    {
        RecognisedText = e.Result.Text;
    } else
    {
        Console.WriteLine("Text not available");
    }
}