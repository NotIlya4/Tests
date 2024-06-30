locals {
  name = "${var.name}-lb"
  path = var.name
}

resource "yandex_vpc_address" "lb_address" {
  name = local.name
  folder_id = var.folder_id

  external_ipv4_address {
    zone_id = var.zone
  }
}

resource "yandex_lb_target_group" "lb_group" {
  name      = var.name
  folder_id = var.folder_id

  target {
    subnet_id = var.subnet_id
    address   = var.target_address
  }
}

resource "yandex_lb_network_load_balancer" "lb" {
  name = var.name
  folder_id = var.folder_id

  dynamic "listener" {
    for_each = var.port_mappings
    content {
      name = "${local.name}-${listener.key}"
      port = listener.value.lb_port
      target_port = listener.value.target_port
      external_address_spec {
        address = yandex_vpc_address.lb_address.external_ipv4_address[0].address
      }
    }
  }

  attached_target_group {
    target_group_id = yandex_lb_target_group.lb_group.id

    healthcheck {
      name = local.name
      tcp_options {
        port = var.health_port
      }
    }
  }
}

module "lb_external_address" {
  source = "../output-write"

  path = "${local.path}/lb_external_address.txt"
  value = yandex_vpc_address.lb_address.external_ipv4_address[0].address
}