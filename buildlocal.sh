# #!/bin/bash

# kubectl get secret crt-ids.$DOMAIN -n $NAMESPACE -o json | jq '.data["tls.crt"]' | tr -d '"' | base64 -d  > tls.crt
# kubectl get secret crt-ids.$DOMAIN -n $NAMESPACE -o json | jq '.data["tls.key"]' | tr -d '"' | base64 -d  > tls.key

# sudo cp tls.crt /etc/ssl/sonaticket-certs/tls.crt
# sudo cp tls.key /etc/ssl/sonaticket-certs/tls.key

# #docker system prune -a
# eval $(minikube docker-env)

# export SONA_BACKEND_VERSION=v1.2.27-local.74

# find . \( -name "*.config" -o -name "*.csproj" -o -name "*.sln" -o -name "NuGet.config" -o -name "*.json" \) -print0 | tar -cvf projectfiles.tar --null -T -

# docker build  -t sonatribe/sonatribeticketreservationsapi:$SONA_BACKEND_VERSION -f src/sonaticket-reservations/docker/api/Dockerfile .
# docker build  -t sonatribe/sonatribeticketreservationsprocessor:$SONA_BACKEND_VERSION -f src/sonaticket-reservations/docker/processor/Dockerfile .
# docker build  -t sonatribe/sonatribebff:$SONA_BACKEND_VERSION -f src/sonaticket-bff/docker/api/Dockerfile .
# docker build  -t sonatribe/sonatribeidentityserver:$SONA_BACKEND_VERSION -f src/sonaticket-ids/docker/api/Dockerfile .
# docker build  -t sonatribe/sonatribeticketmanagementapi:$SONA_BACKEND_VERSION -f src/sonaticket-management/docker/api/Dockerfile .
# docker build  -t sonatribe/sonatribeticketmanagementprocessor:$SONA_BACKEND_VERSION -f src/sonaticket-management/docker/processor/Dockerfile .
# docker build  -t sonatribe/sonatribepaymentsapi:$SONA_BACKEND_VERSION -f src/sonaticket-payments/docker/api/Dockerfile .
# docker build  -t sonatribe/sonatribepaymentsprocessor:$SONA_BACKEND_VERSION -f src/sonaticket-payments/docker/processor/Dockerfile .
# docker build  -t sonatribe/sonatribepermissionsapi:$SONA_BACKEND_VERSION -f src/sonaticket-permissions/docker/api/Dockerfile .
# docker build  -t sonatribe/sonatribepermissionsprocessor:$SONA_BACKEND_VERSION -f src/sonaticket-permissions/docker/processor/Dockerfile .
# docker build  -t sonatribe/sonatribescheduler:$SONA_BACKEND_VERSION -f src/sonaticket-scheduler/docker/processor/Dockerfile .
# docker build  -t sonatribe/sonatribesignalr:$SONA_BACKEND_VERSION -f src/sonaticket-signalr/docker/api/Dockerfile .

# rm projectfiles.tar




#!/bin/bash

kubectl get secret crt-ids.$DOMAIN -n $NAMESPACE -o json | jq '.data["tls.crt"]' | tr -d '"' | base64 -d  > tls.crt
kubectl get secret crt-ids.$DOMAIN -n $NAMESPACE -o json | jq '.data["tls.key"]' | tr -d '"' | base64 -d  > tls.key

sudo cp tls.crt /etc/ssl/sonaticket-certs/tls.crt
sudo cp tls.key /etc/ssl/sonaticket-certs/tls.key


export SONA_BACKEND_VERSION=v2.0.393

find . \( -name "*.config" -o -name "*.csproj" -o -name "*.sln" -o -name "NuGet.config" -o -name "*.json" \) -print0 | tar -cvf projectfiles.tar --null -T -

docker build  -t sonatribe/sonatribepaymentsapi:$SONA_BACKEND_VERSION --target payments-api .
docker build  -t sonatribe/sonatribepaymentsprocessor:$SONA_BACKEND_VERSION --target payments-processor .

docker build  -t sonatribe/sonatribeticketreservationsapi:$SONA_BACKEND_VERSION --target reservations-api .
docker build  -t sonatribe/sonatribeticketreservationsprocessor:$SONA_BACKEND_VERSION --target reservations-processor .

docker build  -t sonatribe/sonatribeticketmanagementapi:$SONA_BACKEND_VERSION --target management-api .
docker build  -t sonatribe/sonatribeticketmanagementprocessor:$SONA_BACKEND_VERSION --target management-processor .


docker build  -t sonatribe/sonatribebff:$SONA_BACKEND_VERSION --target bff .
11 
docker build  -t sonatribe/sonatribeidentityserver:$SONA_BACKEND_VERSION --target ids .

docker build  -t sonatribe/sonatribescheduler:$SONA_BACKEND_VERSION --target scheduler .

docker build  -t sonatribe/sonatribesignalr:$SONA_BACKEND_VERSION --target signalr .


rm projectfiles.tar



