Param([string]$repo)

$msbuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"

# *** Test_Toolkit requires BHoM and BHoM_Engine in order to build correctly for compliance tests, so here we make sure we have BHoM and BHoM_Engine ***

# **** Cloning BHoM ****
$repo = "BHoM"

# **** Defining BHoM Paths ****
$slnPath = "$ENV:BUILD_SOURCESDIRECTORY\Test_Toolkit_Corner\$repo\$repo.sln"

# **** Cloning BHoM Repo ****
Write-Output ("BHoM Cloning " + $repo + " to " + $ENV:BUILD_SOURCESDIRECTORY + "\Test_Toolkit_Corner\" + $repo)

If(!(test-path $ENV:BUILD_SOURCESDIRECTORY\Test_Toolkit_Corner\$repo))
{
	git clone -q --branch=master https://github.com/BHoM/$repo.git  $ENV:BUILD_SOURCESDIRECTORY\Test_Toolkit_Corner\$repo
}

# **** Restore BHoM NuGet ****
Write-Output ("Restoring NuGet packages for " + $repo)
& NuGet.exe restore $slnPath

# **** Preparing BHoM_Engine ****
$repo = "BHoM_Engine"

# **** Defining BHoM_Engine Paths ****
$slnPath = "$ENV:BUILD_SOURCESDIRECTORY\Test_Toolkit_Corner\$repo\$repo.sln"

# **** Cloning BHoM_Engine Repo ****
Write-Output ("BHoM_Engine Cloning " + $repo + " to " + $ENV:BUILD_SOURCESDIRECTORY + "\Test_Toolkit_Corner\" + $repo)

If(!(test-path $ENV:BUILD_SOURCESDIRECTORY\Test_Toolkit_Corner\$repo))
{
	git clone -q --branch=master https://github.com/BHoM/$repo.git  $ENV:BUILD_SOURCESDIRECTORY\Test_Toolkit_Corner\$repo
}

# **** Restore BHoM_Engine NuGet ****
Write-Output ("Restoring NuGet packages for " + $repo)
& NuGet.exe restore $slnPath

# **** Cloning BHoM Test_Toolkit ****
$repo = "Test_Toolkit"

# **** Defining BHoM Test_Toolkit Paths ****
$slnPath = "$ENV:BUILD_SOURCESDIRECTORY\Test_Toolkit_Corner\$repo\$repo.sln"

# **** Cloning BHoM Test_Toolkit Repo - this one to its default location for access PS scripts ****
Write-Output ("Cloning " + $repo + " to " + $ENV:BUILD_SOURCESDIRECTORY + "\" + $repo)
git clone -q --branch=master https://github.com/BHoM/$repo.git  $ENV:BUILD_SOURCESDIRECTORY\$repo

# **** Cloning BHoM Test_Toolkit Repo - this one quarantined for building as necessary ****
Write-Output ("Cloning " + $repo + " to " + $ENV:BUILD_SOURCESDIRECTORY + "\" + $repo)
git clone -q --branch=master https://github.com/BHoM/$repo.git  $ENV:BUILD_SOURCESDIRECTORY\Test_Toolkit_Corner\$repo

# **** Restore NuGet ****
Write-Output ("Restoring NuGet packages for " + $repo)
& NuGet.exe restore $slnPath

write-Output ("Building BHoM.sln")
& $msbuildPath -nologo "$ENV:BUILD_SOURCESDIRECTORY\Test_Toolkit_Corner\BHoM\BHoM.sln" /verbosity:minimal

write-Output ("Building BHoM_Engine.sln")
& $msbuildPath -nologo "$ENV:BUILD_SOURCESDIRECTORY\Test_Toolkit_Corner\BHoM_Engine\BHoM_Engine.sln" /verbosity:minimal