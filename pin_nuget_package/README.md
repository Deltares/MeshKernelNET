# pin_nuget_package.py

`pin_nuget_package.py` pins and tags a specified NuGet package on TeamCity through the 
TeamCity REST API. This ensures the artifact is not automatically cleaned up when it
is being used. The previous pinned and tagged artifact will, by default, be moved to
the newly specified build. This is especially useful in combination with automatic updates 
of NuGet packages.

## Example Usage

```cmd
REM Current working directory is assumed to be the pin_nuget_package directory.
CALL conda env create -f environment.yml
REM Note that the environment name is similar as the folder structure
CALL activate pin_nuget_package
REM The provided parameters are set in TeamCity
CALL python "pin_nuget_package.py" %nuget_package_name% %public_nuget_version% %depBuildID% %tag_value% %svn_buildserver_username% %svn_buildserver_password% 
CALL conda deactivate
CALL conda remove -y -n pin_nuget_package --all
```

# pin_nuget_packages_from_csproj_or_props

`pin_nuget_packages_from_csproj_or_props.py` pins and tags the specified NuGet packages based upon the versions found in the .csproj and Directory.Packages.props files. The script first checks the .csproj files. If no version is found in the .csproj files, the scripts checks the Directory.Packages.props files for the version number. The `pin_nuget_packages_from_csproj_or_props.py` reuses the logic from `pin_nuget_package.py` to actually pin and tag the files,
and merely extends the logic provided to retrieve the specific versions from the source files.
When using this script in release mode, the `--release` argument can be used. When enabled, the script will use the `get_release_version.py` 
script to tag the dependencies with the correct release version. 

# get_release_version

`get_release_version.py` retrieves the product release version from the setup file `WixUIVariables.wxl`.
The result of the script is for instance `2023.02`.

## Exmaple Usage

```cmd
REM Current working directory is assumed to be the pin_nuget_package directory.
CALL conda env create -f environment.yml
REM Note that the environment name is similar as the folder structure
CALL activate pin_nuget_package
REM The provided parameters are set in TeamCity
CALL python "pin_nuget_packages_from_csproj_or_props.py" ^
         %svn_buildserver_username% ^
         %svn_buildserver_password% ^ 
         %tag_value% ^
         (Dimr.Libs, DHydroUserInterface_DHydroExternalLibraries_KernelNuGetPackages_Dimr) ^
         (RGFGRID, DHydroUserInterface_DHydroExternalLibraries_KernelNuGetPackages_Rgfgrid) ^
         (DIDO, DHydroUserInterface_DHydroExternalLibraries_KernelNuGetPackages_Dido) ^
         (PLCT.Libs, DHydroUserInterface_DHydroExternalLibraries_KernelNuGetPackages_Plct) ^
         (Substances.Libs, DHydroUserInterface_DHydroExternalLibraries_KernelNuGetPackages_Substances) ^
         (DeltaShell.Framework, DeltaShellFramework_DeltaShellFrameworkGit_NuGet_DeltaShellFrameworkSigned) ^
		 --release	 
CALL conda deactivate
CALL conda remove -y -n pin_nuget_package --all
```