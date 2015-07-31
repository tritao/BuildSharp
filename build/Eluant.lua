project "Eluant"

  SetupManagedProject()
  kind "SharedLib"

  location (depsdir .. "/Eluant")

  files
  {
    depsdir .. "/Eluant/Eluant/**.cs",
    depsdir .. "/Eluant/Eluant/*.lua",    
  }

  configuration '**/**.lua'
    buildaction "Embed"

  links
  {
    "System",
    "System.Core",
  }  