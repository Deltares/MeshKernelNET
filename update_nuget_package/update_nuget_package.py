from itertools import chain
from pathlib import Path
import re
import argparse


def search_files(folder_path: Path, extension: str):
    """Returns full file paths with the specified extension in the folder."""
    return folder_path.glob(f"**/*{extension}")


def update_files(file_paths, find, replace, encoding=None):
    """Finds and replaces the content for each specified file."""
    for p in file_paths:
        with p.open(encoding=encoding) as f:
            lines = f.read()
            lines = find.sub(replace, lines)

        with p.open(mode="w", encoding=encoding) as f:
            f.writelines(lines)


def get_args():
    """Parses and returns the arguments"""
    parser = argparse.ArgumentParser()
    parser.add_argument("root_path", help="Path to the root of the working directory")
    parser.add_argument("package_name", help="The name of the NuGet package.")
    parser.add_argument(
        "new_version",
        help="The full version to update to i.e. Major.Minor.Patch[-Postfix]",
    )

    return parser.parse_args()


def get_version_regex_string() -> str:
    """Gets the regex string to match a version number"""
    integer_regex = r"(0|([1-9]\d*))"
    integers_regex = rf"(\.{integer_regex})*"
    postfix = r"(?:-\w+)?"

    return f"{integer_regex}{integers_regex}{postfix}"


def get_new_version_string(string: str) -> str:
    """Removes the leading zeros from the version string"""
    postfix_split = string.split("-", 1)

    version = postfix_split[0]
    postfix = f"-{postfix_split[1]}" if len(postfix_split) > 1 else ""

    return ".".join(str(int(x)) for x in version.split(".")) + postfix


if __name__ == "__main__":
    args = get_args()
    root_path = Path(args.root_path)
    package_name = args.package_name
    new_version_string = get_new_version_string(args.new_version)

    escaped_package_name = re.escape(package_name)
    version_regex = get_version_regex_string()

    project_file_paths = search_files(root_path, ".csproj")
    props_file_paths = search_files(root_path, "Directory.Packages.props")
    
    file_paths_to_update = chain(project_file_paths, props_file_paths)
    
    update_files(
        file_paths_to_update,
        re.compile(f'"{escaped_package_name}" Version="{version_regex}"'),
        f'"{package_name}" Version="{new_version_string}"',
        "utf-8-sig",
    )
