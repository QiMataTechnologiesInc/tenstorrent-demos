# Ansible Demo

Tenstorrent Ansible demo with ansible-lint and vault support.

## Prerequisites

- Ansible 2.15+
- ansible-lint

## Installation

```bash
pip install ansible ansible-lint
```

## Usage

### Running Playbooks

```bash
ansible-playbook -i inventories/dev playbooks/demo.yml
```

### Using Vault

```bash
# Create encrypted variable
ansible-vault create group_vars/all/vault.yml

# Edit encrypted variable
ansible-vault edit group_vars/all/vault.yml

# Run with vault password
ansible-playbook -i inventories/dev playbooks/demo.yml --ask-vault-pass
```

### Linting

```bash
ansible-lint
```

## Project Structure

- `inventories/` - Inventory files for different environments
- `playbooks/` - Ansible playbooks
- `roles/` - Ansible roles
- `group_vars/` - Group variables
- `host_vars/` - Host-specific variables
- `ansible.cfg` - Ansible configuration
