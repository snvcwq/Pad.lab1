
namespace Client;

public static class RegistrationHandler
{
    public static void AddClientIdentifier()
    {
        do
        {
            Console.WriteLine("Insert a valid Identifier");
            ClientData.Identifier = Console.ReadLine()!;
        } while (string.IsNullOrEmpty(ClientData.Identifier));
    }
}