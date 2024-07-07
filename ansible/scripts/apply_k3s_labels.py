import argparse
from typing import List

import yaml
from commons import *


name_suffix = '-node'


def apply_k3s_labels(workers):
    def apply_node_label(node_name, label_name, label_value):
        run(f'kubectl label nodes {node_name}{name_suffix} {label_name}={label_value} --overwrite=true')
    
    workers = yaml.safe_load(workers)['workers']

    for worker in workers:
        for label in worker['labels'].items():
            apply_node_label(worker['name'], label[0], label[1])


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--name", type=str, required=True)

    parse = parser.parse_args()
    name = parse.name

    apply_k3s_labels(read_workers(name))