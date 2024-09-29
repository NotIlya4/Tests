module "defaults" {
  source = "../yc-defaults"
}

resource "yandex_iam_service_account" "sa" {
  name        = "${var.name}-cluster"
  folder_id = module.defaults.folder.id
}

resource "yandex_resourcemanager_folder_iam_member" "sa_admin" {
  folder_id = module.defaults.folder.id
  role               = "admin"
  member             = "serviceAccount:${yandex_iam_service_account.sa.id}"
}

resource "yandex_kubernetes_cluster" "kubernetes" {
  name        = var.name
  folder_id = module.defaults.folder.id

  network_id = module.defaults.subnet_b.network_id

  master {
    version = "1.30"
    zonal {
      zone      = module.defaults.zone_b
      subnet_id = module.defaults.subnet_b.id
    }

    public_ip = true
  }

  service_account_id      = "${yandex_iam_service_account.sa.id}"
  node_service_account_id = "${yandex_iam_service_account.sa.id}"

  release_channel = "RAPID"

  depends_on = [ yandex_resourcemanager_folder_iam_member.sa_admin ]
}

output "cluster" {
  value = yandex_kubernetes_cluster.kubernetes
}