using System;
using AbakHelper.XeroIntegration.UI;
using Xero.Api.Example.Applications;
using Xero.Api.Infrastructure.Interfaces;
using Xero.Api.Infrastructure.OAuth.Signing;

namespace AbakHelper.XeroIntegration
{
    public class PublicAuthenticator : TokenStoreAuthenticator
    {
        private readonly string _scope;
        private readonly bool _redirectOnError;

        public PublicAuthenticator(string baseUri, string tokenUri, string callBackUrl, ITokenStore store, string scope = null, bool redirectOnError = false)
            : base(baseUri, tokenUri, callBackUrl, store)
        {
            _scope = scope;
            _redirectOnError = redirectOnError;
        }

        protected override string AuthorizeUser(IToken token)
        {
            var authorizeUrl = GetAuthorizeUrl(token, _scope, _redirectOnError);
            OAuthWindow window = new OAuthWindow(authorizeUrl);
            if (window.ShowDialog() ?? false)
            {
                return window.Pin;
            }
            return null;
            //Process.Start(authorizeUrl);

            //Console.WriteLine("Enter the PIN given on the web page");

            //return Console.ReadLine();
        }

        protected override string CreateSignature(IToken token, string verb, Uri uri, string verifier,
            bool renewToken = false, string callback = null)
        {
            return new HmacSha1Signer().CreateSignature(token, uri, verb, verifier, callback);
        }

        protected override IToken RenewToken(IToken sessionToken, IConsumer consumer)
        {
            return GetToken(consumer);
        }
    }
}
