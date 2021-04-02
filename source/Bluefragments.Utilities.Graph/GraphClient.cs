using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Bluefragments.Utilities.Http;
using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bluefragments.Utilities.Graph
{
    public class GraphClient
    {
        private string token;
        private string graphEndpoint = "https://graph.microsoft.com/beta";

        public GraphClient(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            this.token = token;
            Client = GetAuthenticatedClient(token);
        }

        public GraphClient(string clientId, string clientSecret, string tenant)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            if (string.IsNullOrWhiteSpace(tenant))
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            Client = GetAuthenticatedClientAsync(clientId, clientSecret, tenant).Result;
        }

        public GraphServiceClient Client { get; private set; }

        public async Task SendMailAsync(string toAddress, string subject, string content)
        {
            if (string.IsNullOrWhiteSpace(toAddress))
            {
                throw new ArgumentNullException(nameof(toAddress));
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            var recipients = new List<Recipient>
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = toAddress,
                    },
                },
            };

            var email = new Message
            {
                Body = new ItemBody
                {
                    Content = content,
                    ContentType = BodyType.Text,
                },
                Subject = subject,
                ToRecipients = recipients,
            };

            await Client.Me.SendMail(email, true).Request().PostAsync();
        }

        public async Task<Message[]> GetRecentMailAsync()
        {
            var messages = await Client.Me.MailFolders.Inbox.Messages.Request().GetAsync();
            return messages.Take(5).ToArray();
        }

        public async Task<User> GetMeAsync()
        {
            var me = await Client.Me.Request().GetAsync();
            return me;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            try
            {
                return await Client.Users.Request().GetAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<Organization> GetOrganizationAsync()
        {
            var organization = await Client.Organization.Request().GetAsync();
            return organization?.FirstOrDefault();
        }

        public async Task<Microsoft.Graph.Group> GetIsMemberOfGroupAsync(string groupId)
        {
            var groups = await GetMyMemberOfGroupsAsync();
            return groups?.SingleOrDefault(x => x.Id == groupId);
        }

        public async Task<User> GetManagerAsync()
        {
            var manager = await Client.Me.Manager.Request().GetAsync() as User;
            return manager;
        }

        public async Task<GraphPhotoResponse> GetPhotoAsync()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            using (var response = await client.GetAsync("https://graph.microsoft.com/v1.0/me/photo/$value"))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Graph returned an invalid success code: {response.StatusCode}");
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);

                var photoResponse = new GraphPhotoResponse
                {
                    Bytes = bytes,
                    ContentType = response.Content.Headers.ContentType?.ToString(),
                };

                if (photoResponse != null)
                {
                    photoResponse.Base64String = $"data:{photoResponse.ContentType};base64," +
                                                 Convert.ToBase64String(photoResponse.Bytes);
                }

                return photoResponse;
            }
        }

        public async Task<string> GetEmailAddressAsync()
        {
            string endpoint = "https://graph.microsoft.com/v1.0/me";
            string queryParameter = "?$select=mail,userPrincipalName";
            string email = null;

            using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint + queryParameter))
            {
                 request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                 request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                 using (var response = await HttpClientHelper.HttpClient.SendAsync(request))
                 {
                     if (response.IsSuccessStatusCode)
                     {
                         var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                         email = json.GetValue("userPrincipalName").ToString();
                     }

                     return email;
                 }
             }
        }

        public async Task<bool> EventDeleteAsync(string userId, string eventId, string comment)
        {
            var response = new EventResponse
            {
                Comment = comment,
                Response = true,
            };

            var http = GetAuthenticatedHttpClient(token);
            var result = await http.DeleteAsync($"{graphEndpoint}/users/{userId}/events/{eventId}");

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> EventDeclineAsync(string userId, string eventId, string comment)
        {
            var response = new EventResponse
            {
                Comment = comment,
                Response = true,
            };

            var http = GetAuthenticatedHttpClient(token);
            var content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json");
            var result = await http.PostAsync($"{graphEndpoint}/users/{userId}/events/{eventId}/decline", content);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> EventAcceptAsync(string userId, string eventId, string comment)
        {
            var response = new EventResponse
            {
                Comment = comment,
                Response = true,
            };

            var http = GetAuthenticatedHttpClient(token);
            var content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json");
            var result = await http.PostAsync($"{graphEndpoint}/users/{userId}/events/{eventId}/accept", content);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> EventTentiveAsync(string userId, string eventId, string comment)
        {
            var response = new EventResponse
            {
                Comment = comment,
                Response = true,
            };

            var http = GetAuthenticatedHttpClient(token);
            var content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json");
            var result = await http.PostAsync($"{graphEndpoint}/users/{userId}/events/{eventId}/tentativelyAccept", content);

            return result.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Event>> GetEvents(string userId)
        {
            var http = GetAuthenticatedHttpClient(token);
            var result = await http.GetAsync($"{graphEndpoint}/users/{userId}/events");

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Event>>(content);
            }

            return null;
        }

        /// <summary>
        /// This snippet requires an admin work account.
        /// </summary>
        /// <returns>A task that returns a list of Groups the current user is a member of. </returns>
        public async Task<List<Group>> GetMyMemberOfGroupsAsync()
        {
            List<Group> groups = new List<Group>();

            // Get groups the current user is a direct member of.
            IUserMemberOfCollectionWithReferencesPage memberOfGroups = await Client.Me.MemberOf.Request().GetAsync();

            if (memberOfGroups?.Count > 0)
            {
                foreach (var directoryObject in memberOfGroups)
                {
                    // We only want groups, so ignore DirectoryRole objects.
                    if (directoryObject is Group)
                    {
                        Group group = directoryObject as Group;
                        groups.Add(directoryObject as Group);
                    }
                }
            }

            return groups;
        }

        private static HttpClient GetAuthenticatedHttpClient(string token)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
            client.DefaultRequestHeaders.Add("Prefer", "outlook.timezone=\"" + TimeZoneInfo.Local.Id + "\"");

            return client;
        }

        /// <summary>
        /// This client requires that admin consent have been provided to the client.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="tenant"></param>
        /// <returns>A task that returns a GraphServiceClient with the provided clientId. </returns>
        private async Task<GraphServiceClient> GetAuthenticatedClientAsync(string clientId, string clientSecret, string tenant)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
            });

            var tokenEndpoint = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token";
            var tokenResponse = await HttpClientHelper.HttpClient.PostAsync(tokenEndpoint, content);
            var json = await tokenResponse.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json);
            string token = data?.access_token;

            this.token = token;

            return GetAuthenticatedClient(token);
        }

        /// <summary>
        /// This client requires a valid access token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>A GraphServiceClient using the provided token. </returns>
        private GraphServiceClient GetAuthenticatedClient(string token)
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    requestMessage =>
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                        requestMessage.Headers.Add("Prefer", "outlook.timezone=\"" + TimeZoneInfo.Local.Id + "\"");
                        return Task.CompletedTask;
                    }));

            return graphClient;
        }
    }
}
