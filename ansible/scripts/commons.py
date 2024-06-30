from pathlib import Path
import subprocess
import os


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


def run(command):
    return subprocess.run(command, text=True, shell=True, capture_output=True)


def write_output(path, value):
    path = Path(find_dot_output_folder(), path)
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(value)


def read_output(path):
    return Path(find_dot_output_folder(), path).read_text()


def rm_output(path):
    Path(find_dot_output_folder(), path).unlink(missing_ok=True)


def read_private_ip(name):
    return read_output(f'{name}/private_ip_address.txt')


def read_nat_ip(name):
    return read_output(f'{name}/nat_ip_address.txt')


def read_lb_address(name):
    return read_output(f'{name}/lb_external_address.txt')


def write_ansible_inventory(value):
    write_output('inventory.yaml', value)


def read_workers(name):
    return read_output(f'{name}/workers.yaml')


def read_cluster_ip(name):
    return read_output(f'{name}/cluster-ip.txt')