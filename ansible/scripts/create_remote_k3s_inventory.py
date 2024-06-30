import argparse
from commons import *
import yaml


def create_remote_k3s_inventory(workers_raw):
    workers = yaml.safe_load(workers_raw)

    worker_sum_template = ''

    i = 0
    for worker in workers['workers']:
        i += 1
        worker_template = f"""
        {worker['name']}:
            ansible_host: {worker['ip']}"""
        worker_sum_template = worker_sum_template + worker_template

    template = f"""nodes:
    hosts:{worker_sum_template}
    vars:
        ansible_ssh_private_key_file: /ssh.key
        k3s_server:
            disable:
            - traefik"""
    return template


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--name", type=str, required=True)

    parse = parser.parse_args()
    name = parse.name

    template = create_remote_k3s_inventory(read_workers(name))
    print(template)