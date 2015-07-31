project "libssh2"

  kind "StaticLib"
  language "C++"
  
  files { depsdir .. "/libssh2/src/*.c" }
  excludes { depsdir .. "/libssh2/src/win*.c" }
  includedirs { depsdir .. "/libssh2/include" }
  
  configuration "windows"
    defines { "LIBSSH2_WINCNG", "HAVE_LIBCRYPT32", "HAVE_WINDOWS_H", "STATUS_SUCCESS=0" }
    files { depsdir .. "/libssh2/src/win*.c" }
    excludes { depsdir .. "/libssh2/src/openssl.c" }
    includedirs { depsdir .. "/libssh2/win32" }

  configuration "vs*"
    callingconvention "StdCall"
    buildoptions { "/wd4013 /wd4018 /wd4047 /wd4133 /wd4244" }

