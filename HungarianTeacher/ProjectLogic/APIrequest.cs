using Google.Cloud.Translation.V2;
using Serilog;


class APIrequest 
{
    private string _apiKey = "Your key";
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

    public string SendRequestTranslation(string text, string languageCode) 
    {
        try
        {
            TranslationClient client = TranslationClient.CreateFromApiKey(apiKey);
            var response = client.TranslateText(text, languageCode); 

            return response.TranslatedText;
        }
        catch
        {
            Log.Error("Exception while sending API request!");

            return "Google translater is not awailable now!"; 
        }
    }
}