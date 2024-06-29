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

module "node1" {
  source = "../modules/node"

  name = "${local.name}-node1"
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

module "lb" {
  source = "../modules/load-balancer"

  name = "${local.name}-lb"
  folder_id = local.folder_id
  zone = local.zone
  subnet_id = local.subnet_id

  target_address = module.node1.private_ip_address
  port_mappings = {
    "http" = {
      lb_port = 80
      target_port = 32160
    }
    "https" = {
      lb_port = 443
      target_port = 30643
    }
  }
}