import argparse
from commons import *

output_path = "../.output"

parser = argparse.ArgumentParser()
parser.add_argument("--ip-path", type=str, required=True)
parser.add_argument("--ssh-key-path", type=str, required=True)

parse = parser.parse_args()
ip_path = parse.ip_path
ssh_key_path = parse.ssh_key_path

inventory_file = f"{output_path}/inventory.ini.generated"

ip = read(f'{output_path}/{ip_path}')

with open(inventory_file, "w") as f:
    f.write("[nodes]\n")
    f.write(f"{ip} ansible_user=ubuntu ansible_ssh_private_key_file={output_path}/{ssh_key_path}\n")