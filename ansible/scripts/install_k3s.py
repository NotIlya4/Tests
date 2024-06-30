import argparse
from commons import *
from create_k3s_inventory import create_k3s_inventory
from pathlib import *


def write_to_k3s_output(path, value):
    write_output(f'k3s_transfer_files/{path}', value)


def install_k3s(master_name, worker_names):
    write_to_k3s_output('inventory.yaml', create_k3s_inventory(master_name, worker_names))


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--master-name", type=str, required=True)
    parser.add_argument("--worker-names", type=str, required=False)

    parse = parser.parse_args()
    master_name = parse.master_name
    worker_names = parse.worker_names
    print('asd')