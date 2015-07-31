  project "BuildSharp.BuildAgent"

    SetupManagedProject()
    kind "ConsoleApp"

    files   { "**.cs" }
  
    links
    {
      "BuildSharp",
      "System",
      "System.Net"
    }