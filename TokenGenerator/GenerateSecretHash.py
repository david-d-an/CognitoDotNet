import sys
import hmac, hashlib, base64

username = "superdavid73@hotmail.com"
app_client_id = "d2rtg9rnht9mc0u89mups2lkp"
appClientSecret = "1c5d2fpt67k5f84cotjin9kv842nbvkp12h2tpin7qqpj0ltp0qg"

message = bytes(username+app_client_id)
appClientSecret = bytes(appClientSecret)
secret_hash = base64.b64encode(hmac.new(appClientSecret, message, digestmod=hashlib.sha256).digest()).decode()

print("SECRET HASH:")
print(secret_hash)
