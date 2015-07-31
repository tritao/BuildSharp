  project "BuildSharp"

    SetupManagedProject()
    kind "SharedLib"

    files
    {
      "**.cs",
      path.join(depsdir, "fastJSON/*.cs")
    }
  
    links
    {
      "System",
      "System.Core",
      "System.Data",
      "System.Xml",
      "Octokit",
      "Eluant",
    }