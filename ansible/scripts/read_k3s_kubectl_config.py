from commons import *
import argparse


parser = argparse.ArgumentParser()
parser.add_argument("--ip", type=str, required=True)

parse = parser.parse_args()
ip = parse.ip
output_path = find_dot_output_folder()

config = run(f'ssh -o StrictHostKeychecking=no ubuntu@{ip} -i {output_path}/ssh.key "sudo kubectl config view --raw"').stdout
print(config)