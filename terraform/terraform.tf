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

provider "yandex" {
  token = data.command.yandex_token.stdout
  folder_id = var.folder_id
  zone = var.zone
}