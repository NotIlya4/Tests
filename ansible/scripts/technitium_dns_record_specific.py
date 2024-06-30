import argparse
from technitium_dns_record import set_record
from commons import *


parser = argparse.ArgumentParser()
parser.add_argument("--name", type=str, required=True)

parse = parser.parse_args()
name = parse.name

set_record('krupcov.ru', read_lb_address(name))