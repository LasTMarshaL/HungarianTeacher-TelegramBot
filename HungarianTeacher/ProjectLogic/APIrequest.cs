using Google.Cloud.Translation.V2;
using Serilog;


class APIrequest // This class is responsiable for working with API
{
    private string _apiKey = "AIzaSyDHso9ATf4lg3U4Li0ROprvfAfZvtdw_Pw"; // Key to work with API
    public string apiKey
    {
        get
        {
            return _apiKey;
        }
    }

    private string _languageCode = "en";

    public string languageCode
    {
        get
        {
            return _languageCode;
        }
        set
        {
            _languageCode = value;
        }
    }
    public string SendRequestTranslation(string text, string languageCode) // Send api request to translate a text
    {
        try
        {
            TranslationClient client = TranslationClient.CreateFromApiKey(apiKey); // Create a client to work with API using key
            var response = client.TranslateText(text, languageCode); // Send request to translate a text

            return response.TranslatedText; // Return translated tex
        }
        catch
        {
            Log.Error("Exception while sending API request!"); // Print error message if something went wrong

            return "Google translater is not awailable now!"; // Return this text for mesage if something went wrong
        }
    }
}