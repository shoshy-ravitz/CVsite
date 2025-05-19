using CVSite;
using Microsoft.AspNetCore.Mvc;
using Octokit;

//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace CVSiteAPI.Controllers
//{

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
