using TrekBucketListApp.Model;
public interface ITrekService
{
    Task<List<ImageList>> GetTrekImages(string _foldername);
}