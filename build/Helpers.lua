-- This module checks for the all the project dependencies.

action = _ACTION or ""

depsdir = path.getabsolute("../deps");
srcdir = path.getabsolute("../src");
incdir = path.getabsolute("../include");
bindir = path.getabsolute("../bin");
examplesdir = path.getabsolute("../examples");
testsdir = path.getabsolute("../tests");

builddir = path.getabsolute("./" .. action);
libdir = path.join(builddir, "lib", "%{cfg.buildcfg}_%{cfg.platform}");
gendir = path.join(builddir, "gen");

function SetupNativeProject()
  location (path.join(builddir, "projects"))

  local c = configuration "Debug"
    defines { "DEBUG" }
    
  configuration "Release"
    defines { "NDEBUG" }
    optimize "On"
    
  -- OS-specific options
  
  configuration "Windows"
    defines { "WIN32", "_WINDOWS" }
  
  configuration(c)
end

function SetupManagedProject()
  language "C#"
  location (path.join(builddir, "projects"))

  local c = configuration "vs*"
    location "."

  configuration(c)
end

function IncludeDir(dir)
  local deps = os.matchdirs(dir .. "/*")
  
  for i,dep in ipairs(deps) do
    local fp = path.join(dep, "premake4.lua")
    fp = path.join(os.getcwd(), fp)
    
    if os.isfile(fp) then
      print(string.format(" including %s", dep))
      include(dep)
    end
  end
end