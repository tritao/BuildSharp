project "NLua"

  SetupManagedProject()
  kind "SharedLib"

  location (depsdir .. "/NLua/Core/NLua")

  files
  {
    depsdir .. "/NLua/Core/NLua/**.cs",
  }

  links
  {
    "System",
    "System.Core",
    "System.Data",
    "System.Xml",
    "KeraLua"
  }

project "KeraLua"

  SetupManagedProject()
  kind "SharedLib"

  location (depsdir .. "/KeraLua/src/KeraLua")

  files
  {
    depsdir .. "/KeraLua/src/KeraLua/**.cs",
  }

  links
  {
    "System",
    "System.Core",
  }