variable "zone" {
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
  })
}