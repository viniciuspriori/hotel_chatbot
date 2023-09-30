// See https://aka.ms/new-console-template for more information
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

bool loginInfo = false;
var driver = new ChromeDriver(@"C:\chromedriver");

Run();

void CheckLoggedIn()
{
	Thread.Sleep(5000);
	var checked1 = driver?.FindElements(By.XPath("//div[contains(@aria-label, 'foto do perfil')]")).SingleOrDefault();
	if (checked1 == null)
	{
		loginInfo = false;
	}
	else
	{
		loginInfo = true;
	}
}

void Run()
{
	var options = new ChromeOptions();
	options.AddArguments("--start-maximized");
	options.AddExcludedArgument("enable-automation");
	driver.Navigate().GoToUrl("https://web.whatsapp.com");

	while (loginInfo == false)
	{
		CheckLoggedIn();
	}

	var wait = new WebDriverWait(driver, TimeSpan.Zero);
	wait.Until(dr => dr.FindElement(By.Id("side")));

	while (true)
	{
		if (GetNewMessage(driver, wait))
		{
			var writeMsg = wait.Until(dr => dr.FindElement(By.XPath("/html/body/div[1]/div/div/div[5]/div/footer/div[1]/div/span[2]/div/div[2]/div[1]/div/div[1]")));

			writeMsg.SendKeys("Bem vindos a pousada LeandroTrones!" + Keys.Enter);
		}
	}
}

static bool GetNewMessage(ChromeDriver driver, WebDriverWait wait)
{
	//<span class="l7jjieqr cfzgl7ar ei5e7seu h0viaqh7 tpmajp1w c0uhu3dl riy2oczp dsh4tgtl sy6s5v3r gz7w46tb lyutrhe2 qfejxiq4 fewfhwl7 ovhn1urg ap18qm3b ikwl5qvt j90th5db aumms1qt" aria-label="Não lidas">1</span>

	string insideChat = "//div[contains(@class, 'n5hs2j7m oq31bsqd gx1rr48f qh5tioqs')]//div[last()][contains(@class, 'message-in')]";
	string outSideChat = "//span[contains(@aria-label, 'Não lidas')]";

	string notRead = $"{insideChat} | {outSideChat}";
	string msg = notRead + "//ancestor::div[@class='_8nE1Y']";

	wait.Until(dr => dr.FindElement(By.XPath(notRead)));

	var msgReceived = By.XPath(msg);
	try
	{
		var element = driver?.FindElement(msgReceived);
		if (element != null)
		{
			element.Click();
			return true;
		}
	} 
	catch(NoSuchElementException) 
	{
		return false;
	}

	return false;

}