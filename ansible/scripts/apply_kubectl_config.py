import argparse
from commons import *
import yaml
import base64


def apply_kubectl_config(name, config, ip):
    def remove_temp_files():
        Path(client_key_file).unlink(missing_ok=True)
        Path(client_certificate_file).unlink(missing_ok=True)

    client_key_file = f'./{name}-client-key.generated'
    client_certificate_file = f'./{name}-client-certificate.generated'

    user = f'{name}-user'
    cluster = f'{name}-cluster'
    context = f'{name}-context'

    remove_temp_files()

    microk8s_config = yaml.safe_load(config)

    Path(client_key_file).write_text(base64.b64decode(microk8s_config['users'][0]['user']['client-key-data']).decode('utf-8'))
    Path(client_certificate_file).write_text(base64.b64decode(microk8s_config['users'][0]['user']['client-certificate-data']).decode('utf-8'))

    run(f'kubectl config set-credentials {user} --client-key={client_key_file} --client-certificate={client_certificate_file} --embed-certs=true')

    remove_temp_files()

    port = microk8s_config['clusters'][0]['cluster']['server'].split(':')[-1]
    url = f'https://{ip}:{port}'

    run(f'kubectl config set-cluster {cluster} --insecure-skip-tls-verify=true --server={url}')
    run(f'kubectl config set-context {context} --cluster={cluster} --user={user}')
    run(f'kubectl config use-context {context}')


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--name", type=str, required=True)

    parse = parser.parse_args()
    name = parse.name

    ip = read_nat_ip(name)

    output_path = find_dot_output_folder()
    config = run(f'ssh -o StrictHostKeychecking=no ubuntu@{ip} -i {output_path}/ssh.key "sudo kubectl config view --raw"')

    apply_kubectl_config(name, config.stdout, ip)