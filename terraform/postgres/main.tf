data "command" "yandex_token" {
  command = ["yc", "iam", "create-token"]
}

module "postgres" {
    source = "../modules/node"

    name = var.name
    folder_id = var.folder_id
    zone = var.zone
    instance_resources = var.instance_resources
}