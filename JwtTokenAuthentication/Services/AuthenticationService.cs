using JwtTokenAuthentication.Requests;
using JwtTokenAuthentication.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TCPIP_ServerClient;
using WebServiceTest.Data;
using System.Xml.Linq;
using WebServiceTest.Converters;

namespace JwtTokenAuthentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private MyJsonToXmlConverter _JSONconverter { get; set; }
        private IJwtTokenGenerator _JWTGenerator { get; set; }
        private MyXmlToJsonConverter _XMLconverter { get; set; }

        private  TokenValidationParameters _tokenValidationParameters;

        readonly JwtDescriptor _jwtKey;
        public AuthenticationService(IOptions<JwtDescriptor> myDescriptor, IJwtTokenGenerator JWTGenerator, MyXmlToJsonConverter mxljson, MyJsonToXmlConverter jsonxml, TokenValidationParameters tokenValidationParameters)
        {
            _jwtKey = myDescriptor.Value;
            _JSONconverter = jsonxml;
            _XMLconverter = mxljson;
            _JWTGenerator = JWTGenerator;
            _tokenValidationParameters = tokenValidationParameters;
        }
        private bool IsJwtValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256,StringComparison.InvariantCultureIgnoreCase);
        }
        private ClaimsPrincipal GetPrincipleFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                //using RSA rsa = RSA.Create();
                //rsa.ImportSubjectPublicKeyInfo(_jwtKey.RsaPublicKey.ToByteArray(), out _);

                //_tokenValidationParameters.IssuerSigningKey = new RsaSecurityKey(rsa);

                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters , out var validatedToken);

                if (!IsJwtValidSecurityAlgorithm(validatedToken))
                {
                    string c = tokenHandler.WriteToken(validatedToken);
                    return null;
                }
                else
                {
                    return principal;
                }
            }
            catch
            {
                return null;
            }

        }



        public ObjectResult Authenticate(UserLoginRequestModel reqLogin)
        {

            TCPIPClientMain main = new TCPIPClientMain();

            //main.ClientMessage = reqLogin.UID + ',' + reqLogin.PASSWORD;

            JsonDocument messagejson = JsonSerializer.SerializeToDocument<UserLoginRequestModel>(reqLogin);

            _JSONconverter.JsonToXmlSet(messagejson/*,session*/);

            string xmlmessage = _JSONconverter.ExecuteFunction();

            main.ClientMessage = xmlmessage;

            main.ClientCommand = 'J';

            main.StartCommunicationThread();

            string result = main.ClientMessageReturn;


            XDocument xmldoc = XDocument.Parse(result);

            IEnumerable<XElement> quesry = from c in xmldoc.Elements("Root").Elements("USERID") select c;

            //string UID = xmldoc.Elements("ITQXML").Elements("USERID").Select(e => e.Value).First();

            //string GROUPID = xmldoc.Elements("ITQXML").Elements("GROUPID").Select(e => e.Value).First();

            

            string UID = xmldoc.Element("ITQXML").Element("USERID").Value;

            string SessionID = xmldoc.Element("ITQXML").Element("SESSIONID").Value;

            string GROUPID = xmldoc.Element("ITQXML").Element("GROUPID").Value;
            //ObjectResult ojojo = new ObjectResult(result);
            //ojojo.StatusCode = 200;
            //return ojojo;

            


            string result2 = "Authorized";

            if (result2 == "Authorized")
            {

                UserLoginResponseModel model = _JWTGenerator.GenerateToken(new List<string> {UID,SessionID,GROUPID}.ToArray(), DateTime.Now, DateTime.Now.AddSeconds(150),reqLogin.USERNAME);

                //string b = _JWTGenerator.GenerateRefleshToken();

                UserLoginResponseModel response = model;
                //"{\"Token\":\""+ a +"\"}";            

                JsonDocument responseJson = JsonSerializer.SerializeToDocument(response);


                XDocument refreshset = JsonXmlHelper.PrepareSetRefreshTokenData(UID, GROUPID, SessionID,responseJson.RootElement.GetProperty("Token").ToString(),responseJson.RootElement.GetProperty("RefleshToken").ToString());

                string refreshsetstr = JsonXmlHelper.XDocumentToString(refreshset);

                main.ClientMessage = refreshsetstr;

                main.StartCommunicationThread();

                string result3 = main.ClientMessageReturn;

                if (main.Client != null)
                    main.StopCommunication();


                ObjectResult ojoj = new ObjectResult(responseJson);
                ojoj.StatusCode = 200;

                return ojoj;
            }

            else
            {
                ObjectResult ojoj = new ObjectResult("Wrong Username/password");
                ojoj.StatusCode = 401;
                return ojoj;
            }
        }

        public ObjectResult RefreshToken(RefreshTokenRequestModel reqRefresh)
        {
            var validatedToken = GetPrincipleFromToken(reqRefresh.Token);

            if(validatedToken == null)
            {
                ObjectResult ojoj = new ObjectResult("WrongToken");

                ojoj.StatusCode = 401;

                return ojoj;
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            string tokenjti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;


            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(expiryDateUnix);

            if(expiryDateTimeUtc > DateTime.UtcNow)
            {
                ObjectResult ojoj = new ObjectResult("Token Hasnt Been Expired");

                ojoj.StatusCode = 401;

                return ojoj;
            }




            string reftokens = "";   //DBden alınacak...  //Şimdilik elle alıyoruz.
            string refjti = "";

            if (refjti == tokenjti)
            {
                UserLoginRequestModel model = new UserLoginRequestModel();

                model.USERNAME = "test";
                model.PASSWORDX = "Admin";

                return Authenticate(model);
            }

            if (reftokens == reqRefresh.RefleshToken)
            {
                UserLoginRequestModel model = new UserLoginRequestModel();

                model.USERNAME = "test";
                model.PASSWORDX = "Admin";

                return Authenticate(model);
            }
            
            else
            {
                ObjectResult ojoj = new ObjectResult("RefleshToken Bulunamadı.");

                ojoj.StatusCode = 401;

                return ojoj;
            }

        }
    }
}
