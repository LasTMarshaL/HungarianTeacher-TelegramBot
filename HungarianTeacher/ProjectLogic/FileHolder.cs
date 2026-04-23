class FileHolder // This class is responsiable for files control.
{
    private Random _random = new Random(); 

    public string FilePath { get; private set; } = Path.Combine(AppContext.BaseDirectory, "Assets"); 

    /// <summary>
    /// Selects and returns the path of a random file from the directory specified by the FilePath property.
    /// </summary>
    public string GetPictureFile() 
    {
        string[] files = Directory.GetFiles(FilePath, "*.*", SearchOption.TopDirectoryOnly); 

        int randomIndex = _random.Next(0, files.Length); 

        string randomPicture = files[randomIndex]; 

        return randomPicture; 
    }
}