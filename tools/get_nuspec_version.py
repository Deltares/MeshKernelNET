import argparse
import sys
import xml.etree.ElementTree as ET
from pathlib import Path


def get_path() -> Path:
    """
    Parse the arguments with which this script is called
    """
    parser = argparse.ArgumentParser()

    parser.add_argument(
        "--file",
        "-f",
        type=Path,
        required=True,
        help="Path to the nuspec file to parse.",
    )

    args = parser.parse_args()

    return args.file


def is_nuspec_file(file: Path) -> bool:
    """
    Parse the arguments with which this script is called
    """
    return file.suffix == ".nuspec"


def get_version(file: Path) -> str:
    """
    Get the nuspec version by parsing a nuspec file
    """
    if not is_nuspec_file(file):
        raise Exception(str(file) + " is not a nuspec file.")

    # Parse the XML file
    tree = ET.parse(file)

    # Get the root node
    root = tree.getroot()

    # Extract the namespace from the root element
    namespace = root.tag.split("}")[0][1:]

    # Find the version element
    version = root.find(".//{%s}metadata/{%s}version" % (namespace, namespace))

    if version is not None:
        return version.text
    else:
        raise Exception("Could not find metadata/version element in " + str(file))


if __name__ == "__main__":
    version = str()
    try:
        file = get_path()
        version = get_version(file)
    except Exception as error:
        print("Error:", error, file=sys.stderr)

    print(version)
