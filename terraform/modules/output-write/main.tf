variable "value" {
  type = string
}

variable "path" {
  type = string
}

data "external" "output_path" {
  program = ["python", "../find_output_folder.py"]
}

resource "local_file" "file" {
  filename = "${data.external.output_path.result["path"]}/${var.path}"
  content = var.value
}