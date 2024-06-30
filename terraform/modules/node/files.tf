module "nat_ip_address" {
  source = "../output-write"

  path = "${local.path}/nat_ip_address.txt"
  value = yandex_compute_instance.instance.network_interface[0].nat_ip_address
}

module "private_ip_address" {
  source = "../output-write"

  path = "${local.path}/private_ip_address.txt"
  value = yandex_compute_instance.instance.network_interface[0].ip_address
}