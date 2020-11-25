New-SelfSignedCertificate -Type Custom -Container test* -Subject "CN=Dan Forsyth" -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.2","2.5.29.17={text}upn=dan@danno.ca") -KeyUsage DigitalSignature -KeyAlgorithm RSA -KeyLength 1024 -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider"


Export from certlm