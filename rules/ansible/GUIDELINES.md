# Ansible Guidelines

This document provides guidelines for Ansible projects in the Tenstorrent demo monorepo.

## Standards

- **Ansible Version**: 2.15+ (minimum)
- **Python Version**: 3.10+ (for Ansible execution)
- **Linter**: ansible-lint (production profile)
- **Security**: Ansible Vault for secrets

## Project Structure

```
ansible/
├── ansible.cfg
├── .ansible-lint
├── inventories/
│   ├── dev/
│   │   └── hosts.yml
│   ├── staging/
│   │   └── hosts.yml
│   └── prod/
│       └── hosts.yml
├── playbooks/
│   └── playbook.yml
├── roles/
│   └── role_name/
│       ├── tasks/
│       ├── handlers/
│       ├── templates/
│       ├── files/
│       ├── vars/
│       ├── defaults/
│       └── meta/
├── group_vars/
│   ├── all.yml
│   └── all/
│       └── vault.yml
└── host_vars/
```

## Naming Conventions

- **Playbooks**: descriptive-name.yml
- **Roles**: role_name (snake_case)
- **Variables**: snake_case
- **Tasks**: Use descriptive names with proper casing

## ansible-lint Configuration

Use production profile for strict checks:
```yaml
---
profile: production

skip_list: []
warn_list: []
enable_list:
  - yaml
```

## Playbook Best Practices

1. Use fully qualified collection names (FQCN)
2. Always include `name` for tasks
3. Use `ansible.builtin` for core modules
4. Set `gather_facts` explicitly
5. Use `become` sparingly and explicitly

## Example Playbook

```yaml
---
- name: Configure web servers
  hosts: webservers
  gather_facts: true
  become: true

  vars:
    app_port: 8080

  tasks:
    - name: Install nginx
      ansible.builtin.package:
        name: nginx
        state: present

    - name: Start nginx service
      ansible.builtin.service:
        name: nginx
        state: started
        enabled: true
```

## Vault Usage

Encrypt sensitive data:
```bash
# Create encrypted file
ansible-vault create group_vars/all/vault.yml

# Edit encrypted file
ansible-vault edit group_vars/all/vault.yml

# Use in playbook
ansible-playbook -i inventory playbook.yml --ask-vault-pass
```

Variable naming convention for vault:
```yaml
# vault.yml (encrypted)
vault_db_password: "secret123"

# vars.yml (plaintext reference)
db_password: "{{ vault_db_password }}"
```

## Inventory Structure

Use YAML format for inventories:
```yaml
---
all:
  children:
    webservers:
      hosts:
        web01:
          ansible_host: 192.168.1.10
        web02:
          ansible_host: 192.168.1.11
    databases:
      hosts:
        db01:
          ansible_host: 192.168.1.20
```

## Role Structure

Standard role layout:
```
role_name/
├── README.md
├── defaults/
│   └── main.yml
├── files/
├── handlers/
│   └── main.yml
├── meta/
│   └── main.yml
├── tasks/
│   └── main.yml
├── templates/
└── vars/
    └── main.yml
```

## Testing

- **Framework**: Molecule
- **Driver**: Docker or Vagrant
- **Linting**: Always pass ansible-lint before commit
- **Idempotency**: All playbooks must be idempotent

## ansible.cfg

```ini
[defaults]
inventory = inventories/dev
roles_path = roles
host_key_checking = False
retry_files_enabled = False

[privilege_escalation]
become = True
become_method = sudo
become_user = root
```
