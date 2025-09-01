namespace Architexor.Core
{
	public struct ATXUser
	{
		public string Id;
		public string FirstName;
		public string LastName;
		public string Email;
		public string Token;
		public string DeviceId;
	}

	public static class Constants
	{
		public const string BRAND = "Architexor";
		public const string CONTACT_PRODUCT_MANAGER = "chris@futuretimber.com";
		public const string CONTACT_DEVELOPER = "johnpeace200@gmail.com";
		public const string FRONTEND = "http://193.203.202.249:81";
		public const string BACKEND = "https://193.203.202.249:81";
		public const string API_ENDPOINT = "http://193.203.202.249:81/";
		public const string AI_ENGINE_ENDPOINT = "http://193.203.202.249:82/";
		public const string FontFamily = "Calibri";
		public static ATXUser thisUser = new ATXUser();
	}
}
