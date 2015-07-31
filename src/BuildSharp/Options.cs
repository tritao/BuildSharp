namespace BuildSharp
{
    public class Options
    {
        public Options()
        {
            ServerAddress = "localhost";
            ServerPort = 8081;
            Database = "builds.sqlite3";
        }

        // General options
        public bool Verbose = true;
        public string OutputDir;

        // Agent options
        public string ServerAddress;
        public int ServerPort;

        // GitHub credentials
        public string Username;
        public string Token;

        public string Database;
    }
}
