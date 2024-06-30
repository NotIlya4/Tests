import subprocess
import os
import yaml
import argparse
import base64
from commons import *


def apply_microk8s_config(name, microk8s_config_path, ip):
    def remove_temp_files():
        rm(client_key_file)
        rm(client_certificate_file)


    ip = f'https://{ip}:16443'

    client_key_file = f'./{name}-client-key.generated'
    client_certificate_file = f'./{name}-client-certificate.generated'

    user = f'{name}-user'
    cluster = f'{name}-cluster'
    context = f'{name}-context'

    remove_temp_files()

    microk8s_config = yaml.safe_load(read(microk8s_config_path))

    write(client_key_file, base64.b64decode(microk8s_config['users'][0]['user']['client-key-data']).decode('utf-8'))
    write(client_certificate_file, base64.b64decode(microk8s_config['users'][0]['user']['client-certificate-data']).decode('utf-8'))

    run(f'kubectl config set-credentials {user} --client-key={client_key_file} --client-certificate={client_certificate_file} --embed-certs=true')

    remove_temp_files()

    run(f'kubectl config set-cluster {cluster} --insecure-skip-tls-verify=true --server={ip}')
    run(f'kubectl config set-context {context} --cluster={cluster} --user={user}')
    run(f'kubectl config use-context {context}')


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--name", type=str, required=True, help="Name")
    parser.add_argument("--microk8s-config-path", type=str, required=True, help="Microk8s config path")
    parser.add_argument("--ip", type=str, required=True, help="Ip address of a cluster")

    parse = parser.parse_args()
    name = parse.name
    microk8s_config_path = parse.microk8s_config_path
    ip = parse.ip

    apply_microk8s_config(name, microk8s_config_path, ip)