class FileHolder // This class is responsiable for files
{
    Random random = new Random(); // Random

    private string _filePath = Path.Combine(AppContext.BaseDirectory, "Assets"); // Get path to the folder with pictures
    public string filePath 
    {
        get
        {
            return _filePath;
        }
        set
        {
            _filePath = value;
        }

    }

    public string GetPictureFile() // Method to get random pictore from folder
    {
        string[] files = Directory.GetFiles(filePath, "*.*", SearchOption.TopDirectoryOnly); // Make list of  all files ("*.*" - all files)

        int randomIndex = random.Next(0, files.Length); // Rendom index

        string randomPicture = files[randomIndex]; // Get picture using random index

        return randomPicture; // return file path of this picture
    }
}