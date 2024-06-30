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
  name = "kube"
  path = "../../.output/${local.name}"

  node1_name = "${local.name}-node1"
  node2_name = "${local.name}-node2"
  node3_name = "${local.name}-node3"
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

  name = local.node1_name
  folder_id = module.yc_defaults.folder.id
  zone = module.yc_defaults.zone_a
  subnet_id = module.yc_defaults.subnet_a.id
  instance_resources = {
    cores = 4
    memory = 8
    disk = {
      size = 100
      type = "network-hdd"
    }
  }
}

module "node2" {
  source = "../modules/node"

  ssh_pub = module.ssh.ssh_pub

  name = local.node2_name
  folder_id = module.yc_defaults.folder.id
  zone = module.yc_defaults.zone_a
  subnet_id = module.yc_defaults.subnet_a.id
  instance_resources = {
    cores = 16
    memory = 16
    disk = {
      size = 186
      type = "network-ssd-nonreplicated"
    }
  }
}

module "node3" {
  source = "../modules/node"

  ssh_pub = module.ssh.ssh_pub

  name = local.node3_name
  folder_id = module.yc_defaults.folder.id
  zone = module.yc_defaults.zone_b
  subnet_id = module.yc_defaults.subnet_b.id
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
  folder_id = module.yc_defaults.folder.id
  zone = module.yc_defaults.zone_a
  subnet_id = module.yc_defaults.subnet_a.id

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

module "master_node" {
  source = "../modules/output-write"

  path = "${local.name}/master-node.txt"
  value = local.node1_name
}

module "worker_nodes" {
  source = "../modules/output-write"

  path = "${local.name}/worker-nodes.txt"
  value = join(",", local.node2_name, local.node3_name)
}