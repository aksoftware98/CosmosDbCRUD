using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Dynamic;
using System.Threading.Tasks;

namespace AppStoreComsosDbDemo
{
    class Program
    {

        private static string _connectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";


        private static CosmosClient _client = new CosmosClient(_connectionString);
        private static Database _database = null;
        private static Microsoft.Azure.Cosmos.Container _container = null; 

        static async Task Main(string[] args)
        {
            _database = _client.GetDatabase("AppStore");
            _container = _database.GetContainer("Apps");

            //Console.WriteLine("Adding Item....");
            //Console.WriteLine("Updating Item....");
            //Console.WriteLine("Deleting Item....");        
            Console.WriteLine("Getting Items....");

            await GetAppsAsync(); 

            //await CreateAppAsync();
            //await UpdateAppAsync(); 
            //await DeleteAppAsync(); 

            Console.ReadKey(); 
        }

        #region Creating Docuemnt 
        static async Task CreateAppAsync()
        {
            for (int i = 1; i < 21; i++)
            {
                var app = new StoreApp
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"App {i}",
                    Description = "Description of App {i}",
                    Icon = $"app{i}.png",
                    CoverPhoto = $"cover{i}.jpg",
                    Category = "Business",
                    Developer = new Developer
                    {
                        Name = $"Developer {i}",
                        Icon = $"developer{i}.png",
                        Url = "some url"
                    },
                    Version = $"{i}.0.1",
                    PackageUrl = "package.exe",
                };
                
                Console.Write($"{i} ");
                var result = await _container.CreateItemAsync(app);
                Console.WriteLine(result.StatusCode);
                
            }
            
        }
        #endregion

        #region Update Operation 
        static async Task UpdateAppAsync()
        {
            var app = new
            {
                id = "87cca0bb-76cb-49ba-ab49-e1f393d9fbaa",
                name = "App 2-u",
                description = "Description of App 2",
                icon = "app2.png",
                coverPhoto = "cover2.jpg",
                category = "Business",
                developer = new
                {
                    name = "Developer 2",
                    icon = "developer2.png",
                    url = "some url"
                },
                version = "2.0.6",
                packageUrl = "package.exe",
                reviews = new[]
                {
                    new { name = "Some user", title = "That's a nice app", rate = 4 }
                }
            };

            var result = await _container.ReplaceItemAsync(app, app.id);
            Console.WriteLine(result.StatusCode); 
        }
        #endregion

        #region Delete Operation 
        async static Task DeleteAppAsync()
        {
            var result = await _container.DeleteItemAsync<object>("87cca0bb-76cb-49ba-ab49-e1f393d9fbaa", new PartitionKey("Business"));
            Console.WriteLine(result.StatusCode); 
        }
        #endregion

        #region Query 
        static async Task GetAppsAsync()
        {

            string query = "SELECT * FROM c";

            var appsIterator = _container.GetItemQueryIterator<StoreApp>(query);

            var apps = await appsIterator.ReadNextAsync();

            foreach (var item in apps)
            {
                Console.WriteLine($"{item.Name} - {item.Version} - {item.Developer.Name} - {item.Category}"); 
            }

        }
        #endregion

    }

    public class StoreApp
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("coverPhoto")]
        public string CoverPhoto { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }


        [JsonProperty("packageUrl")]
        public string PackageUrl { get; set; }

        [JsonProperty("developer")]
        public Developer Developer { get; set; }

        [JsonProperty("reviews")]
        public Review[] Reviews { get; set; }
    }

    public class Developer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public class Review
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("rate")]
        public int Rate { get; set; }
    }

}
