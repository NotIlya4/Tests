resource "tls_private_key" "ssh" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

module "write_output_ssh_pub" {
  source = "../modules/output-write"

  path = "ssh.pub"
  value = tls_private_key.ssh.public_key_openssh
}

module "write_output_ssh_key" {
  source = "../modules/output-write"

  path = "ssh.key"
  value = tls_private_key.ssh.private_key_openssh
}