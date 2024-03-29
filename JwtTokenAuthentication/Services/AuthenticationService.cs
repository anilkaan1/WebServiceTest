﻿using JwtTokenAuthentication.Requests;
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
using JwtTokenAuthentication;
using System.ComponentModel.DataAnnotations;
using System.Collections;

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
                //_tokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5);

                _tokenValidationParameters.ValidateLifetime = false;///

                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters , out var validatedToken);

                _tokenValidationParameters.ValidateLifetime = true;///


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
            #region authentication
            TCPIPClientMain main = new TCPIPClientMain();
            
            JsonDocument messagejson = JsonSerializer.SerializeToDocument<UserLoginRequestModel>(reqLogin);

            _JSONconverter.JsonToXmlSet(messagejson);

            string xmlmessage = _JSONconverter.ExecuteFunction();

            main.ClientMessage = xmlmessage;

            main.StartCommunicationThread();

            string result = main.ClientMessageReturn;
            #endregion authentication



            XDocument xmldoc = XDocument.Parse(result);
            //IEnumerable<XElement> quesry = from c in xmldoc.Elements("Root").Elements("USERID") select c;

            //string UID = xmldoc.Elements("ITQXML").Elements("USERID").Select(e => e.Value).First();

            //string GROUPID = xmldoc.Elements("ITQXML").Elements("GROUPID").Select(e => e.Value).First();
            IEnumerable<ValidationResult> sonuc = JsonXmlHelper.PrepareRefreshTokenInfo(xmldoc);

            List<string> refreshtokeninfo = JsonXmlHelper.GetRefreshTokenInfo(xmldoc);

            if (!sonuc.Any())
            {
                UserLoginResponseModel model = _JWTGenerator.GenerateToken(refreshtokeninfo.ToArray(), DateTime.UtcNow, DateTime.UtcNow.AddSeconds(30));    

                JsonDocument responseJson = JsonSerializer.SerializeToDocument(model);

                XDocument refreshset = JsonXmlHelper.PrepareSetRefreshTokenData(xmldoc,responseJson.RootElement.GetProperty("Token").ToString(),responseJson.RootElement.GetProperty("RefleshToken").ToString());

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

            

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(expiryDateUnix);

            if(expiryDateTimeUtc > DateTime.UtcNow)
            {
                ObjectResult ojoj = new ObjectResult("Token Hasnt Been Expired");

                ojoj.StatusCode = 401;

                return ojoj;
            }


            string USERID = validatedToken.Claims.Single(x => x.Type == "USERID").Value;
            string SESSIONID = validatedToken.Claims.Single(x => x.Type == "SESSIONID").Value;
            string GROUPID = validatedToken.Claims.Single(x => x.Type == "GROUPID").Value;



            string requestreftoken = JsonXmlHelper.CreateAuthenticateWithRefreshTokenRequest(reqRefresh.Token,reqRefresh.RefleshToken,USERID, SESSIONID, GROUPID);

            TCPIPClientMain main = new TCPIPClientMain();

            main.ClientMessage = requestreftoken;

            main.StartCommunicationThread();

            string responsereftoken = main.ClientMessageReturn;

            XDocument responsereftokenXML = XDocument.Parse(responsereftoken);

            string responserefsuccess = responsereftokenXML.Element("ITQXML").Element("RESULT").Value;

            if (responserefsuccess == "True")
            {

                string[] infos = new string[3]{USERID,SESSIONID,GROUPID};

                UserLoginResponseModel model = _JWTGenerator.GenerateToken(infos, DateTime.UtcNow, DateTime.UtcNow.AddSeconds(30));

                JsonDocument responseJson = JsonSerializer.SerializeToDocument(model);

                XDocument refreshset = JsonXmlHelper.PrepareSetRefreshTokenData(XDocument.Parse(requestreftoken), responseJson.RootElement.GetProperty("Token").ToString(), responseJson.RootElement.GetProperty("RefleshToken").ToString());

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
            else if(responserefsuccess == "False")
            {
                ObjectResult ojoj = new ObjectResult(responsereftoken);

                ojoj.StatusCode = 401;

                return ojoj;
            }
            else
            {
                ObjectResult ojoj = new ObjectResult(responsereftoken);

                ojoj.StatusCode = 401;

                return ojoj;
            }

            //if (responserefsuccess == "True")
            //{
            //    UserLoginRequestModel model = new UserLoginRequestModel();

            //    model.USERNAME = "test";
            //    model.PASSWORDX = "Admin";

            //    return Authenticate(model);
            //}

            //if (reftokens == reqRefresh.RefleshToken)
            //{
            //    UserLoginRequestModel model = new UserLoginRequestModel();

            //    model.USERNAME = "test";
            //    model.PASSWORDX = "Admin";

            // Authenticate(model);




        }
    }
}
