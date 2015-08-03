project "libgit2"

  kind "SharedLib"
  language "C++"
  
  targetname "git2-06d772d"
  
  files
  {
    depsdir .. "/libgit2sharp/libgit2/src/*.c",
    depsdir .. "/libgit2sharp/libgit2/src/transports/*.c",
    depsdir .. "/libgit2sharp/libgit2/src/xdiff/*.c",
    depsdir .. "/libgit2sharp/libgit2/src/xdiff/*.c",
    depsdir .. "/libgit2sharp/libgit2/src/hash/hash_generic.c",
    depsdir .. "/libgit2sharp/libgit2/deps/http-parser/*.c",
    depsdir .. "/libgit2sharp/libgit2/deps/zlib/*.c",
  }
  
  includedirs
  { 
    depsdir .. "/libgit2sharp/libgit2",
    depsdir .. "/libgit2sharp/libgit2/src",
    depsdir .. "/libgit2sharp/libgit2/include",
    depsdir .. "/libgit2sharp/libgit2/deps/http-parser",
    depsdir .. "/libgit2sharp/libgit2/deps/zlib",
    depsdir .. "/libssh2/include",
    depsdir .. "/libssl/include"
  }
  
  defines
  {
    "NO_VIZ",
    "STDC",
    "NO_GZIP",

    "_FILE_OFFSET_BITS=64",
    "_GNU_SOURCE",
  }

  -- Disable SSH and SSL support for now
  --[[
  defines
  {
    "GIT_SSL",
    "GIT_SSH"
  }

  links
  {
    "ssl",
    "crypto",
    "libssh2"
  }
  ]]
  
  configuration "windows"
    links { "Bcrypt.lib", "Crypt32.lib" }
  
  configuration "vs*"
    callingconvention "StdCall"
    buildoptions { "/wd4133", "/wd4244","/wd4267", "/wd4996" } 
  
  configuration { "windows" }
    defines
    {
      "WIN32", 
      "_WIN32_WINNT=0x0501",
      "__USE_MINGW_ANSI_STDIO=1"
    }
    
    files
    {
      depsdir .. "/libgit2sharp/libgit2/src/win32/*.c",
      depsdir .. "/libgit2sharp/libgit2/src/compat/*.c",
      depsdir .. "/libgit2sharp/libgit2/deps/regex/regex.c",
    }

    includedirs
    {
      depsdir .. "/libgit2sharp/libgit2/deps/regex"
    }
    
  configuration { "not windows" }
    files
    {
      depsdir .. "/libgit2sharp/libgit2/src/unix/*.c",
    }

--dofile "LibOpenSSL.lua"
--dofile "LibSSH2.lua"