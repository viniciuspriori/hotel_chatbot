// See https://aka.ms/new-console-template for more information
using ChatBot;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using static ChatBot.Constants;

bool loginInfo = false;
var driver = new ChromeDriver(@"C:\chromedriver");
IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
Dictionary<string, int> users = new Dictionary<string, int>();
WebDriverWait wait;
Run();

void Run()
{
	var options = new ChromeOptions();
	options.AddArguments("--start-maximized");
	options.AddExcludedArgument("enable-automation");
	options.AddArgument("@user-data-dir=C:\\Users\\Username\\AppData\\Local\\Google\\Chrome\\User Data");
	driver.Navigate().GoToUrl("https://web.whatsapp.com");

	while (loginInfo == false)
	{
		CheckLoggedIn();
	}

	wait = new WebDriverWait(driver, TimeSpan.Zero);
	wait.Until(dr => dr.FindElement(By.Id("side")));

	//sleep

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
	var append = withBackSpace ? Keys.Backspace + Keys.Enter : Keys.Enter;

	IWebElement textBox = GetTextBoxToSend();
	jse.ExecuteScript($"arguments[0].innerHTML = '{text}'", textBox);
	textBox.SendKeys("." + append);
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

	string notRead = $"{GetLastInsideChatMessage} | {outSideChat}";
	string msg = $"{notRead}//ancestor::div[@{Elements.Parents.CLASS}='{Elements.USER_PANEL}']";

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


IWebElement FindElementByXPath(string elementName)
{
	return driver?.FindElement(By.XPath(elementName));
}
void Process()
{
	var body = ContainsInXPath(Elements.Parents.SPAN, Elements.Parents.ARIA_LABEL, Elements.USER_NAME_OR_NUMBER);
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


void ProcessMessage(string headerName)
{
	try
	{
		string element = GetLastInsideChatMessage() + $"//{Elements.Parents.SPAN}[@{Elements.Parents.CLASS}='{Elements.INNER_MESSAGE_CLASS}']";
		var text = FindElementByXPath(element).Text;

		int num = -1;

		int.TryParse(text, out num);


		if (users.GetValueOrDefault(headerName) == (int)UserState.Entered)
		{
			switch (num)
			{
				//case 1:
				//	jse.ExecuteScript($"arguments[0].innerHTML = '{mensaje_1()}'", textbox);
				//	lst_user[name_user] = 1;
				//	break;
				//case 2:
				//	jse.ExecuteScript($"arguments[0].innerHTML = '{mensaje_2()}'", textbox);
				//	lst_user[name_user] = 2;
				//	break;
				//case 3:
				//	jse.ExecuteScript($"arguments[0].innerHTML = '{mensaje_3()}'", textbox);
				//	lst_user[name_user] = 3;
				//	break;
				//case 4:
				//	jse.ExecuteScript($"arguments[0].innerHTML = '{mensaje_4()}'", textbox);
				//	lst_user[name_user] = 4;
				//	break;
				//case 0:
				//	jse.ExecuteScript($"arguments[0].innerHTML = '{mensaje_0()}'", textbox);
				//	lst_user.Remove(name_user);
				//	break;
				//default:
				//	jse.ExecuteScript($"arguments[0].innerHTML = '{mensaje_error()}'", textbox);
				//	break;
			}
		}


	}
	catch
	{
		ErrorMessage();
	}
}

void ErrorMessage()
{
	SendMessage(MessageHelper.ErrorMessage());
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