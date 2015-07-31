-- This is the starting point of the build scripts for the project.
-- It defines the common build settings that all the projects share
-- and calls the build scripts of all the sub-projects.

dofile "Helpers.lua"

solution "BuildSharp"

  configurations { "Debug", "Release" }
  platforms { "x32", "x64" }
  flags { "Unicode", "Symbols" }
  
  location (builddir)
  objdir (path.join(builddir, "obj"))

  targetdir (libdir)
  libdirs { libdir }
  debugdir (bindir)

  startproject "BuildSharp"
  
  configuration "vs2012"
    framework "4.5"

  configuration {}

  include("../src/BuildSharp")
  include("../src/BuildSharp.BuildAgent")
  include("../src/BuildSharp.BuildServer")

  group "Dependencies"

    dofile "Lua.lua"
    dofile "Eluant.lua"
    --dofile "NLua.lua"  
    --dofile "LibGit2.lua"
    dofile "Octokit.lua"
