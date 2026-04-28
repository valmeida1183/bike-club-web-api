namespace BikeClub.SharedKernel.Services
{
    public interface ICryptographerService
    {
        string Hash(string value);
        bool CompareHash(string value, string hash);
    }
}
