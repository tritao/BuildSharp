using System;
using System.Diagnostics;
using System.IO;
using BuildSharp.Builds;
using BuildSharp.VersionControl;

namespace BuildSharp.Steps
{
    /// <summary>
    /// Build step used for Git-related tasks.
    /// </summary>
    public class GitBuildStep : BuildStep
    {
        public GitBuildStep(BuildExecutor executor, Build build)
            : base(executor, build)
        {
        }

        public override void Run()
        {
            //CheckProjectGitChanges(repo);
        }

        private bool IsValidRepo(string repo)
        {
            if (!Directory.Exists(repo))
                return false;

            //try
            //{
            //    if (!LibGit2Sharp.Repository.IsValid(repo))
            //        return false;
            //}
            //catch (RepositoryNotFoundException ex)
            //{
            //    return false;
            //}

            return true;
        }

        private void CheckProjectGitChanges()
        {
            Branch branch;
            Build.Commit.Branch.TryGetTarget(out branch);

            Repository repo;
            branch.Repository.TryGetTarget(out repo);

            var repoPath = Path.Combine(Options.OutputDir, Build.Project.Name);

            if (!IsValidRepo(repoPath))
                CloneRepo(repo, repoPath);

            //repo.Git = new LibGit2Sharp.Repository(repoPath);

            //Log.Message("Checking for changes...");
            //FetchRepo(localRepo);
        }

        //void FetchRepo(LibGit2Sharp.IRepository localRepo)
        //{
        //    var options = new FetchOptions
        //    {
        //        OnProgress = output =>
        //        {
        //            Log.Message("Fetch progress: {0}", output);
        //            return true;
        //        },
        //        OnTransferProgress = progress =>
        //        {
        //            Log.Message("Fetch transfer progress ({0}/{1})",
        //                progress.ReceivedObjects, progress.TotalObjects);
        //            return true;
        //        },
        //        OnUpdateTips = (name, id, newId) =>
        //        {
        //            Log.Message("Fetch updated '{0}' from {1} to {2}", name,
        //                id, newId);
        //            return true;
        //        }
        //    };

        //    localRepo.Fetch("origin", options);
        //}

        void CloneRepo(Repository repo, string repoPath)
        {
            try
            {
                if (Directory.Exists(repoPath))
                    Directory.Delete(repoPath, true);
            }
            catch (Exception ex)
            {
                Log.Error("Could not delete repository: {0}", ex.ToString());
                throw;
            }

            Log.Message("Cloning the repository for project '{0}...", Build.Project.Name);

            using (var process = new Process {
                StartInfo =
                {
                    WorkingDirectory = Directory.GetParent(repoPath).FullName,
                    FileName = @"git.exe",
                    Arguments = string.Format("clone {0} {1}", repo.URL, repoPath),
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }})
            {
                process.Start();
                process.WaitForExit();
                process.Close();
            }
        }

        static string CloneLibGit2(Repository repo, string repoPath)
        {
            //var cloneOptions = new CloneOptions()
            //{
            //    IsBare = false,
            //    Checkout = true,
            //    OnCheckoutProgress = (path, completedSteps, totalSteps) =>
            //    {
            //        //Log.Message("checkout progress ({0}/{1})", completedSteps,
            //        //    totalSteps);
            //    },
            //    OnTransferProgress = progress =>
            //    {
            //        //Log.Message("transfer progress ({0}/{1})", progress.ReceivedObjects,
            //        //    progress.TotalObjects);
            //        return true;
            //    },
            //};

            //var clonedRepo = LibGit2Sharp.Repository.Clone(repo.URL, repoPath, cloneOptions);
            //return clonedRepo;

            return string.Empty;
        }
    }
}
