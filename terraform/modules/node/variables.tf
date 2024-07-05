variable "zone" {
  type = string
}

variable "subnet_id" {
  type = string
}

variable "folder_id" {
  type = string
}

variable "name" {
  type = string
}

variable "instance_resources" {
  type = object({
    cores = number
    memory = number
    disk = object({
      type = string
      size = number
    })
  })
  default = {
    cores = 12
    memory = 12
    disk = {
      type = "network-ssd-nonreplicated"
      size = 186
    }
  }
}

variable "ssh_pub" {
  type = string
}

variable "nat" {
  type = bool
}