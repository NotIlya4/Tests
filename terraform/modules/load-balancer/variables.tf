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

variable "target_address" {
  type = string
}

variable "port_mappings" {
  type = map(object({
    lb_port = number
    target_port = number
  }))
}