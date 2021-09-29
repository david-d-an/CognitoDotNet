echo "Auth result by Cognito with Client Secret" > ./Log/AuthAdminResult.txt 
aws cognito-idp admin-initiate-auth --region us-east-2 --cli-input-json file://Credentials/auth_admin_ClientSecret.json >> ./Log/AuthAdminResult.txt

echo >> ./Log/AuthAdminResult.txt

echo "Auth result by Cognito without Client Secret" >> ./Log/AuthAdminResult.txt 
aws cognito-idp admin-initiate-auth --region us-east-2 --cli-input-json file://Credentials/auth_admin_No_ClientSecret.json >> ./Log/AuthAdminResult.txt
