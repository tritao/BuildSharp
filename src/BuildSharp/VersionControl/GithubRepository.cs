namespace BuildSharp.VersionControl
{
    /// <summary>
    /// Represents a GitHub repository.
    /// </summary>
    public class GithubRepository : Repository
    {
        public GithubRepository(Project project) : base(project)
        {
            
        }

        /// <summary>
        /// Owner of the repository.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Name of the repository.
        /// </summary>
        public string Name { get; set; }
    }
}
