import argparse
from commons import *


parser = argparse.ArgumentParser()
parser.add_argument("--ip", type=str, required=True)

parse = parser.parse_args()
ip = parse.ip

template = f"""nodes:
  hosts:
    node:
      ansible_host: {ip}
      ansible_user: ubuntu
      ansible_ssh_private_key_file: ../.output/ssh.key"""

print(template)