ips_file = "../terraform/output/nat_ip_address.txt"
inventory_file = "inventory.ini"

with open(ips_file, "r") as f:
    ips = f.read().strip().split(",")

with open(inventory_file, "w") as f:
    f.write("[nodes]\n")
    for ip in ips:
        f.write(f"{ip} ansible_user=ubuntu ansible_ssh_private_key_file=../terraform/output/ssh.key\n")
