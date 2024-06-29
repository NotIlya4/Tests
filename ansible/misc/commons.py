import subprocess
import os


def run(command):
        return subprocess.run(command, text=True, shell=True)


def read(path):
    with open(path, 'r') as file:
        data = file.read()
    return data


def write(path, value):
    with open(path, 'w+') as file:
        file.write(value)


def rm(path):
    if os.path.exists(path):
        os.remove(path)