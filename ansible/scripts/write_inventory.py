from commons import *
import argparse


parser = argparse.ArgumentParser()
parser.add_argument("--inventory", type=str, required=True)

parse = parser.parse_args()
inventory = parse.inventory

write_ansible_inventory(inventory)