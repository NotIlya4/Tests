import argparse


def update_dns_record(hostname, ip_address):
    hosts_file_path = r'C:\Windows\System32\drivers\etc\hosts'
    new_entry = f'{ip_address} {hostname}\n'

    # Read the current hosts file content
    with open(hosts_file_path, 'r') as f:
        lines = f.readlines()

    # Check if the entry already exists
    entry_exists = False
    for i, line in enumerate(lines):
        if line.strip().endswith(hostname):
            # Replace existing entry
            lines[i] = new_entry
            entry_exists = True
            break

    # If entry does not exist, append it
    if not entry_exists:
        lines.append(f'\n{new_entry}')

    # Write the updated content back to the hosts file
    with open(hosts_file_path, 'w') as f:
        f.writelines(lines)

    if entry_exists:
        print(f"Updated entry for {hostname} in hosts file.")
    else:
        print(f"Added new entry for {hostname} in hosts file.")

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--host", type=str, required=True)
    parser.add_argument("--ip", type=str, required=True)

    parse = parser.parse_args()
    host = parse.host
    ip = parse.ip

    update_dns_record(host, ip)
