terraform {
  required_providers {
    command = {
      source = "hkak03key/command"
      version = "0.1.1"
    }
    yandex = {
      source = "yandex-cloud/yandex"
      version = "0.115.0"
    }
  }
}

data "command" "yandex_token" {
  command = ["yc", "iam", "create-token"]
}

locals {
  folder_id = "b1g1go57602tvtunmuro"
  zone = "ru-central1-b"
  subnet_id = "e2ldnruf5jc90sjkl4io"
  name = "kube"
  path = "../../.output/${local.name}"
}

provider "yandex" {
  token = data.command.yandex_token.stdout
  folder_id = local.folder_id
  zone = local.zone
}

module "node" {
    source = "../modules/node"

    name = local.name
    folder_id = local.folder_id
    zone = local.zone
    subnet_id = local.subnet_id
    instance_resources = {
      cores = 16
      memory = 16
      disk = {
        size = 186
        type = "network-ssd-nonreplicated"
      }
    }
}

resource "yandex_vpc_address" "lb_address" {
  name = "${local.name}-lb"

  external_ipv4_address {
    zone_id = local.zone
  }
}

resource "yandex_lb_target_group" "lb_group" {
  name      = local.name

  target {
    subnet_id = local.subnet_id
    address   = module.node.yandex_compute_instance.network_interface.0.ip_address
  }
}

resource "yandex_lb_network_load_balancer" "lb" {
  name = local.name

  listener {
    name = "${local.name}-http"
    port = 80
    target_port = 32160
    external_address_spec {
      address = yandex_vpc_address.lb_address.external_ipv4_address[0].address
    }
  }

  listener {
    name = "${local.name}-https"
    port = 443
    target_port = 30643
    external_address_spec {
      address = yandex_vpc_address.lb_address.external_ipv4_address[0].address
    }
  }

  attached_target_group {
    target_group_id = yandex_lb_target_group.lb_group.id

    healthcheck {
      name = local.name
      tcp_options {
        port = 32160
      }
    }
  }
}

resource "local_file" "lb_external_address" {
  filename = "${local.path}/lb_external_address.txt"
  content  = yandex_vpc_address.lb_address.external_ipv4_address[0].address
}