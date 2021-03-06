﻿// -----------------------------------------------------------------------
// <copyright file="IHeadersManager.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-19 07:44 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using Identity;
    using JWT;
    using RestSharp;

    /// <summary>
    /// Define las operaciones necesarias para establecer las cabeceras personalizadas requeridas por el servicio Aspen.
    /// </summary>
    public interface IHeadersManager
    {
        /// <summary>
        /// Agrega la cabecera que identifica a la aplicación solicitante.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiKey">ApiKey de la aplicación para incluir en la cabecera.</param>
        void AddApiKeyHeader(
            IRestRequest request,
            string apiKey);

        /// <summary>
        /// Agrega la cabecera que identifica el número de versión del API solicitada.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="apiVersion">Número de versión del API para incluir en la cabecera.</param>
        void AddApiVersionHeader(
            IRestRequest request,
            string apiVersion);

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para firmar una solicitud de una aplicación a partir del token de autenticación.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        /// <param name="token">El token de autenticación emitido para la aplicación.</param>
        void AddSignedPayloadHeader(
            IRestRequest request,
            IJwtEncoder jwtEncoder,
            string apiSecret,
            string token);

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para autenticar a un usuario en el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        /// <param name="token">El token de autenticación emitido para el usuario.</param>
        /// <param name="username">La identificación del usuario autenticado.</param>
        /// <param name="device">La información asociada con el dispositivo del usuario.</param>
        void AddSignedPayloadHeader(
            IRestRequest request,
            IJwtEncoder jwtEncoder,
            string apiSecret,
            string token,
            string username,
            IDeviceInfo device = null);

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para autenticar a la aplicación en el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        void AddSigninPayloadHeader(
            IRestRequest request,
            IJwtEncoder jwtEncoder,
            string apiSecret);

        /// <summary>
        /// Agrega la cabecera con los datos de la carga útil necesarios para autenticar a un usuario en el servicio Aspen.
        /// </summary>
        /// <param name="request">Solicitud a donde se agrega la cabecera.</param>
        /// <param name="jwtEncoder">Instancia del codificador del contenido de la carga útil.</param>
        /// <param name="apiSecret">Secreto de la aplicación que se utiliza para codificar el contenido del carga útil.</param>
        /// <param name="userIdentity">La información que se utiliza para autenticar la solicitud en función de un usuario.</param>
        void AddSigninPayloadHeader(
            IRestRequest request,
            IJwtEncoder jwtEncoder,
            string apiSecret,
            IUserIdentity userIdentity);
    }
}