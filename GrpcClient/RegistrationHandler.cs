using Common;

namespace gRPCClient;

public static class RegistrationHandler
{
    public static void AddClientIdentifier()
    {
        do
        {
            Console.WriteLine("Insert a valid Identifier".AddInsertPrefix());
            ClientData.Identifier = Console.ReadLine()!;
        } while (string.IsNullOrEmpty(ClientData.Identifier));
    }

    public static ClientRegistration AddClientIdentifier(this ClientRegistration registration)
    {
        registration.ClientId = ClientData.Identifier;
        return registration;
    }
}