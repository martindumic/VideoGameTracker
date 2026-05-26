using System.Net.Http.Headers;

namespace VideoGameTracker.Tests;

public static class TestHttpClientExtensions
{
    public static void SetTestUser(this HttpClient client, string userId, string userName, string email, params string[] roles)
    {
        client.ClearTestUser();

        client.DefaultRequestHeaders.Add(TestAuthConstants.UserIdHeader, userId);
        client.DefaultRequestHeaders.Add(TestAuthConstants.UserNameHeader, userName);
        client.DefaultRequestHeaders.Add(TestAuthConstants.EmailHeader, email);

        if (roles.Length > 0)
        {
            client.DefaultRequestHeaders.Add(TestAuthConstants.RolesHeader, string.Join(",", roles));
        }
    }

    public static void ClearTestUser(this HttpClient client)
    {
        client.DefaultRequestHeaders.Remove(TestAuthConstants.UserIdHeader);
        client.DefaultRequestHeaders.Remove(TestAuthConstants.UserNameHeader);
        client.DefaultRequestHeaders.Remove(TestAuthConstants.EmailHeader);
        client.DefaultRequestHeaders.Remove(TestAuthConstants.RolesHeader);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
