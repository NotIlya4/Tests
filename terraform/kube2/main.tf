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
  name = "kube2"
  path = "../../.output/${local.name}"
}

provider "yandex" {
  token = data.command.yandex_token.stdout
}

module "yc_defaults" {
  source = "../modules/yc-defaults"
}

module "ssh" {
  source = "../modules/global-ssh"
}

module "node1" {
  source = "../modules/node"

  ssh_pub = module.ssh.ssh_pub
  folder_id = module.yc_defaults.folder.id
  zone = module.yc_defaults.zone_b
  subnet_id = module.yc_defaults.subnet_b.id

  name = "${local.name}-worker1"
  nat = true

  instance_resources = {
    cores = 8
    memory = 8
    disk = {
      size = 100
      type = "network-hdd"
    }
  }
}

module "lb" {
  source = "../modules/load-balancer"

  name = local.name
  folder_id = module.yc_defaults.folder.id
  zone = module.node1.zone
  subnet_id = module.node1.subnet_id

  target_address = module.node1.private_ip_address
  health_port = 6443
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

module "worker_nodes" {
  source = "../modules/output-write"

  path = "${local.name}/workers.yaml"
  value = yamlencode({
    workers = [
      {
        name = module.node1.name
        ip = module.node1.private_ip_address
        labels = {
          "node-role.kubernetes.io/worker" = "true"
          "topology.kubernetes.io/zone" = module.node1.zone
        }
      },
    ]
  })
}

module "cluster_ip"{
  source = "../modules/output-write"

  path = "${local.name}/cluster-ip.txt"
  value = module.node1.nat_ip_address
}