@echo Off

REM Build

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild ..\src\TypeAlias.Net20.sln /p:Configuration="Release" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild ..\src\TypeAlias.Net35.sln /p:Configuration="Release" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild ..\src\TypeAlias.Net40.sln /p:Configuration="Release" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM Package

rd /s/q bin
mkdir bin
mkdir bin\lib
mkdir bin\lib\net20
mkdir bin\lib\net35
mkdir bin\lib\net40

copy ..\src\output\TypeAlias\bin\Net20\Release\TypeAlias.* bin\lib\net20
copy ..\src\output\TypeAlias\bin\Net35\Release\TypeAlias.* bin\lib\net35
copy ..\src\output\TypeAlias\bin\Net40\Release\TypeAlias.* bin\lib\net40

call nuget.exe pack TypeAlias.nuspec -BasePath bin
