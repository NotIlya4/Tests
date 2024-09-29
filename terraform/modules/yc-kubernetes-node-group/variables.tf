variable "name" {
  type = string
}

variable "cluster_id" {
  type = string
}

variable "labels" {
  type = map(string)
}

variable "subnet_ids" {
  type = list(string)
}

variable "zones" {
  type = list(string)
}

variable "size" {
  type = number
}

variable "resources" {
  type = object({
    cpu = number
    memory = number
  })
}

variable "boot_disk" {
  type = object({
    type = string
    size = number
  })
}