dofile (depsdir .. "/libssl/premake/ssl.lua")
dofile (depsdir .. "/libssl/premake/crypto.lua")

project "ssl"
	configuration "windows"
		defines { "WIN32_LEAN_AND_MEAN" }

	configuration "vs*"
		callingconvention "StdCall"
		buildoptions { "/wd4244","/wd4267", "/wd4996" }	

project "crypto"

	configuration "windows"
		defines
		{
			"NOCRYPT", -- workaround wincrypt.h conflict
			"OPENSSL_SYSNAME_WIN32",
			"NO_WINDOWS_BRAINDEATH",
		}
	
	configuration "vs*"
		buildoptions { "/wd4164","/wd4244","/wd4267", "/wd4996" }	
		callingconvention "StdCall"
