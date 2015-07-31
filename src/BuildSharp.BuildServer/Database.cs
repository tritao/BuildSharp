using System;
using System.Collections.Generic;
using BuildSharp.Builds;
using SQLite;

namespace BuildSharp.BuildServer.Database
{
    public class Build
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, MaxLength(40)]
        public string Revision { get; set; }

        public int ProjectId { get; set; }

        public BuildState State { get; set; }
    }

    public class Project
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// Database that persists builds data.
    /// </summary>
    public class Database : IDisposable
    {
        public SQLiteConnection Sqlite;

        public Database(string databasePath)
        {
            Sqlite = new SQLiteConnection(databasePath);
            CreateSchema();
        }

        public void Dispose()
        {
            Sqlite.Dispose();
        }

        public void CreateSchema()
        {
            Sqlite.CreateTable<Build>();
            Sqlite.CreateTable<Project>();
        }

        public void AddBuild(Builds.Build build)
        {
            var obj = new Build
            {
                Revision = build.Commit.Revision,
                State = build.State
            };

            Sqlite.Insert(obj);

            build.Id = obj.Id;
        }

        public void AddProject(VersionControl.Project project)
        {
            var obj = new Project
            {
                Name = project.Name
            };

            Sqlite.Insert(obj);

            project.Id = obj.Id;
        }

        public void LoadProjects(List<VersionControl.Project> projects)
        {
            foreach (var obj in Sqlite.Table<Project>())
            {
            }
        }

        public void LoadBuilds()
        {
            foreach (var obj in Sqlite.Table<Build>())
            {
            }
        }
    }
}
