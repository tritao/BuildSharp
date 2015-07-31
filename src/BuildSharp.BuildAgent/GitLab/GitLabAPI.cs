using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using BuildSharp.Agent.GitLab;
using BuildSharp.Builds;

namespace BuildSharp.GitLab
{
    public static class GitLabAPI
    {
        /// <summary>
        /// GitLab CI API URL
        /// </summary>
        private static string APIEndpoint
        {
            get { return Config.GitLabCIEndpoint + "/api/v1"; }
        }

        public struct RunnerInfo
        {
            public int id;
            public string token;
        }

        /// <summary>
        /// Register the runner with the coordinator
        /// </summary>
        /// <param name="publicKey">SSH Public Key</param>
        /// <param name="token">Token</param>
        /// <returns>Token</returns>
        public static List<RunnerInfo> GetRunners()
        {
            var query = "private_token=" + Uri.EscapeDataString(Config.GitLabPrivateToken)
                + "&url=" + Uri.EscapeDataString(Config.GitLabEndpoint);

            HttpStatusCode status;
            var response = WebAPI.GET(APIEndpoint + "/runners" + query,
                string.Empty, out status);
            if (response == null)
                return null;

            try
            {
                return fastJSON.JSON.ToObject<List<RunnerInfo>>(response);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Register the runner with the coordinator
        /// </summary>
        /// <param name="publicKey">SSH Public Key</param>
        /// <param name="token">Token</param>
        /// <returns>Token</returns>
        public static string RegisterRunner(string publicKey, string token)
        {
            var postBody = "token=" + Uri.EscapeDataString(token) + "&public_key="
                + Uri.EscapeDataString(publicKey);

            HttpStatusCode status;
            var response = WebAPI.POST(APIEndpoint + "/runners/register", postBody,
                out status);
            if (response == null)
                return null;

            try
            {
                return fastJSON.JSON.ToObject<RunnerInfo>(response).token;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public struct BuildInfo
        {
            public int id;
            public int project_id;
            public string project_name;
            public string[] commands;
            public string repo_url;
            public string sha;
            public string before_sha;
            public string ref_name;
            public int timeout;
            public bool allow_git_fetch;
        }

        /// <summary>
        /// Get a new build
        /// </summary>
        /// <returns>BuildInfo object or null on error/no build</returns>
        public static BuildInfo? GetBuild()
        {
            Console.WriteLine("* Checking for builds...");

            var postBody = "token=" + Uri.EscapeDataString(Config.AuthToken);

            HttpStatusCode status;
            var response = WebAPI.POST(APIEndpoint + "/builds/register", postBody,
                out status);
            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("* Nothing");
                return null;
            }

            try
            {
                var obj = fastJSON.JSON.ToObject<BuildInfo>(response);
                var info = obj;
                //info.id = obj.Get<int>("id");
                //info.project_id = obj.Get<int>("project_id");
                //info.project_name = obj.Get("project_name");
                //info.commands = System.Text.RegularExpressions.Regex.Replace(obj.Get<string>("commands"), "(\r|\n)+", "\n").Split('\n');
                //info.repo_url = obj.Get("repo_url");
                //info.sha = obj.Get("sha");
                //info.before_sha = obj.Get("before_sha");
                //info.ref_name = obj.Get("ref");
                //info.timeout = int.Parse(obj.Get("timeout") ?? "1800");
                //info.allow_git_fetch = obj.Get<bool>("allow_git_fetch");
                return info;
            }
            catch (Exception)
            {
                Console.WriteLine("* Failed");
            }
            return null;
        }

        /// <summary>
        /// PUSH the Build to the Gitlab CI Coordinator
        /// </summary>
        /// <param name="id">Build ID</param>
        /// <param name="state">State</param>
        /// <param name="sTrace">Command output</param>
        /// <returns></returns>
        public static bool PushBuild(int id, BuildState state, string sTrace)
        {
            Console.WriteLine("[" + DateTime.Now + "] Submitting build " + id
                + " to coordinator ...");
            var body = new StringBuilder();

            body.Append("token=").Append(Uri.EscapeDataString(Config.AuthToken));

            body.Append("&state=");

            switch (state)
            {
            case BuildState.Queued:
                body.Append("waiting");
                break;
            case BuildState.Running:
                body.Append("running");
                break;
            case BuildState.Failed:
                body.Append("failed");
                break;
            case BuildState.Success:
                body.Append("success");
                break;
            default:
                throw new ArgumentOutOfRangeException("state");
            }

            body.Append("&trace=");
            foreach (string t in sTrace.Split('\n'))
                body.Append(Uri.EscapeDataString(t)).Append("\n");

            HttpStatusCode status;
            var response = WebAPI.PUT(APIEndpoint + "/builds/" + id + ".json",
                body.ToString(), out status);

            return response != null;
        }
    }
}
