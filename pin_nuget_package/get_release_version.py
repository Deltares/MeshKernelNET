"""
Retrieves the public release version of the D-HYDRO product from the specified repository.
"""
from pathlib import Path

import argparse
import xml.etree.ElementTree as et

FILE_CONTAINING_VERSION = Path("./setup/FM/WixUIVariables.wxl")


def retrieve_release_version(wxl_path: Path) -> str:
    """Retrieves the public release version from the WixUIVariables.wxl file.
    The version can be found within the 'String' element of which the attribute 'Id' matches 'PublicReleaseVersion'.

    Args:
        wxl_path (Path): The path to the WixUIVariables.wxl file.

    Returns:
        str: The public release version.
    """

    root = et.parse(wxl_path).getroot()
    ns = {"d": "http://schemas.microsoft.com/wix/2006/localization"}
    release_version_attrib = root.find("d:String/[@Id='PublicReleaseVersion']", ns)
    release_version = release_version_attrib.text

    return release_version


def run(root_directory: Path) -> str:
    """Retrieves the public release version from the WixUIVariables.wxl file.
    The version can be found within the 'String' element of which the attribute 'Id' matches 'PublicReleaseVersion'.

    Args:
        root_directory (Path): The root directory of the repository.

    Returns:
        str: The public release version.
    """
    return retrieve_release_version(root_directory / FILE_CONTAINING_VERSION)


def parse_arguments():
    """
    Parse the arguments with which this script was called through
    """
    parser = argparse.ArgumentParser()
    parser.add_argument("root_directory", help="The root directory of the repository.")

    return parser.parse_args()


if __name__ == "__main__":
    args = parse_arguments()
    root = Path(args.root_directory)
    release_version = retrieve_release_version(root / FILE_CONTAINING_VERSION)
    print(release_version)
