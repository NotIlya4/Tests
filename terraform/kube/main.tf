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
    cores = 16
    memory = 16
    disk = {
      size = 100
      type = "network-hdd"
    }
  }
}

module "node2" {
  source = "../modules/node"

  ssh_pub = module.ssh.ssh_pub

  folder_id = module.yc_defaults.folder.id
  zone = module.yc_defaults.zone_a
  subnet_id = module.yc_defaults.subnet_a.id

  name = "${local.name}-kafka1"
  nat = true

  instance_resources = {
    cores = 6
    memory = 12
    disk = {
      size = 186
      type = "network-ssd-nonreplicated"
    }
  }
}

module "node3" {
  source = "../modules/node"

  ssh_pub = module.ssh.ssh_pub

  folder_id = module.yc_defaults.folder.id
  zone = module.yc_defaults.zone_b
  subnet_id = module.yc_defaults.subnet_b.id

  name = "${local.name}-kafka2"
  nat = true

  instance_resources = {
    cores = 6
    memory = 12
    disk = {
      size = 186
      type = "network-ssd-nonreplicated"
    }
  }
}

module "node4" {
  source = "../modules/node"

  ssh_pub = module.ssh.ssh_pub

  folder_id = module.yc_defaults.folder.id
  zone = module.yc_defaults.zone_d
  subnet_id = module.yc_defaults.subnet_d.id

  name = "${local.name}-kafka3"
  nat = true

  instance_resources = {
    cores = 6
    memory = 12
    disk = {
      size = 186
      type = "network-ssd-nonreplicated"
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
        controller = true
        labels = {
          "node-role.kubernetes.io/worker" = true
          "topology.kubernetes.io/zone" = module.node1.zone
        }
      },
      {
        name = module.node2.name
        ip = module.node2.private_ip_address
        labels = {
          "node-role.kubernetes.io/kafka" = true
          "topology.kubernetes.io/zone" = module.node2.zone
        }
      },
      {
        name = module.node3.name
        ip = module.node3.private_ip_address
        labels = {
          "node-role.kubernetes.io/kafka" = true
          "topology.kubernetes.io/zone" = module.node3.zone
        }
      },
      {
        name = module.node4.name
        ip = module.node4.private_ip_address
        labels = {
          "node-role.kubernetes.io/kafka" = true
          "topology.kubernetes.io/zone" = module.node4.zone
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