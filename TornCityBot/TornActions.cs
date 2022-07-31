using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using SeleniumExtras.WaitHelpers;

namespace TornCityBot
{
    public static class TornActions
    {
        public static Dictionary<string, string[]> crimeList = new Dictionary<string, string[]>();
        static ChromeDriver driver;
        static WebDriverWait wait;
        
        //Setup chromedriver and the wait
        public static void Init(ChromeDriver _driver, WebDriverWait _wait)
        {
            driver = _driver ?? throw new ArgumentNullException(nameof(driver));
            wait = _wait ?? throw new ArgumentNullException(nameof(wait));

            //Sorry for spaghet. Complete list of all crimes for future gui.
            crimeList.Add("Search for Cash", new string[] { "Search the Train Station", "Search Under the Old Bridge", "Search the Bins", "Search the Water Fountain", "Search the Dumpsters", "Search the Movie Theater" });
            crimeList.Add("Sell Copied Media", new string[] { "Rock CDs", "Heavy Metal CDs", "Pop CDs", "Rap CDs", "Reggae CDs", "Horror DVDs", "Action DVDs", "Romance DVDs", "Sci Fi DVDs", "Thriller DVDs" });
            crimeList.Add("Shoplift", new string[] { "Sweet Shop", "Market Stall", "Clothes Shop", "Jewelry Shop" });
            crimeList.Add("Pickpocket Someone", new string[] { "Hobo", "Kid", "Old Woman", "Businessman", "Lawyer" });
            crimeList.Add("Larceny", new string[] { "Apartment", "Detached House", "Mansion", "Cars", "Office" });
            crimeList.Add("Armed Robberies", new string[] { "Swift Robbery", "Thorough Robbery", "Swift Convenience", "Thorough Convenience", "Swift Bank", "Thorough Bank", "Swift Armored Car", "Thorough Armored Car" });
            crimeList.Add("Transport Drugs", new string[] { "Transport Cannabis", "Transport Amphetamines", "Transport Cocaine", "Sell Cannabis", "Sell Pills", "Sell Cocaine" });
            crimeList.Add("Plant a Computer Virus", new string[] { "Simple Virus", "Polymorphic Virus", "Tunneling Virus", "Armored Virus", "Stealth Virus" });
            crimeList.Add("Assassination", new string[] { "Assassinate a Target", "Drive-by Shooting", "Car Bomb", "Mob Boss" });
            crimeList.Add("Arson", new string[] { "Home", "Car Lot", "Office Building", "Apartment Building", "Warehouse", "Motel", "Government Building" });
            crimeList.Add("Grand Theft Auto", new string[] { "Steal a Parked Car", "Hijack a Car", "Steal Car from Showroom" });
            crimeList.Add("Pawn Shop", new string[] { "Side Door", "Rear Door" });
            crimeList.Add("Counterfeiting", new string[] { "Money", "Casino Tokens", "Credit Card" });
            crimeList.Add("Kidnapping", new string[] { "Kid", "Woman", "Undercover Cop", "Mayor" });
            crimeList.Add("Arms Trafficking", new string[] { "Explosives", "Firearms" });
            crimeList.Add("Bombings", new string[] { "Bomb a Factory", "Bomb a Government Building" });
            crimeList.Add("Hacking", new string[] { "Hack into a Bank Mainframe", "Hack the F.B.I Mainframe" });
        }

        //General purpose func to auto wait for element to exist before clicking/sending keys.
        public static void WebElementInput(By findElement, IWebElement? parent = null, string? input = null, Action? threadRandomWait = null, bool dontClear = false)
        {
            //Wait till element exists
            IWebElement element;
            
            if (parent == null) {
                wait.Until(ExpectedConditions.ElementExists(findElement));
                element = driver.FindElement(findElement);
            } else
            {
                wait.Until(ExpectedConditions.ElementExists(findElement));
                element = parent.FindElement(findElement);
            }

            if (string.IsNullOrEmpty(input))
            {
                element.Click();
            } else
            {
                if (!dontClear)
                {
                    element.SendKeys(Keys.Control + "a");
                }
                element.SendKeys(input);
            }

            if (threadRandomWait != null)
            {
                threadRandomWait();
            }
        }

