#!/usr/bin/env sh

openssl genrsa -out private_key.pem 2048

openssl req -new -x509 -key private_key.pem -out auth_verifying.pem -days $((365 * 5)) \
  -subj "/CN=LassoAuth"
  
openssl pkcs12 -export -out auth_signing.pfx -inkey private_key.pem -in auth_verifying.pem

rm private_key.pem