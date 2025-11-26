# Tenstorrent Ansible Roles

This directory provides three Ansible roles you can use together or independently to prepare a host for Tenstorrent accelerators, install the inference stack, and verify that required models are present. Use the bundled `site.yml` to run all three in sequence or include the roles selectively in your own playbooks.

## Prerequisites

1. Install Ansible on the control machine (e.g., `pip install ansible-core`).
2. Install the required collections before running the playbooks:

   ```bash
   ansible-galaxy collection install -r collections/requirements.yml
   ```
3. Ensure you can connect to the target hosts with sufficient privileges. The `tt_drivers` and `tt_inference_stack` roles expect `become: yes` because they modify system packages and configuration.

## Inventory and playbook examples

Create an inventory that defines the hosts you want to target and any variable overrides. For example:

```ini
[tenstorrent_hosts]
host1 ansible_host=10.0.0.10
host2 ansible_host=10.0.0.11

[tenstorrent_hosts:vars]
ansible_user=ubuntu
ansible_become=true
tenstorrent_card_model=blackhole
```

To run all roles in order (drivers ➜ inference stack ➜ model check), use the bundled playbook:

```bash
ansible-playbook -i inventory site.yml
```

If you prefer to run a single role, create a short playbook with just that role:

```yaml
# drivers-only.yml
- hosts: tenstorrent_hosts
  become: yes
  roles:
    - tt_drivers
```

Then run it with your inventory:

```bash
ansible-playbook -i inventory drivers-only.yml
```

## Role: `tt_drivers`

Installs OS prerequisites, Tenstorrent kernel drivers, firmware, system tools, and optional topology helpers, then validates device visibility with `tt-smi`. Key defaults:

- Kernel driver version and repository: `tenstorrent_kmd_version` (`2.6.1`) and `tenstorrent_kmd_repo` (`https://github.com/tenstorrent/tt-kmd.git`).
- Firmware bundle: `tenstorrent_fw_version` (`19.02.0`) with the corresponding `tenstorrent_fw_url`.
- System tools package: `tenstorrent_tools_pkg_url` pointing to the Debian package that provides hugepages setup and `tt-smi`.
- Optional topology helper: `tenstorrent_install_topology_util` (default `false`) and `tenstorrent_topology_repo`.
- Utilities: `tt_flash_repo` (for flashing firmware) and `tt_smi_repo`.
- Reboot control: `tt_reboot_after_firmware` (default `true`).

Example usage within a playbook:

```yaml
- hosts: tenstorrent_hosts
  become: yes
  roles:
    - role: tt_drivers
      vars:
        tenstorrent_kmd_version: "2.6.1"
        tenstorrent_fw_version: "19.02.0"
        tenstorrent_install_topology_util: true
        tt_reboot_after_firmware: true
```

The role asserts Debian/Ubuntu or RedHat/Fedora families, installs the DKMS driver, flashes firmware with `tt-flash`, enables hugepages on Debian-based systems, optionally configures mesh topology, and surfaces `tt-smi` output for quick validation.

## Role: `tt_inference_stack`

Installs the Tenstorrent Python stack (TTNN) and supporting repository, applies optional environment variables, and tunes CPU governor settings. Key defaults:

- Card model for runtime: `tenstorrent_card_model` (`blackhole`).
- TTNN package selection: `ttnn_package` (`ttnn`) with an optional pinned `ttnn_version` (`0.64.4`).
- Example repository: `tt_metal_repo` (`https://github.com/tenstorrent/tt-metal.git`) and version (`main`) cloned to `tt_metal_install_dir` (`/opt/tt-metal`).
- Extra requirements installation toggle and path: `tt_install_extra_requirements` (`true`) and `tt_requirements_rel_path` (`python_env/requirements-dev.txt`).
- Environment persistence toggle: `tt_set_system_env` (`true`) to write `ARCH_NAME`, `TT_METAL_HOME`, and `PYTHONPATH` into `/etc/environment`.

Example playbook snippet to pin TTNN and skip extra requirements:

```yaml
- hosts: tenstorrent_hosts
  become: yes
  roles:
    - role: tt_inference_stack
      vars:
        ttnn_version: "0.64.4"
        tt_install_extra_requirements: false
        tt_set_system_env: true
```

The role validates the OS family, installs TTNN via `pip3`, clones TT-Metal, optionally installs model/demo dependencies when the requirements file exists, updates system environment variables, sets the CPU governor to `performance` when supported, and runs a Python import check to confirm `ttnn` is usable.

## Role: `tt_model_check`

Verifies that required model directories or files exist under a configurable root. Key defaults:

- Model root directory: `tt_model_root_dir` (`/opt/tenstorrent/models`).
- Expected items list: `tt_models_expected` (`Llama-70B`, `Falcon-7B`, `ResNet50`).

Example usage to validate a custom model set:

```yaml
- hosts: tenstorrent_hosts
  roles:
    - role: tt_model_check
      vars:
        tt_model_root_dir: "/mnt/models"
        tt_models_expected:
          - llama-70b
          - falcon-7b
          - resnet50
```

The role runs `stat` against each expected path, fails clearly when any entry is missing, and reports which models are present.

## Troubleshooting tips

- Ensure kernel headers match the running kernel when DKMS builds the Tenstorrent driver.
- The provided system tools package URL targets Debian-based hosts; supply an OS-specific package for RedHat/Fedora if needed.
- `cpupower` may be unavailable or restricted in virtualized environments; the role will not fail if the governor cannot be set but will still report stdout.
- When installing from git repositories, ensure outbound network access or pre-stage the repositories in an internal mirror and override the URLs accordingly.
