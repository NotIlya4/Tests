import argparse
from commons import *

parser = argparse.ArgumentParser()
parser.add_argument("--name", type=str, required=True)

parse = parser.parse_args()
name = parse.name

print(read_nat_ip(name))