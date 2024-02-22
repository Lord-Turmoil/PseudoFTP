namespace PseudoFTP.Helper;

public static class CredentialHelper
{
    public static string GetUsername(string credential)
    {
        return credential.Split(":")[0];
    }

    public static string GetPassword(string credential)
    {
        return credential.Split(":")[1];
    }

    public static string GetCredential(string username, string password)
    {
        return $"{username}:{password}";
    }
}