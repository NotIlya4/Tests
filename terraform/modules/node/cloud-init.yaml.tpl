#cloud-config
users:
  - name: ubuntu
    gecos: User for ansible connection
    sudo: ALL=(ALL) NOPASSWD:ALL
    shell: /bin/bash
    ssh-authorized-keys:
      - %{ ssh_public }

package_update: true
package_upgrade: true
package_reboot_if_required: true