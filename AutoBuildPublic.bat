::Configuration Settings
set SolutionName=Medical.sln

::Less likely to need to change these.
set ThisFolder=%~dp0
set RootDependencyFolder=%ThisFolder%..\
set BuildCommand="C:\Program Files (x86)\MSBuild\12.0\Bin\msbuild.exe" /m
set CurrentDirectory=%CD%

set SolutionPath=%ThisFolder%%SolutionName%

%BuildCommand% "%SolutionPath%" /property:Configuration=PublicRelease;Platform="Any CPU" /target:Clean
%BuildCommand% "%SolutionPath%" /property:Configuration=PublicRelease;Platform="Any CPU"

%BuildCommand% "%SolutionPath%" /property:Configuration=PublicRelease;Platform="x64" /target:Clean
%BuildCommand% "%SolutionPath%" /property:Configuration=PublicRelease;Platform="x64"