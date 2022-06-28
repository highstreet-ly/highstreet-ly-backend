# Highstreet.ly backend monorepo

This is a reference implementation of a deliveroo "like" system. The system comprises of multiple parts:

  - Backend services
  - Dashboard management UI
  - Operator app
  - Marketing site
  - Pulumi IaC

This code was actually the v1 of a startup myself and a friend tried to bootstrap which unfortunately failed. Rather than keep the code private and eventually delete it I am making it public so that it can act as an extension to my CV and maybe help others interested in K8s, Pulumi, ES/CQRS etc.

While there are tests for the backend code, there isn't anywhere near as many as I would like but this was code that lived somewhere between POC and Beta - the aim was to use it to show the concept and get funding. So please bare in mind this is not what I would consider production code.

The system ran on 3 x DL380 G10 servers in a datacenter in Brighton which I hosted behind a SonicWALL TZ250/SOHO firewall. I implemented the k8s cluster on an array of ESXi VM hosts running Ubuntu. 

The k8s cluster was initialized using Ansible / Kubespray and then the infrastructure was installed into that using Pulumi. 
