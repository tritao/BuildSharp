  project "BuildSharp.BuildServer"

    SetupManagedProject()
    kind "ConsoleApp"

    files
    {
      "**.cs",
      path.join(depsdir, "sqlite-net/src/SQLite.cs")
    }
  
    links
    {
      "BuildSharp",
      "System",
      "System.Net.Http",
    }