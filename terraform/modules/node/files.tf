resource "local_file" "nat_ip_address" {
  filename = "${local.path}/nat_ip_address.txt"
  content  = yandex_compute_instance.instance.network_interface[0].nat_ip_address
}

resource "local_file" "private_ip_address" {
  filename = "${local.path}/private_ip_address.txt"
  content = yandex_compute_instance.instance.network_interface[0].ip_address
}

resource "local_file" "ssh_pub" {
  filename = "${local.path}/ssh.pub"
  content  = tls_private_key.ssh.public_key_openssh
}

resource "local_file" "ssh_key" {
  filename = "${local.path}/ssh.key"
  content  = tls_private_key.ssh.private_key_openssh
}