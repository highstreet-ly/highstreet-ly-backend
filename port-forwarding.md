# local

kubectl port-forward services/sonatribe-rabbitmq 5672:5672 -n highstreetly-live
kubectl port-forward services/sonatribe-pgsql-postgresql 5432:5432 -n highstreetly-live
kubectl port-forward services/hashicorp-consul-consul-server 8500:8500 -n hashicorp-consul   
kubectl port-forward services/app-elasticsearch-es-http 9200:9200 -n highstreetly-live


# test

kubectl port-forward services/sonatribe-rabbitmq 5672:5672 -n sonashop-xyz
kubectl port-forward services/sonatribe-pgsql-postgresql 5432:5432 -n sonashop-xyz
kubectl port-forward services/hashicorp-consul-consul-server 8500:8500 -n hashicorp-consul   
kubectl port-forward services/app-elasticsearch-es-http 9200:9200 -n sonashop-xyz