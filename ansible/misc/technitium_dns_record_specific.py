import argparse
from technitium_dns_record import set_record
from commons import *


parser = argparse.ArgumentParser()
parser.add_argument("--ip-path", type=str, required=True)

parse = parser.parse_args()
ip_path = parse.ip_path

set_record('krupcov.ru', read(ip_path))