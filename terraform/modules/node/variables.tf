variable "zone" {
  type = string
  default = "ru-central1-b"
}

variable "subnet_id" {
  type = string
  default = "e2ldnruf5jc90sjkl4io"
}

variable "folder_id" {
  type = string
  default = "b1g1go57602tvtunmuro"
}

variable "name" {
  type = string
  default = "test1"
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