data "command" "yandex_token" {
  command = ["yc", "iam", "create-token"]
}

resource "tls_private_key" "ssh" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

resource "local_file" "ssh_pub" {
  filename = "./output/ssh.pub"
  content  = tls_private_key.ssh.public_key_openssh
}

resource "local_file" "ssh_key" {
  filename = "./output/ssh.key"
  content  = tls_private_key.ssh.private_key_openssh
}

resource "yandex_vpc_network" "network" {
  name = "${var.name}-network"
}

resource "yandex_vpc_subnet" "subnet" {
  v4_cidr_blocks = ["10.2.0.0/16"]
  network_id     = yandex_vpc_network.network.id
}

data "yandex_compute_image" "ubuntu" {
  family = "ubuntu-2204-lts-oslogin"
}

resource "yandex_compute_instance" "instance" {
  name = "${var.name}-instance"

  resources {
    cores  = var.instance_resources.cores
    memory = var.instance_resources.memory
  }

  boot_disk {
    auto_delete = true
    initialize_params {
      size     = 186
      type     = "network-ssd-nonreplicated"
      image_id = data.yandex_compute_image.ubuntu.id
    }
  }

  network_interface {
    subnet_id = yandex_vpc_subnet.subnet.id
    nat       = true
  }

  scheduling_policy {
    preemptible = true
  }

  metadata = {
    ssh-keys = "ubuntu:${tls_private_key.ssh.public_key_openssh}"
  }
}

resource "local_file" "instance_stdout" {
  filename = "./output/instance.json"
  content  = jsonencode(yandex_compute_instance.instance)
}

resource "local_file" "nat_ip_address" {
  filename = "./output/nat_ip_address.txt"
  content  = yandex_compute_instance.instance.network_interface[0].nat_ip_address
}
