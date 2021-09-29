echo "Auth result by Cognito with Client Secret" > ./Log/AuthUserResult.txt
aws cognito-idp initiate-auth --region us-east-2 --cli-input-json file://Credentials/auth_user_ClientSecret.json >> ./Log/AuthUserResult.txt

echo >> ./Log/AuthUserResult.txt

echo "Auth result by Cognito without Client Secret" >> ./Log/AuthUserResult.txt
aws cognito-idp initiate-auth --region us-east-2 --cli-input-json file://Credentials/auth_user_No_ClientSecret.json >> ./Log/AuthUserResult.txt
