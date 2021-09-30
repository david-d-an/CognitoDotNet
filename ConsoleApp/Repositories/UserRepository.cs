using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using ConsoleApp.Model;

namespace ConsoleApp.Repositories {
    public class UserRepository : IUserRepository {
        public async Task<AuthResponseModel> TryLoginAsync(
            UserLoginModel model,
            AppConfigModel config
        ) {
            var provider = new AmazonCognitoIdentityProviderClient(
                                new AnonymousAWSCredentials(),
                                RegionEndpoint.GetBySystemName(config.Region));

            var userPool = new CognitoUserPool(
                                config.UserPoolId,
                                config.AppClientId,
                                provider);

            var user = new CognitoUser(
                            model.username,
                            config.AppClientId,
                            userPool,
                            provider);

            var authRequest = new InitiateSrpAuthRequest {
                Password = model.password
            };

            try
            {
                var authResponse = await user.StartWithSrpAuthAsync(authRequest);
                var result = authResponse.AuthenticationResult;

                var authResponseModel = new AuthResponseModel {
                    EmailAddress = user.UserID,
                    UserId = user.Username,
                    Tokens = new TokenModel {
                        IdToken = result.IdToken,
                        AccessToken = result.AccessToken,
                        ExpiresIn = result.ExpiresIn,
                        RefreshToken = result.RefreshToken
                    },
                    IsSuccess = true
                };
                return authResponseModel;
            }
            catch (UserNotConfirmedException) {
                // Occurs if the User has signed up 
                // but has not confirmed his EmailAddress
                // In this block we try sending 
                // the Confirmation Code again and ask user to confirm
                return new AuthResponseModel {
                    IsSuccess = false,
                    Message = "User has not been confirmed."
                };
            }
            catch (UserNotFoundException) {
                // Occurs if the provided emailAddress 
                // doesn't exist in the UserPool
                return new AuthResponseModel {
                    IsSuccess = false,
                    Message = "EmailAddress not found."
                };
            }
            catch (NotAuthorizedException) {
                return new AuthResponseModel {
                    IsSuccess = false,
                    Message = "Incorrect username or password"
                };
            }
        }
    }
}
