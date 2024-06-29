locals {
  path = "../../.output/${var.name}"
}

resource "tls_private_key" "ssh" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

resource "local_file" "ssh_pub" {
  filename = "${local.path}/ssh.pub"
  content  = tls_private_key.ssh.public_key_openssh
}

resource "local_file" "ssh_key" {
  filename = "${local.path}/ssh.key"
  content  = tls_private_key.ssh.private_key_openssh
}

data "yandex_vpc_subnet" "subnet" {
  subnet_id = var.subnet_id
}

data "yandex_compute_image" "ubuntu" {
  family = "ubuntu-2204-lts-oslogin"
}

resource "yandex_vpc_address" "address" {
  name = var.name

  external_ipv4_address {
    zone_id = var.zone
  }
}

resource "yandex_compute_instance" "instance" {
  name                      = "${var.name}"
  allow_stopping_for_update = true
  hostname = "${var.name}"

  resources {
    cores  = var.instance_resources.cores
    memory = var.instance_resources.memory
  }

  boot_disk {
    auto_delete = true
    initialize_params {
      size     = var.instance_resources.disk.size
      type     = var.instance_resources.disk.type
      image_id = data.yandex_compute_image.ubuntu.id
    }
  }

  network_interface {
    subnet_id      = data.yandex_vpc_subnet.subnet.id
    nat            = true
    nat_ip_address = yandex_vpc_address.address.external_ipv4_address[0].address
  }

  scheduling_policy {
    preemptible = true
  }

  metadata = {
    ssh-keys = "ubuntu:${tls_private_key.ssh.public_key_openssh}"
  }
}

resource "local_file" "nat_ip_address" {
  filename = "${local.path}/nat_ip_address.txt"
  content  = yandex_compute_instance.instance.network_interface[0].nat_ip_address
}

output "yandex_compute_instance" {
  value = yandex_compute_instance.instance
}

output "nat_ip_address" {
  value = yandex_compute_instance.instance.network_interface[0].nat_ip_address
}