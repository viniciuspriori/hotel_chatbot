using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot
{
	public static class Constants
	{

		public enum DailyOptions
		{ 
			Normal = 1,
			Weekend = 2
		}


		public enum SendOptions
		{
			MoreInfo = 1,
		}


		public enum UserState
		{ 
			Entered = 0,
			SendOptions,
			ChooseDailyValues,
		}

	}
}
