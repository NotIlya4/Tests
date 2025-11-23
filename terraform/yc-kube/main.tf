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

provider "yandex" {
  token = data.command.yandex_token.stdout
}

locals {
    name = "yc-kube"
}

module "defaults" {
  source = "../modules/yc-defaults"
}

module "kubernetes" {
  source = "../modules/yc-kubernetes"

  name = local.name
}

module "node_group" {
  source = "../modules/yc-kubernetes-node-group"

  name = local.name
  cluster_id = module.kubernetes.cluster.id

  labels = {
    "node.kubernetes.io/role" = "worker"
  }

  subnet_ids = [module.defaults.subnet_a.id, module.defaults.subnet_b.id, module.defaults.subnet_d.id]
  zones = [module.defaults.zone_a, module.defaults.zone_b, module.defaults.zone_d]

  size = 1

  resources = {
    cpu = 8
    memory = 16
  }

  boot_disk = {
    type = "network-hdd"
    size = 64
  }
}

# module "node_group_kafka" {
#   source = "../modules/yc-kubernetes-node-group"

#   name = "${local.name}-kafka"
#   cluster_id = module.kubernetes.cluster.id

#   labels = {
#     "node.kubernetes.io/role" = "kafka"
#   }

#   subnet_ids = [module.defaults.subnet_a.id, module.defaults.subnet_b.id, module.defaults.subnet_d.id]
#   zones = [module.defaults.zone_a, module.defaults.zone_b, module.defaults.zone_d]

#   size = 3

#   resources = {
#     cpu = 4
#     memory = 16
#   }

#   boot_disk = {
#     type = "network-hdd"
#     size = 64
#   }
# }