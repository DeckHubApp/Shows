namespace DeckHub.Shows.Services
{
    public interface IIdentityPaths
    {
        string Login { get; }
        string Logout { get; }
        string Manage { get; }
    }
}