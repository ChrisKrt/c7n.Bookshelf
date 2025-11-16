---
applyTo: "docker-compose.yml,*.docker-compose.yml,Dockerfile"
---

# Docker
- use docker-compose
- images are not simple microservices, they are self contained applications
- use domain naming for services / container 
- use domain naming for images
- use domain naming for volumes
- use domain naming for networks
- use multistage builds to reduce image size
- use always https / tls for communication between services
- never use latest tags for images, always use specific version tags