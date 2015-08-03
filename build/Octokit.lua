project "Octokit"
  --SetupManagedProject()
  language "C#"
  kind "SharedLib"

  defines
  {
    "SIMPLE_JSON_OBJARRAYINTERNAL",
    "SIMPLE_JSON_INTERNAL",
    "SIMPLE_JSON_READONLY_COLLECTIONS",
    "NET_45"
  }

  files
  {
    depsdir .. "/octokit.net/Octokit/**.cs",
    depsdir .. "/octokit.net/SolutionInfo.cs" 
  }

  links
  {
    "Microsoft.CSharp",
    "System",
    "System.Core",
    "System.Net",
    "System.Net.Http",
    "System.Runtime.Serialization"
  }