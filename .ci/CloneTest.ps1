Param([string]$repo)

$msbuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"

# *** Test_Toolkit requires BHoM and BHoM_Engine in order to build correctly for compliance tests, so here we make sure we have BHoM and BHoM_Engine ***

# **** Cloning BHoM ****
$repo = "BHoM"

# **** Defining BHoM Paths ****
$slnPath = "$ENV:BUILD_SOURCESDIRECTORY\$repo\$repo.sln"

# **** Cloning Repo ****
Write-Output ("BHoM Cloning " + $repo + " to " + $ENV:BUILD_SOURCESDIRECTORY + "\" + $repo)

If((git rev-parse --verify --quiet ("origin/"+$ENV:SYSTEM_PULLREQUEST_SOURCEBRANCH)).length -gt 0)
{
	Write-Output("Changing branch in repo " + $repo + " to " + $ENV:SYSTEM_PULLREQUEST_SOURCEBRANCH)
	git clone -q --branch=$ENV:SYSTEM_PULLREQUEST_SOURCEBRANCH https://github.com/BHoM/$repo.git  $ENV:BUILD_SOURCESDIRECTORY\$repo
}
Else
{
	git clone -q --branch=master https://github.com/BHoM/$repo.git  $ENV:BUILD_SOURCESDIRECTORY\$repo
}


# **** Restore BHoM NuGet ****
Write-Output ("Restoring NuGet packages for " + $repo)
& NuGet.exe restore $slnPath

# **** Preparing BHoM_Engine ****
$repo = "BHoM_Engine"

# **** Defining BHoM_Engine Paths ****
$slnPath = "$ENV:BUILD_SOURCESDIRECTORY\$repo\$repo.sln"

# **** Restore BHoM_Engine NuGet ****
Write-Output ("Restoring NuGet packages for " + $repo)
& NuGet.exe restore $slnPath

# **** Cloning BHoM Test_Toolkit ****
$repo = "Test_Toolkit"

# **** Defining Paths ****
$slnPath = "$ENV:BUILD_SOURCESDIRECTORY\$repo\$repo.sln"

# **** Cloning Repo ****
Write-Output ("Cloning " + $repo + " to " + $ENV:BUILD_SOURCESDIRECTORY + "\" + $repo)
git clone -q --branch=master https://github.com/BHoM/$repo.git  $ENV:BUILD_SOURCESDIRECTORY\$repo

# **** Restore NuGet ****
Write-Output ("Restoring NuGet packages for " + $repo)
& NuGet.exe restore $slnPath

write-Output ("Building BHoM.sln")
& $msbuildPath -nologo "$ENV:BUILD_SOURCESDIRECTORY\BHoM\BHoM.sln" /verbosity:minimal

write-Output ("Building BHoM_Engine.sln")
& $msbuildPath -nologo "$ENV:BUILD_SOURCESDIRECTORY\BHoM_Engine\BHoM_Engine.sln" /verbosity:minimal