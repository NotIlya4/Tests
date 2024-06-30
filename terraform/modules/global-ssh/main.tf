data "external" "output_path" {
  program = ["python", "../find_output_folder.py"]
}

module "ssh_pub" {
  source = "../output-read"

  path = "ssh.pub"
}

module "ssh_key" {
  source = "../output-read"

  path = "ssh.key"
}

output "ssh_pub" {
  value = module.ssh_pub.value
}

output "ssh_key" {
  value = module.ssh_key.value
}