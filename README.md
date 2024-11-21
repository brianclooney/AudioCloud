# AudioCloud

AudioCloud is a web application for archiving and playing back audio tracks organized into playlists. Originally designed to manage rehearsal recordings by date, AudioCloud is versatile and can be adapted for various purposes, providing \
a seamless experience for organizing and listening to audio files.

## Prerequisites

* .NET 8.0 SDK
* Docker (optional, for containerization)

## Deploy with Ansible

```
cd deploy/ansible
ansible-playbook -i inventory/example.ini deploy.yml
```
