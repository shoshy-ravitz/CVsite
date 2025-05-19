using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Octokit;

namespace CVSite
{
    public class GitHubService
    {
        private readonly GitHubClient _client;
        private readonly string _username;
        private readonly IMemoryCache _cache;

        public GitHubService(IOptions<GitHubOptions> options, IMemoryCache cache)
        {
            var settings = options.Value;
            _username = settings.Username;
            _client = new GitHubClient(new ProductHeaderValue("MyApp"))
            {
                Credentials = new Credentials(settings.Token)
            };
            _cache = cache;
        }

        public async Task<List<RepositoryInfo>> GetPortfolio()
        {
            const string cacheKey = "GitHubPortfolio";


            if (_cache.TryGetValue(cacheKey, out List<RepositoryInfo> cachedPortfolio))
            {
                var repositories = await _client.Repository.GetAllForUser(_username);
                var lastUpdate = repositories.Max(repo => repo.UpdatedAt);

                if (cachedPortfolio != null && cachedPortfolio.Any())
                {
                    var cachedLastUpdate = cachedPortfolio.Max(info => info.LastCommit);
                    if (cachedLastUpdate >= lastUpdate)
                    {
                        return cachedPortfolio;
                    }
                }
            }

            var repositoriesList = await _client.Repository.GetAllForUser(_username);
            var repositoryInfos = new List<RepositoryInfo>();

            foreach (var repo in repositoriesList)
            {
                var commits = await _client.Repository.Commit.GetAll(repo.Id);
                var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Id);
                var languages = await _client.Repository.GetAllLanguages(repo.Id);

                List<string> lang = new List<string>();
                foreach (var l in languages)
                {
                    lang.Add(l.Name);
                    Console.WriteLine(l.Name);
                }

                repositoryInfos.Add(new RepositoryInfo
                {
                    Name = repo.Name,
                    Languages = lang,
                    LastCommit = commits.FirstOrDefault()?.Commit.Committer.Date,
                    Stars = repo.StargazersCount,
                    PullRequestCount = pullRequests.Count,
                    Url = repo.HtmlUrl
                });
            }


            _cache.Set(cacheKey, repositoryInfos, TimeSpan.FromMinutes(5));

            return repositoryInfos;
        }

        public async Task<List<Repository>> SearchRepositories(string repoName = null, string language = null, string username = null)
        {
            var request = new SearchRepositoriesRequest(repoName)
            {
                Language = Language.CSharp,
                //User = username
            };

            var searchResult = await _client.Search.SearchRepo(request);
            return searchResult.Items.ToList();
        }
    }

    public class RepositoryInfo
    {
        public string Name { get; set; }
        public List<string> Languages { get; set; }
        public DateTimeOffset? LastCommit { get; set; }
        public int Stars { get; set; }
        public int PullRequestCount { get; set; }
        public string Url { get; set; }
    }
}