        public static void CheckForCaptcha()
        {
            bool complete = false;
            //Complete captcha if there is one
            try
            {
                while (!complete)
                {
                    //WebElementInput(By.Id("ui-id-3"), null, null, () => ThreadRandomWait(1, 1.5));
                    driver.FindElement(By.Id("ui-id-3")).Click();
                    ThreadRandomWait(1, 1.5);
                    CaptchaSolver(ref complete);
                    if (!complete)
                    {
                        continue;
                    }

                    ThreadRandomWait(1, 1.5);
                    WebElementInput(By.XPath("//input[@name='reCaptcha']"), null, null, () => ThreadRandomWait(1, 1.5));
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        public static void Fly(string plane, string city)
        {
            //WebElementInput(By.CssSelector("a[href*='/city.php']"), null, () => ThreadRandomWait(1, 2));
            //Apparent if class 't-red' is greater than 11, OC is about to start
            if (driver.Url != "https://www.torn.com/city.php")
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href*='/city.php']")));
                ThreadRandomWait(1, 2);
                driver.FindElement(By.CssSelector("a[href*='/city.php']")).Click();
                ThreadRandomWait(1, 1.5);
                driver.FindElement(By.CssSelector("a[href*='/travelagency.php']")).Click();
                ThreadRandomWait(1, 1.5);

                CheckForCaptcha();
            }

            //item cap
            //delimiter[1]
            //span[2] / span[3]

            wait.Until(ExpectedConditions.ElementExists(By.ClassName("travel-agency")));
            ThreadRandomWait(1, 1.5);
            driver.FindElement(By.ClassName(plane)).Click();
            ThreadRandomWait(0.5, 0.75);
            IWebElement map = driver.FindElement(By.CssSelector($"div[type*='{plane}']"));
            map.FindElement(By.ClassName(city)).Click();
            ThreadRandomWait(1, 1.5);
            map.FindElement(By.ClassName("torn-btn")).Click();
            ThreadRandomWait(0.7, 1);
            map.FindElements(By.ClassName("torn-btn"))[1].Click();
            //ThreadRandomWait(1, 2);
            Thread.Sleep(15000);
            //WebElementInput(By.ClassName("torn-btn"), map, null, () => ThreadRandomWait(2, 3));
            map.FindElements(By.ClassName("torn-btn"))[1].Click();
            wait.Until(ExpectedConditions.ElementExists(By.Id("countrTravel")));
            Thread.Sleep(10000);
            string travelTime = driver.FindElement(By.Id("countrTravel")).Text;
            DriverNavigate("https://www.google.com");
            Thread.Sleep(TimeSpan.Parse(travelTime) + TimeSpan.FromMinutes(1));
            //countrTravel
        }

        public static void BuyAbroad(string item, int amount)
        {
            LogIn("christian.hensman1@gmail.com", "romeo007");
            //Complete captcha if there is one
            CheckForCaptcha();

            IWebElement shop = driver.FindElement(By.CssSelector("ul[role*='tablist']"));
            bool itemExists = false;
            IWebElement child = null;

            while (!itemExists)
            {
                try
                {
                    //aria-label="Buy: {item}"
                    //child = driver.FindElement(By.XPath($"(//*[contains(., '{item}')])"));
                    child = driver.FindElement(By.CssSelector($"a[aria-label*='Buy: {item}']"));
                    itemExists = true;
                } catch {
                    DriverNavigate("https://www.google.com");
                    Thread.Sleep(900000);
                    LogIn("christian.hensman1@gmail.com", "romeo007");
                }
            }

            //IWebElement buyAmount = child.FindElement(By.ClassName("deal"));
            IWebElement buyAmount = child.FindElement(By.XPath(".."));

            buyAmount.FindElement(By.TagName("input")).SendKeys(Keys.Control + "a");
            buyAmount.FindElement(By.TagName("input")).SendKeys(amount.ToString());
            ThreadRandomWait(1, 1.5);
            buyAmount.FindElement(By.TagName("a")).Click();
            ThreadRandomWait(1, 1.5);

            //wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[. = 'BUY']")));
            //buyAmount.FindElement(By.XPath("//button[. = 'BUY']")).Click();
            driver.FindElement(By.ClassName("expanded")).FindElement(By.ClassName("torn-btn")).Click();
            ThreadRandomWait(3, 4);
            driver.FindElement(By.ClassName("travel-home")).Click();
            ThreadRandomWait(1, 1.5);
            driver.FindElement(By.XPath("//button[. = 'TRAVEL BACK']")).Click();
            Thread.Sleep(10000);
            wait.Until(ExpectedConditions.ElementExists(By.Id("countrTravel")));
            string time = driver.FindElement(By.Id("countrTravel")).Text;
            Console.WriteLine(time);
            DriverNavigate("https://www.google.com");
            Thread.Sleep(TimeSpan.Parse(time));
            Console.WriteLine(TimeSpan.Parse(time).TotalSeconds);
        }

        public static void LogIn(string username, string password)
        {
            Console.WriteLine("Logging in");
            //Bypasses browser check
            DriverNavigate("https://www.torn.com/2644601");
            ThreadRandomWait(1, 1.5);

            //If user is not logged in
            if (driver.Url != "https://www.torn.com/index.php")
            {
                WebElementInput(By.Id("player"), null, username);
                //driver.FindElement(By.Id("player")).SendKeys(username);
                driver.FindElement(By.Id("password")).SendKeys(password);
                ThreadRandomWait(7, 10);
                driver.FindElement(By.Id("password")).SendKeys(Keys.Enter);
            }
        }

        public static void GymTrain(string stat, int amount)
        {
            Console.WriteLine($"Training {amount} {stat}");
            //Check if user is already in gym
            if (driver.Url != "https://www.torn.com/gym.php")
            {
                WebElementInput(By.CssSelector("a[href*='/gym.php']"), null, null, () => ThreadRandomWait(1, 2));
                //Complete captcha if there is one
                CheckForCaptcha();
            }

            List<IWebElement> ul = driver.FindElements(By.TagName("ul")).ToList();
            List<IWebElement> training = ul[ul.Count - 1].FindElements(By.TagName("li")).ToList();
            string[] types = new string[] { "strength", "defense", "speed", "dexterity" };

            WebElementInput(By.TagName("input"), training[Array.IndexOf(types, stat)], amount.ToString(), () => ThreadRandomWait(1, 1.5));
            training[Array.IndexOf(types, stat)].FindElements(By.TagName("button"))[2].Click();
            ThreadRandomWait(1, 1.5);
        }

        public static void Crimes(string crime, string type, int nerve)
        {
            if (driver.Url != "https://www.torn.com/crimes.php#/step=main")
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href*='/crimes.php#/step=main']")));
                ThreadRandomWait(1, 2);
                driver.FindElement(By.CssSelector("a[href*='/crimes.php#/step=main']")).Click();
                ThreadRandomWait(1, 1.5);

                //Complete captcha if there is one
                CheckForCaptcha();
            }
            ThreadRandomWait(1, 1.5);
            wait.Until(ExpectedConditions.ElementExists(By.XPath("//form[@name='crimes']")));
            driver.FindElement(By.XPath("//form[@name='crimes']")).FindElement(By.XPath($"//li[normalize-space() = '{crime}']")).FindElement(By.XPath("..//..")).Click();

