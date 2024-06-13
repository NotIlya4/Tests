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
}

provider "yandex" {
  token = data.command.yandex_token.stdout
  folder_id = local.folder_id
  zone = local.zone
}

module "node" {
    source = "../modules/node"

    name = "net1"
    folder_id = local.folder_id
    zone = local.zone
    subnet_id = local.subnet_id
    instance_resources = {
      cores = 4
      memory = 4
      disk = {
        size = 186
        type = "network-hdd"
      }
    }
}