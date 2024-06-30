import argparse
from commons import *

parser = argparse.ArgumentParser()
parser.add_argument("--name", type=str, required=True)

parse = parser.parse_args()
name = parse.name

ip = read_nat_ip(name)

template = f"""nodes:
  hosts:
    node:
      ansible_host: {ip}
      ansible_user: ubuntu
      ansible_ssh_private_key_file: ../.output/ssh.key"""

write_ansible_inventory(template)