locals {
  name = "${var.name}-node"
  path = var.name
}

data "yandex_compute_image" "ubuntu" {
  family = "ubuntu-2204-lts"
}

resource "yandex_vpc_address" "address" {
  name = local.name
  folder_id = var.folder_id

  count = var.nat ? 1 : 0

  external_ipv4_address {
    zone_id = var.zone
  }
}

resource "yandex_compute_instance" "instance" {
  name                      = var.name
  folder_id = var.folder_id
  zone = var.zone
  platform_id = "standard-v3"

  allow_stopping_for_update = true
  hostname = local.name

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
    subnet_id      = var.subnet_id
    nat            = var.nat
    nat_ip_address = var.nat ? yandex_vpc_address.address[0].external_ipv4_address[0].address : null
  }

  scheduling_policy {
    preemptible = true
  }

  metadata = {
    user-data = <<-EOT
    #cloud-config
    users:
      - name: ubuntu
        gecos: User for ansible connection
        sudo: ALL=(ALL) NOPASSWD:ALL
        shell: /bin/bash
        ssh-authorized-keys:
          - ${var.ssh_pub}

    package_update: true
    package_upgrade: true
    package_reboot_if_required: true
    EOT
  }
}

output "yandex_compute_instance" {
  value = yandex_compute_instance.instance
}

output "name" {
  value = var.name
}

output "zone" {
  value = var.zone
}

output "subnet_id" {
  value = var.subnet_id
}

output "private_ip_address" {
  value = yandex_compute_instance.instance.network_interface[0].ip_address
}

output "nat_ip_address" {
  value = yandex_compute_instance.instance.network_interface[0].nat_ip_address
}