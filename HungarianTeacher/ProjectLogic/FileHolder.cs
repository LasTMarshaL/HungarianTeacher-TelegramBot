class FileHolder
{
    private Random _random = new Random(); 

    public string FilePath { get; private set; } = Path.Combine(AppContext.BaseDirectory, "Assets"); 

    public string GetPictureFile() 
    {
        string[] files = Directory.GetFiles(FilePath, "*.*", SearchOption.TopDirectoryOnly); 

        int randomIndex = _random.Next(0, files.Length); 

        string randomPicture = files[randomIndex]; 

        return randomPicture; 
    }
}