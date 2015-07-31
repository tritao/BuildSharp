using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Octokit.Internal;

namespace BuildSharp.VersionControl
{
    /// <summary>
    /// Polls Github for changes to the repository.
    /// </summary>
    public sealed class GithubPoller
    {
        public Action<GithubRepository, List<Commit>> RepositoryChanged;

        private readonly GitHubClient github;
        private readonly IEnumerable<Project> projects;

        public GithubPoller(IEnumerable<Project> projects, string user,
            string password)
        {
            var creds = new Credentials(user, password);
            var credsStore = new InMemoryCredentialStore(creds);
            this.github = new GitHubClient(new ProductHeaderValue("BuildSharp"),
                credsStore);
            this.projects = projects;
        }

        private Commit GetCommitFromGithubCommit(GitReference commit)
        {
            return new Commit
            {
                //Author = commit.Author.Name,
                //Message = commit.Message,
                Revision = commit.Sha
            };
        }

        public void Poll()
        {
            foreach (var project in projects)
            {
                var githubRepos = project.Repositories.OfType<GithubRepository>();
                foreach (var repository in githubRepos)
                {
                    Log.Debug("Polling {0} repository for changes...",
                        repository.Name);

                    var repo = repository;
                    Task.Run(async () => { await UpdateRepository(repo); });
                }
            }
        }

        public async Task UpdateRepository(GithubRepository repo)
        {
            if (repo.Branches.Count == 0)
                await UpdateRepositoryBranches(repo);

            foreach (var branch in repo.Branches)
            {
                var info = await github.Repository.GetBranch(repo.Owner,
                    repo.Name, branch.Name);

                if (branch.GetCommitByReference(info.Commit.Sha) != null)
                    continue;

                var commit = GetCommitFromGithubCommit(info.Commit);
                commit.Branch = new WeakReference<Branch>(branch);

                branch.AddCommit(commit);

                if (RepositoryChanged != null)
                    RepositoryChanged(repo, new List<Commit> {commit});
            }
        }

        public async Task UpdateRepositoryBranches(GithubRepository repo)
        {
            Log.Debug("Getting {0} repository branches...", repo.Name);
            var branches = await github.Repository.GetAllBranches(repo.Owner,
                repo.Name);

            foreach (var branch in branches)
            {
                var repoBranch = new Branch
                {
                    Name = branch.Name,
                    Repository = new WeakReference<Repository>(repo)
                };

                repo.Branches.Add(repoBranch);
            }
        }
    }
}
