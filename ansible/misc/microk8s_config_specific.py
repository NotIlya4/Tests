import subprocess
import argparse
from microk8s_config import apply_microk8s_config
from commons import *


parser = argparse.ArgumentParser()
parser.add_argument("--name", type=str, required=True)
parser.add_argument("--ip-path", type=str, required=True)

parse = parser.parse_args()
name = parse.name
ip_path = parse.ip_path
microk8s_config_path = f'./{name}-microk8s-config.generated'

microk8s_config = subprocess.run(f'just microk8s-config {name}', capture_output=True, shell=True, text=True).stdout
write(microk8s_config_path, microk8s_config)

ip = read(ip_path)

apply_microk8s_config(name, microk8s_config_path, ip)

rm(microk8s_config_path)