// See https://aka.ms/new-console-template for more information
using ChatBot;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using static ChatBot.Constants;

bool loginInfo = false;
ChromeDriver driver;
Dictionary<string, int> users = new Dictionary<string, int>();
WebDriverWait wait;
string cookiesPath = @"C:\Users\Vinicius\AppData\Local\Google\Chrome\botSession\";
Run();

void Run()
{
	var options = new ChromeOptions();
	var arguments = new string[] { "--no-sandbox", "--disable-dev-shm-usage", $"user-data-dir={cookiesPath}", "--start-maximized" };
	options.AddArguments(arguments);
	options.AddExcludedArgument("enable-automation");
	
	driver = new ChromeDriver(@"C:\chromedriver", options);
	driver.Navigate().GoToUrl("https://web.whatsapp.com");

	while (loginInfo == false)
	{
		CheckLoggedIn();
	}

	wait = new WebDriverWait(driver, TimeSpan.FromMinutes(Timeout.Infinite));
	wait.Until(dr => dr.FindElement(By.Id("side")));

	Thread.Sleep(5000);

	while (true)
	{
		if (GetNewMessage())
		{
			Process();
		}
	}
}

void SendMessage(string text, bool withBackSpace = true)
{
	string append = withBackSpace ? Keys.Backspace + Keys.Enter : Keys.Enter;

	IWebElement textBox = GetTextBoxToSend();
	//IJavaScriptExecutor jse = driver;
	//var x = jse.ExecuteScript($"arguments[0].innerHTML = '{text}'", textBox);
	textBox.SendKeys(text + append);
}

string GetLastInsideChatMessage()
{
	var body1 = ContainsInXPath(Elements.Parents.DIV, Elements.Parents.CLASS, Elements.USER_MESSAGES_LIST);
	string insideChat = $"{body1}//div[last()][contains(@{Elements.Parents.CLASS}, 'message-in')]";

	return insideChat;
}

bool GetNewMessage()
{
	string outSideChat = ContainsInXPath(Elements.Parents.SPAN, Elements.Parents.ARIA_LABEL, "Não lidas");
	string notRead = $"{GetLastInsideChatMessage()} | {outSideChat}";
	string msg = $"{notRead}//ancestor::div[@{Elements.Parents.CLASS}='{Elements.USER_PANEL}']";

	try
	{
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
		catch (NoSuchElementException)
		{
			return false;
		}
	}
	catch { }

	return false;
}


IWebElement FindElementByXPath(string elementName)
{
	return driver?.FindElement(By.XPath(elementName));
}
void Process()
{
	var body = ContainsInXPath(Elements.Parents.SPAN, Elements.Parents.CLASS, Elements.USER_NAME_OR_NUMBER);
	string? headerName = FindElementByXPath(body).Text;

	if(users.ContainsKey(headerName))
	{
		ProcessMessage(headerName);
	}
	else
	{
		RegisterNewUser(users, headerName);
		SendMessage(MessageHelper.WelcomeMessage());
	}

	Thread.Sleep(2000);
}


void ProcessMessage(string userName)
{
	try
	{
		string element = GetLastInsideChatMessage() + $"//{Elements.Parents.SPAN}[@{Elements.Parents.CLASS}='{Elements.INNER_MESSAGE_CLASS}']";
		var text = FindElementByXPath(element).Text;

		int num = -1;

		int.TryParse(text, out num);


		if (users.GetValueOrDefault(userName) == (int)UserState.Entered)
		{
			switch ((SendOptions)num)
			{
				case SendOptions.MoreInfo:
					SendOptionsMessage();
					users[userName] = (int)UserState.ChooseDailyValues;
					break;

				default:
					ErrorMessage();
					users[userName] = (int)UserState.Entered;
					break;
			}
		}
		else if (users.GetValueOrDefault(userName) == (int)(UserState.ChooseDailyValues))
		{

			switch ((DailyOptions)num)
			{
				case DailyOptions.Normal:
					SendNormalDailyMessage();
					break;

				case DailyOptions.Weekend:
					SendWeekendDailyMessage();
					break;

			}
		}

	}
	catch
	{
		ErrorMessage();
	}
}

void SendNormalDailyMessage()
{
	SendMessage(MessageHelper.NormalDailyMessage());
}

void SendWeekendDailyMessage()
{
	SendMessage(MessageHelper.WeekendDailyMessage());
}

void SendOptionsMessage()
{
	SendMessage(MessageHelper.UserOptionsMessage());
}

void ErrorMessage()
{
	SendMessage(MessageHelper.ErrorMessage(), false);
}

void CheckLoggedIn()
{
	Thread.Sleep(5000);

	var body = ContainsInXPath(Elements.Parents.DIV, Elements.Parents.ARIA_LABEL, "foto do perfil");

	var checked1 = driver?.FindElements(By.XPath(body)).SingleOrDefault();
	if (checked1 == null)
	{
		loginInfo = false;
	}
	else
	{
		loginInfo = true;
	}
}

IWebElement GetTextBoxToSend()
{
	//return driver.FindElement(By.XPath(Elements.TEXT_BOX_SEND));
	return wait.Until(dr => dr.FindElement(By.XPath(Elements.TEXT_BOX_SEND)));
}

string ContainsInXPath(string element, string child, string key)
{
	return $"//{element}[contains(@{child}, '{key}')]";
}

void RegisterNewUser(Dictionary<string, int> users, string? headerName)
{
	users.Add(headerName, (int)UserState.Entered);
}

