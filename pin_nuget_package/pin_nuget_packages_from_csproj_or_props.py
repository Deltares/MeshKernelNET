"""
Pins and tags the specified NuGet package on TeamCity. Removes the tag, if found, from a previous 
build and removes the pin when possible. The specific version numbers are retrieved from the 
csproj or Directory.Packages.props files in the provided folder.
"""

from dataclasses import dataclass
from itertools import chain
from pathlib import Path
from typing import Optional

import get_release_version
import pin_nuget_package
import argparse
import xml.etree.ElementTree as et


@dataclass
class NuGetDescription:
    """
    NuGetDescription describes a single NuGet package with a specific name and build id
    """

    name: str
    id: str

    @classmethod
    def from_input_str(cls, input: str) -> "NuGetDescription":
        name, id = input.strip("()").split(",")
        return NuGetDescription(name.strip(), id.strip())


def search_files(folder_path: Path, extension: str):
    """Returns full file paths with the specified extension in the folder."""
    return folder_path.glob(f"**/*{extension}")


def find_nuget_version_in_file(nuget_name: str, file_path: Path) -> Optional[str]:
    """Find the nuget version of nuget_name in the csproj or Directory.Packages.props
    file corresponding with the provided file_path. If no version is found,
    None is returned.

    Args:
        nuget_name (str): The name of the nuget package
        file_path (Path): The path to the csproj or Directory.Packages.props file

    Returns:
        Optional[str]: The nuget version of nuget_name if found, otherwise None.
    """
    root = et.parse(file_path).getroot()
    package_reference_iterator = (
        pr
        for pr in root.findall(f"ItemGroup/*[@Include='{nuget_name}']")

    )
    package_reference = next(package_reference_iterator, None)

    if package_reference is not None:
        return package_reference.get("Version", None)
    return None


def find_nuget_version(nuget_name: str, folder_path: Path) -> Optional[str]:
    """Find the nuget version of nuget_name in the repository at folder_path.
    First check the the .csproj files, and if the nuget_name is not found,
    then check the Directory.Packages.props.

    Args:
        nuget_name (str): The name of the nuget package
        folder_path (Path): The path to the folder to start searching for files

    Returns:
        Optional[str]: The nuget version of nuget_name if found, otherwise None.
    """
    project_file_paths = search_files(folder_path, ".csproj")
    props_file_paths = search_files(folder_path, "Directory.Packages.props")
    file_paths_to_update = chain(project_file_paths, props_file_paths)

    for path in file_paths_to_update:
        if nuget_version := find_nuget_version_in_file(nuget_name, path):
            return nuget_version
    return None


def parse_arguments():
    """
    Parse the arguments with which this script was called through
    """
    parser = argparse.ArgumentParser()

    parser.add_argument("user", help="The user to authenticate with.")
    parser.add_argument("password", help="The password to authenticate with.")
    parser.add_argument(
        "tag", help="The tag to pin the build of the specified NuGet package with."
    )
    parser.add_argument(
        "root_directory", help="The root directory to search for nugets."
    )
    parser.add_argument(
        "nuget_libraries",
        help="The nuget properties written as (<nuget name>, <build configuration id>).",
        nargs="*",
        type=str,
    )
    parser.add_argument(
        "--release",
        help="The NuGet package should be pinned with the product release version. The release version will be appended to the 'tag' argument when this option is True.",
        action="store_true",
    )

    return parser.parse_args()


if __name__ == "__main__":
    args = parse_arguments()

    nuget_descriptions = (
        NuGetDescription.from_input_str(nuget) for nuget in args.nuget_libraries
    )

    root = Path(args.root_directory)

    tag = args.tag
    if args.release:
        tag += "_" + get_release_version.run(root)

    for nuget in nuget_descriptions:
        if nuget_version := find_nuget_version(nuget.name, root):
            pin_nuget_package.run(
                nuget.name,
                nuget_version,
                nuget.id,
                tag,
                args.user,
                args.password,
            )
