#!/bin/bash
# 

kubectl get secret crt-ids.$DOMAIN -n $NAMESPACE -o json | jq '.data["tls.crt"]' | tr -d '"' | base64 -d  > tls.crt
kubectl get secret crt-ids.$DOMAIN -n $NAMESPACE -o json | jq '.data["tls.key"]' | tr -d '"' | base64 -d  > tls.key

sudo cp tls.crt /etc/ssl/sonaticket-certs/tls.crt
sudo cp tls.key /etc/ssl/sonaticket-certs/tls.key

#docker system prune -a

export SONA_BACKEND_VERSION=v1.2.27-local.94

find . \( -name "*.config" -o -name "*.csproj" -o -name "*.sln" -o -name "NuGet.config" -o -name "*.json" \) -print0 | tar -cvf projectfiles.tar --null -T -

# docker build  -t sonatribe/permissionsapi:$SONA_BACKEND_VERSION --target permissions-api -f ./DockerfileV2 .
# docker build  -t sonatribe/permissionsprocessor:$SONA_BACKEND_VERSION --target permissions-processor -f ./DockerfileV2 .
# docker build  -t sonatribe/paymentsapi:$SONA_BACKEND_VERSION --target payments-api -f ./DockerfileV2 .
# docker build  -t sonatribe/paymentsprocessor:$SONA_BACKEND_VERSION --target payments-processor -f ./DockerfileV2 .
docker build  -t sonatribe/managementapi:$SONA_BACKEND_VERSION --target management-api -f ./DockerfileV2 .
docker build  -t sonatribe/managementprocessor:$SONA_BACKEND_VERSION --target management-processor -f ./DockerfileV2 .
# docker build  -t sonatribe/reservationsapi:$SONA_BACKEND_VERSION --target reservations-api -f ./DockerfileV2 .
# docker build  -t sonatribe/reservationsprocessor:$SONA_BACKEND_VERSION --target reservations-processor -f ./DockerfileV2 .
# docker build  -t sonatribe/identityserver:$SONA_BACKEND_VERSION --target ids -f ./DockerfileV2 .
# docker build  -t sonatribe/bff:$SONA_BACKEND_VERSION --target bff -f ./DockerfileV2 .
# docker build  -t sonatribe/signalr:$SONA_BACKEND_VERSION --target signalr -f ./DockerfileV2 .
# docker build  -t sonatribe/scheduler:$SONA_BACKEND_VERSION --target scheduler -f ./DockerfileV2 .
# 
# kind load docker-image sonatribe/permissionsapi:$SONA_BACKEND_VERSION
# kind load docker-image sonatribe/permissionsprocessor:$SONA_BACKEND_VERSION
# kind load docker-image sonatribe/paymentsapi:$SONA_BACKEND_VERSION
# kind load docker-image sonatribe/paymentsprocessor:$SONA_BACKEND_VERSION
kind load docker-image sonatribe/managementapi:$SONA_BACKEND_VERSION
kind load docker-image sonatribe/managementprocessor:$SONA_BACKEND_VERSION
# kind load docker-image sonatribe/reservationsapi:$SONA_BACKEND_VERSION
# kind load docker-image sonatribe/reservationsprocessor:$SONA_BACKEND_VERSION
# kind load docker-image sonatribe/identityserver:$SONA_BACKEND_VERSION
# kind load docker-image sonatribe/bff:$SONA_BACKEND_VERSION
# kind load docker-image sonatribe/signalr:$SONA_BACKEND_VERSION
# kind load docker-image sonatribe/scheduler:$SONA_BACKEND_VERSION


rm projectfiles.tar
