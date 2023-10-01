using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot
{
	public static class MessageHelper
	{
		public static string ErrorMessage() => "Desculpe! Não reconheci a opção fornecida. Podemos recomeçar?";

		//0
		public static string WelcomeMessage() => "Bem-vindo! Digite *1* para informações sobre as diárias do Hotel.";

		//1
		public static string UserOptionsMessage() => "A diária é cobrada por noite dormida." + "\n" + "*1*. Informações das diárias de segunda-feira a quinta-feira" + "\n" + "*2*. Informações das diárias de sexta-feira a domingo";

		//2.1
		public static string NormalDailyMessage() => "Diária dos períodos entre segunda-feira e quinta-feira: *R$ 290,00*";
		//2.2
		public static string WeekendDailyMessage() => "Diária de fim de semana (sexta-feira a domingo): *R$330,00*" + "\n" + "*ATENÇÃO*: No fim de semana vendemos no mínimo *duas* diárias, de sexta a domingo ou sábado a segunda.";

	}

	//"Diaria é cobrada por noite dormida,
	//diaria de dia de semana hoje é 290 e fim de semana é 330, fim de semana é considerado de sexta a domingo,

	//ou seja 02 diárias, de domingo a segunda já é considerado dia de semana.
	//Outra variavel é que aos fins de semana vendemos somente minimo de 02 diarias, de sexta a domingo ou sabado a segunda";
}

