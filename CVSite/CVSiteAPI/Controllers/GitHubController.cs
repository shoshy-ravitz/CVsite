using CVSite;
using Microsoft.AspNetCore.Mvc;
using Octokit;


namespace CVSite.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GitHubController : ControllerBase
    {
        private readonly GitHubService _gitHubService;

        public GitHubController(GitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        // GET: /github/portfolio
        [HttpGet("portfolio")]
        public async Task<ActionResult<List<RepositoryInfo>>> GetPortfolio()
        {
            var portfolio = await _gitHubService.GetPortfolio();
            return Ok(portfolio);
        }

        // GET: /github/search
        [HttpGet("search")]
        public async Task<ActionResult<List<Repository>>> SearchRepositories(string repoName = null, string language = null, string username = null)
        {
            var repositories = await _gitHubService.SearchRepositories(repoName, language, username);
            return Ok(repositories);
        }
    }
}
