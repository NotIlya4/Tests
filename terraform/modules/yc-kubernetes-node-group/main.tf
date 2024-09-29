resource "yandex_kubernetes_node_group" "node_group" {
  cluster_id  = var.cluster_id
  name        = var.name

  node_labels = var.labels

  instance_template {
    network_interface {
      nat                = true
      subnet_ids         = var.subnet_ids
    }

    resources {
      cores = var.resources.cpu
      memory = var.resources.memory
    }

    boot_disk {
      type = var.boot_disk.type
      size = var.boot_disk.size
    }

    scheduling_policy {
      preemptible = true
    }
  }

  scale_policy {
    fixed_scale {
      size = var.size
    }
  }

  allocation_policy {
    dynamic "location" {
      for_each = var.zones

      content {
        zone = location.value
      }
    }
  }
}