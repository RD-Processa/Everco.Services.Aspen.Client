﻿// -----------------------------------------------------------------------
// <copyright file="MissingEpochPayloadHeader.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 11:48 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using System.Collections.Generic;
    using Auth;
    using JWT;
    using Providers;
    using RestSharp;

    /// <summary>
    /// Implementa un manejador que no establece las cabeceras personalizadas esperadas por el servicio de Aspen.
    /// </summary>
    internal class MissingEpochPayloadHeader : DefaultHeadersManager
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingHeadersManager"/> class.
        /// </summary>
        public MissingEpochPayloadHeader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingHeadersManager"/> class.
        /// </summary>
        /// <param name="headerValueBehavior">The header value behavior.</param>
        public MissingEpochPayloadHeader(HeaderValueBehavior headerValueBehavior)
        {
            this.HeaderValueBehavior = headerValueBehavior;
        }

        /// <summary>
        /// Gets the header value behavior.
        /// </summary>
        private HeaderValueBehavior HeaderValueBehavior { get; }

        /// <summary>
        /// Agrega la cabecera que identifica la aplicación solicitante.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiKey">ApiKey de la aplicación para inclucir en la cabecera.</param>
        public override void AddApiKeyHeader(
            IRestRequest request, 
            string apiKey)
        {
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.ApiKeyHeaderName, apiKey);
        }

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        public override void AddSigninPayloadHeader(
            IRestRequest request, 
            IJwtEncoder jwtEncoder, 
            string apiSecret)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { ServiceLocator.Instance.NonceGenerator.Name, ServiceLocator.Instance.NonceGenerator.GetNonce() }
            };

            this.AddFields(request, jwtEncoder, payload, apiSecret);
        }

        public override void AddSignedPayloadHeader(
            IRestRequest request, 
            IJwtEncoder jwtEncoder, 
            string apiSecret, 
            string token)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { ServiceLocator.Instance.NonceGenerator.Name, ServiceLocator.Instance.NonceGenerator.GetNonce() },
                { "Token", token }
            };

            this.AddFields(request, jwtEncoder, payload, apiSecret);
        }

        public override void AddSigninPayloadHeader(
            IRestRequest request, 
            IJwtEncoder jwtEncoder, 
            string apiSecret, 
            IUserIdentity userIdentity)
        {
            IDeviceInfo deviceInfo = userIdentity.DeviceInfo ?? new DeviceInfo();
            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.DeviceInfoHeaderName, deviceInfo.ToJson());

            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { ServiceLocator.Instance.NonceGenerator.Name, ServiceLocator.Instance.NonceGenerator.GetNonce() },
                { "DocType", userIdentity.DocType },
                { "DocNumber", userIdentity.DocNumber },
                { "Password", userIdentity.Password },
                { "DeviceId", deviceInfo.DeviceId }
            };

            this.AddFields(request, jwtEncoder, payload, apiSecret);
        }

        private void AddFields(IRestRequest request,
                               IJwtEncoder jwtEncoder,
                               IDictionary<string, object> payload,
                               string apiSecret)
        {
            string claimName = ServiceLocator.Instance.EpochGenerator.Name;
            switch (this.HeaderValueBehavior)
            {
                case HeaderValueBehavior.Null:
                    payload.Add(claimName, null);
                    break;

                case HeaderValueBehavior.Empty:
                    payload.Add(claimName, string.Empty);
                    break;

                case HeaderValueBehavior.WhiteSpaces:
                    payload.Add(claimName, "    ");
                    break;

                case HeaderValueBehavior.UnexpectedFormat:
                    payload.Add(claimName, "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE");
                    break;

                case HeaderValueBehavior.MaxLengthExceeded:
                    payload.Add(claimName, $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}");
                    break;

                case HeaderValueBehavior.Missing:
                    break;

                case HeaderValueBehavior.MinLengthRequired:
                    payload.Add(claimName, "x");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            request.AddHeader(ServiceLocator.Instance.RequestHeaderNames.PayloadHeaderName, jwtEncoder.Encode(payload, apiSecret));
        }
    }
}
