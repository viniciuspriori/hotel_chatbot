// See https://aka.ms/new-console-template for more information
using ChatBot;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V85.IndexedDB;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using static ChatBot.Constants;

bool loginInfo = false;
ChromeDriver driver;
Dictionary<string, int> users = new Dictionary<string, int>();
WebDriverWait wait;
string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
string cookiesPath = localAppData + "\\Google\\Chrome\\botSession\\";
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

		try
		{
			if (driver?.Title is null)
			{
				throw new NoSuchWindowException();
			}
		}
		catch (NoSuchWindowException)
		{
			driver?.Quit();
			driver.Dispose();
			Environment.Exit(0);
		}
	}
}

void SendMessage(string text)
{
	IWebElement textBox = GetTextBoxToSend();


	foreach (var item in text.Split("\n"))
	{
		textBox.SendKeys(item + Keys.Shift + Keys.Enter + Keys.Enter);
	}

	textBox.SendKeys(Keys.Enter);
}

string GetOutSideChat() => ContainsInXPath(Elements.Parents.SPAN, Elements.Parents.ARIA_LABEL, "Não lidas");

IWebElement GetElements(IWebDriver driver)
{
	string outSideChat = GetOutSideChat();

	IWebElement elementFound = null;
	try
	{
		var e = driver.FindElements(By.XPath("//div[contains(@class,'message-in focusable-list-item')]/following::span[contains(@class,'selectable-text')]/span"));

		if (e.Count > 0)
		{
			return e.Last();
		}
	}
	catch
	{
		elementFound = null;
	}

	try
	{
		elementFound = driver.FindElement(By.XPath(outSideChat));
		return elementFound;
	}
	catch
	{
		elementFound = null;
	}

	return elementFound;
}

bool GetNewMessage()
{
	string userPanel = $"{GetOutSideChat()}//ancestor::div[@{Elements.Parents.CLASS}='{Elements.USER_PANEL}']";
	try
	{
		wait.Until(GetElements);

		try
		{
			var element = driver?.FindElement(By.XPath(userPanel));
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
	catch
	{
		return false;
	}

	return false;
}


IWebElement FindElementByXPath(string elementName)
{
	return driver?.FindElement(By.XPath(elementName));
}
void Process()
{
	var body = "/html/body/div[1]/div/div/div[5]/div/header/div[2]/div/div/span";
	string? headerName = FindElementByXPath(body).Text;

	if (users.ContainsKey(headerName))
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
		var messagesIn = ContainsInXPath(Elements.Parents.DIV, Elements.Parents.CLASS, "message-in");

		var text = wait.Until(driver => driver.FindElements(By.XPath(messagesIn)).Last().Text).Substring(0, 1);
		//string contenido = driver.FindElement(By.XPath($"//div[contains(@aria-label, 'Lista de mensajes')]"
		//+ $"//div[last()][contains(@class, 'message-in')]//span[contains(@class, 'i0jNr selectable-text copyable-text')]")).Text;
		//var text = FindElementByXPath(element).Text;

		int num = -1;

		var res = int.TryParse(text, out num);
		if (res == false)
		{
			return;
		}
		else
		{
			var x = 2;
		}


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

				default:
					ErrorMessage();
					SendMessage(MessageHelper.WelcomeMessage());
					users[userName] = (int)UserState.Entered;
					break;

			}
		}

	}
	catch
	{

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
	SendMessage(MessageHelper.ErrorMessage());
}

void CheckLoggedIn()
{
	Thread.Sleep(5000);

	var body = ContainsInXPath(Elements.Parents.DIV, Elements.Parents.ARIA_LABEL, "foto do perfil");

	try
	{
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
	catch
	{

	}
}

IWebElement GetTextBoxToSend()
{
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

