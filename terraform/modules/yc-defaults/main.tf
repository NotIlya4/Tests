locals {
  cloud = "default"
  folder = "default"
  zone_a = "ru-central1-a"
  subnet_a = "default-ru-central1-a"
  zone_b = "ru-central1-b"
  subnet_b = "default-ru-central1-b"
  zone_d = "ru-central1-d"
  subnet_d = "default-ru-central1-d"
}

data "yandex_resourcemanager_cloud" "cloud" {
  name = local.cloud
}

data "yandex_resourcemanager_folder" "folder" {
  name     = local.folder
  cloud_id = data.yandex_resourcemanager_cloud.cloud.id
}

data "yandex_vpc_subnet" "subnet_a" {
  name = local.subnet_a
  folder_id = data.yandex_resourcemanager_folder.folder.id
}

data "yandex_vpc_subnet" "subnet_b" {
  name = local.subnet_b
  folder_id = data.yandex_resourcemanager_folder.folder.id
}

data "yandex_vpc_subnet" "subnet_d" {
  name = local.subnet_d
  folder_id = data.yandex_resourcemanager_folder.folder.id
}

output "folder" {
  value = data.yandex_resourcemanager_folder.folder
}

output "zone_a" {
  value = local.zone_a
}

output "zone_b" {
  value = local.zone_b
}

output "zone_d" {
  value = local.zone_d
}

output "subnet_a" {
  value = data.yandex_vpc_subnet.subnet_a
}

output "subnet_b" {
  value = data.yandex_vpc_subnet.subnet_b
}

output "subnet_d" {
  value = data.yandex_vpc_subnet.subnet_d
}