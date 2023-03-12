import json


def read_json_config(filename):
    with open(filename, mode="r") as file:
        return json.loads(file.read())