            double commitAmount = nerve / (crimeList.Keys.ToList().IndexOf(crime) + 3);
            int amount = (int)Math.Floor(commitAmount) - 1;

            ThreadRandomWait(1, 1.5);
            driver.FindElement(By.XPath("//form[@name='crimes']")).FindElement(By.XPath($"//li[normalize-space() = '{type}']")).FindElement(By.XPath("..//..")).Click();

            for (int i = 0; i < amount; i++)
            {
                WebElementInput(By.XPath("//button[normalize-space() = 'TRY AGAIN']"), null, null, () => ThreadRandomWait(0.5, 0.65));
            }
        }



        public static void DriverNavigate(string url)
        {
            Console.WriteLine($"Navigating to {url}");
            try
            {
                driver.Navigate().GoToUrl(url);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        public static void CaptchaSolver(ref bool complete)
        {
            Console.WriteLine($"Solving captcha");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            List<IWebElement> iFrames = driver.FindElements(By.TagName("iframe")).ToList();

            //Open the captcha
            driver.SwitchTo().Frame(iFrames[0]);
            WebElementInput(By.ClassName("recaptcha-checkbox-border"), null, null, () => ThreadRandomWait(2, 3));

            //Open audio captcha
            driver.SwitchTo().DefaultContent();
            iFrames = driver.FindElements(By.TagName("iframe")).ToList();
            driver.SwitchTo().Frame(iFrames[2]);

            WebElementInput(By.Id("recaptcha-audio-button"), null, null, () => ThreadRandomWait(2, 2.5));

            //Get link for audio and download it
            driver.SwitchTo().DefaultContent();
            iFrames = driver.FindElements(By.TagName("iframe")).ToList();
            driver.SwitchTo().Frame(iFrames[iFrames.Count - 1]);

            wait.Until(ExpectedConditions.ElementExists(By.Id("audio-source")));
            string audioUrl = driver.FindElement(By.Id("audio-source")).GetAttribute("src");

            using (var client = new System.Net.WebClient())
            {
                client.DownloadFile(audioUrl, "audio.mp3");
            }

            string audioResponse = SpeechRec();
            Console.WriteLine($"Speech Rec: {audioResponse}");

            if (string.IsNullOrEmpty(audioResponse) || driver.FindElement(By.ClassName("rc-audiochallenge-error-message")).Displayed || audioResponse.Split(" ").Length <= 1)
            {
                Console.WriteLine($"Speech Rec: Failed, retrying");
                driver.Navigate().Refresh();
                //CaptchaSolver();
            }
            else
            {
                Console.WriteLine($"SpeechRec: Successful");
                WebElementInput(By.Id("audio-response"), null, audioResponse, () => ThreadRandomWait(5, 7));
                driver.FindElement(By.Id("audio-response")).SendKeys(Keys.Enter);
                complete = true;
            }
            driver.SwitchTo().DefaultContent();
        }

        public static string SpeechRec()
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

        public static void ImpRandomWait(double a, double b)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(a + new Random().NextDouble() * (b - a));
        }

        public static void ThreadRandomWait(double a, double b)
        {
            Thread.Sleep(Convert.ToInt32((a + new Random().NextDouble() * (b - a)) * 1000));
        }
    }
}
