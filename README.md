# PKCS11Cloud Web_CA

The PKCS11Cloud Web Certificate Authority is the Cloud environment for the PKCS11Cloud Driver. 
The repo contains the following modules:
  1. PKCS11Cloud Web Application
  2. PKCS11Cloud Web Service
  
## PKCS11Cloud Web Application
This module is the user inteface used for requesting cryptographic objects (digital certificates and cryptographic keys).
These are generated and stored in Cloud, the user can acces them through the PKCS11Cloud Driver.
###### Features
- It has configurated an Certificate Authority user for signing the user's digital certificates.

  
## PKCS11Cloud Web Service
  
This module is the intermediate between the PKCS11Cloud Driver and the cryptographic objects stored in Cloud. Each request for using the user's cryptographic objects is proccesed by the Web Service.
