using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GraphQLService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GraphQLService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<User>> GetUsers()
    {
        var query = new
        {
            query = "{ users { id email role { name } } }"
        };

        var fullResponse = await PostGraphQL(query);
        var data = fullResponse?["data"];
        return data?["users"]?.ToObject<List<User>>() ?? new();
    }

    public async Task<List<Role>> GetRoles()
    {
        var query = new
        {
            query = "{ roles { id name } }"
        };

        var fullResponse = await PostGraphQL(query);
        var data = fullResponse?["data"];
        return data?["roles"]?.ToObject<List<Role>>() ?? new();
    }

    public async Task<List<SecurityEvent>> GetSecurityEvents()
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

        var fullResponse = await PostGraphQL(query);

        if (fullResponse == null)
        {
            Console.WriteLine("[GraphQLService] No response.");
            return new();
        }

        if (fullResponse["errors"] is JArray errors)
        {
            Console.WriteLine("[GraphQLService] GraphQL Errors:");
            foreach (var error in errors)
            {
                Console.WriteLine(error["message"]);
            }
            return new(); // Return empty to avoid crash
        }

        var data = fullResponse["data"] as JObject;

        if (data == null)
        {
            Console.WriteLine("[GraphQLService] Data section missing.");
            return new();
        }

        return data["securityEvents"]?.ToObject<List<SecurityEvent>>() ?? new();
    }

    public async Task<bool> AssignUserRole(Guid userId, Guid roleId)
    {
        var authorId = userId; // Simulate the author as the same user for now
        var mutation = new
        {
            query = @"
                mutation($userId: UUID!, $roleId: UUID!, $authorId: UUID!) {
                    assignRole(userId: $userId, roleId: $roleId, authorId: $authorId)
                }",
            variables = new { userId, roleId, authorId }
        };

        var fullResponse = await PostGraphQL(mutation);
        var data = fullResponse?["data"];
        return data?["assignRole"]?.ToObject<bool>() ?? false;
    }

    public async Task<JObject> PostGraphQL(object payload)
    {
        var http = _httpClientFactory.CreateClient("Api");

        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "/graphql")
        {
            Content = content
        };

        // Forward cookies
        var cookie = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
        if (!string.IsNullOrEmpty(cookie))
        {
            request.Headers.Add("Cookie", cookie);
        }

        var response = await http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"[GraphQLService] Status Code: {response.StatusCode}");
        Console.WriteLine($"[GraphQLService] Response Body: {body}");

        if (!response.IsSuccessStatusCode || string.IsNullOrEmpty(body))
        {
            Console.WriteLine("[GraphQLService] Request failed. Not parsing as JSON.");
            return null;
        }

        return JObject.Parse(body); // ✅ return full response
    }
}
