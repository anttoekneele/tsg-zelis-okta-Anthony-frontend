using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class GraphQLService
{
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("http://localhost:5216/graphql")
    };

    public static async Task<List<User>> GetUsers()
    {
        var query = new
        {
            query = "{ users { id email role { name } } }"
        };

        var response = await PostGraphQL(query);
        return response?["users"]?.ToObject<List<User>>() ?? new();
    }

    public static async Task<List<Role>> GetRoles()
    {
        var query = new
        {
            query = "{ roles { id name } }"
        };

        var response = await PostGraphQL(query);
        return response?["roles"]?.ToObject<List<Role>>() ?? new();
    }

    public static async Task<List<SecurityEvent>> GetSecurityEvents()
    {
        var query = new
        {
            query = @"
            {
                securityEvents {
                    eventType
                    occurredUtc
                    details
                    authorUser { email }
                    affectedUser { email }
                }
            }"
        };

        var response = await PostGraphQL(query);
        return response?["securityEvents"]?.ToObject<List<SecurityEvent>>() ?? new();
    }

    public static async Task AssignUserRole(Guid userId, Guid roleId)
    {
        var mutation = new
        {
            query = @"mutation($userId: ID!, $roleId: ID!) {
                assignUserRole(userId: $userId, roleId: $roleId)
            }",
            variables = new { userId, roleId }
        };

        await PostGraphQL(mutation);
    }

    private static async Task<JObject> PostGraphQL(object payload)
    {
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var result = await _http.PostAsync("", content);
        var body = await result.Content.ReadAsStringAsync();

        var parsed = JObject.Parse(body);
        return (JObject)parsed["data"];
    }
}
