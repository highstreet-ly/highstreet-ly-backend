#!/bin/bash

kubectl get secret crt-ids.$DOMAIN -n $NAMESPACE -o json | jq '.data["tls.crt"]' | tr -d '"' | base64 -d  > tls.crt
kubectl get secret crt-ids.$DOMAIN -n $NAMESPACE -o json | jq '.data["tls.key"]' | tr -d '"' | base64 -d  > tls.key




sudo cp tls.crt /etc/ssl/sonaticket-certs/tls.crt
sudo cp tls.key /etc/ssl/sonaticket-certs/tls.key
