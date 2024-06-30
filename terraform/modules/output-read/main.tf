variable "path" {
  type = string
}

data "external" "output_path" {
  program = ["python", "../find_output_folder.py"]
}

data "local_file" "file" {
  filename = "${data.external.output_path.result["path"]}/${var.path}"
}

output "value" {
  value = data.local_file.file.content
}