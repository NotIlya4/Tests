import argparse
from typing import List
from commons import *


name_suffix = '-node'


def apply_labels_by_convention(node_names: List[str]):
    def apply_node_label(node_name, label_name, label_value):
        run(f'kubectl label nodes {node_name}{name_suffix} {label_name}={label_value}')
    
    def apply_convention(node_name, convention):
        if (convention in node_name):
            apply_node_label(node_name, f'node-role.kubernetes.io/{convention}', 'true')
    
    for node_name in node_names:
        apply_convention(node_name, 'worker')
        apply_convention(node_name, 'kafka')


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--node-names", type=str, required=True)

    parse = parser.parse_args()
    node_names = parse.node_names.split(',')

    apply_labels_by_convention(node_names)