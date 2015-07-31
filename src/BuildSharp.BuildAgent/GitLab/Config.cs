namespace BuildSharp.GitLab
{
    public static class Config
    {
        /// <summary>
        /// URL to the GitLab CI coordinator
        /// </summary>
        public static string GitLabCIEndpoint;

        /// <summary>
        /// URL to the GitLab instance
        /// </summary>
        public static string GitLabEndpoint;

        /// <summary>
        /// Private token to access the GitLab instance
        /// </summary>
        public static string GitLabPrivateToken;

        /// <summary>
        /// GitLab CI runner register token
        /// </summary>
        public static string RegisterToken;

        /// <summary>
        /// GitLab CI runner auth token
        /// </summary>
        public static string AuthToken;

        /// <summary>
        /// Load the configuration
        /// </summary>
        public static bool LoadConfig(string file)
        {
            return false;
        }

        /// <summary>
        /// Load the configuration
        /// </summary>
        public static bool SaveConfig(string file)
        {
            return false;
        }
    }
}
