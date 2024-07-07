import argparse
from commons import read_workers
import yaml
from jinja2 import Environment, Template

def create_remote_k3s_inventory(workers_raw):
    workers = yaml.safe_load(workers_raw)

    template_str = """nodes:
    hosts:
        {% for worker in workers %}
        {{ worker.name }}:
            ansible_host: {{ worker.ip }}
        {% endfor %}
    vars:
        ansible_ssh_private_key_file: /ssh.key
        k3s_server:
            flannel-backend: 'none'
            disable-network-policy: true
            disable:
                - traefik
                - servicelb"""

    # Create a Jinja2 environment
    env = Environment(trim_blocks=True, lstrip_blocks=True)

    # Render the template with the workers data
    template = env.from_string(template_str).render(workers=workers['workers'])
    return template

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--name", type=str, required=True)

    parse = parser.parse_args()
    name = parse.name

    template = create_remote_k3s_inventory(read_workers(name))
    print(template)
