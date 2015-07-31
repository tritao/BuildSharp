project "lua5.1"

	SetupNativeProject()
	language "C"
	kind "SharedLib"

	files
	{
		depsdir .. "/lua/src/*.c",
		depsdir .. "/lua/src/*.cpp",
	}

	excludes
	{
		depsdir .. "/lua/src/lua.c",
		depsdir .. "/lua/src/luac.c"
	}

	defines "LUA_BUILD_AS_DLL"

	configuration "vs*"
		buildoptions { "/wd4996" }
