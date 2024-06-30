import argparse
from commons import *


def create_k3s_inventory(master_name, worker_names = ''):
    worker_sum_template = ''

    if (worker_names != None):
        worker_names = worker_names.split(',')

        for worker_name in worker_names:
            worker_template = f"""
        {worker_name}:
            ansible_host: {read_private_ip(worker_name)}"""
            worker_sum_template = worker_sum_template + worker_template

    template = f"""nodes:
    hosts:
        {master_name}:
            ansible_host: {read_private_ip(master_name)}
            k3s_control_node: true
            k3s_server:
                disable:
                - traefik{worker_sum_template}
    vars:
        ansible_ssh_private_key_file: /ssh.key"""
    return template


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--master-name", type=str, required=True)
    parser.add_argument("--worker-names", type=str, required=False)

    parse = parser.parse_args()
    master_name = parse.master_name
    worker_names = parse.worker_names

    template = create_k3s_inventory(master_name, worker_names)

    write_output('k3s-inventory.yaml', template)