import argparse

import yaml
from commons import *


parser = argparse.ArgumentParser()
parser.add_argument("--name", type=str, required=True)

parse = parser.parse_args()
name = parse.name

workers = yaml.safe_load(read_workers(name))

worker_names = []
for worker in workers['workers']:
    worker_names.append(worker['name'])

print(','.join(worker_names))