import argparse
import requests
import json

# Configuration
api_url = 'http://localhost:5380/api'
admin_username = 'admin'  # Replace with your admin username
admin_password = 'admin'  # Replace with your admin password
record_name = '*'
record_type = 'A'

def set_record(host, ip):
    token = json.loads(requests.get(f'{api_url}/user/login?user={admin_username}&pass={admin_password}').content.decode('utf-8'))['token']

    requests.get(f'{api_url}/zones/create?token={token}&zone={host}&type=Primary')
    requests.get(f'{api_url}/zones/records/add?token={token}&domain={record_name}.{host}&zone={host}&type={record_type}&ipAddress={ip}&overwrite=true')


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--host", type=str, required=True)
    parser.add_argument("--ip", type=str, required=True)

    parse = parser.parse_args()
    host = parse.host
    ip = parse.ip

    set_record(host, ip)    