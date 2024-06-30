import os
import json
import argparse


def find_dot_output_folder():
    current_path = os.path.abspath(__file__)

    while True:
        output_folder_path = os.path.join(current_path, '.output')

        if os.path.isdir(output_folder_path):
            return os.path.abspath(output_folder_path)

        new_path = os.path.abspath(os.path.join(current_path, '..'))

        if new_path == current_path:
            return None

        current_path = new_path


parser = argparse.ArgumentParser()
parser.add_argument("--json", action=argparse.BooleanOptionalAction, default=True)

parse = parser.parse_args()
do_json = parse.json

path = find_dot_output_folder()
if do_json:
    print(json.dumps({"path": path}))
else:
    print(path)