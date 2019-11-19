﻿// -----------------------------------------------------------------------
// <copyright file="IRouting.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System;
    using Providers;

    /// <summary>
    /// Define las operaciones que permiten establecer la URL del servicio ASPEN y tiempo de espera para las respuestas.
    /// </summary>
    /// <typeparam name="TFluent">El tipo de aplicación de retorno.</typeparam>
    public interface IRouting<out TFluent>
    {
        /// <summary>
        /// Establece la URL para las solicitudes al servicio ASPEN.
        /// </summary>
        /// <param name="endpointProvider">Instancia con la configuración del servicio ASPEN.</param>
        /// <returns>Instancia de <see cref="IAppIdentity{TFluent}"/> que permite establecer los datos de conexión con el servicio.</returns>
        IAppIdentity<TFluent> RoutingTo(IEndpointProvider endpointProvider);

        /// <summary>
        /// Establece la URL para las solicitudes al servicio ASPEN.
        /// </summary>
        /// <param name="url">La URL para las solicitudes realizadas hacia al servicio ASPEN. Ejemplo: <a>http://localhost/api</a></param>
        /// <param name="timeout">El tiempo de espera (en segundos) para las respuesta de las solicitudes al servicio o <c>null</c> para establecer el valor predeterminado (15 segundos)</param>
        /// <returns>Instancia de <see cref="IAppIdentity{TFluent}"/> que permite establecer los datos de conexión con el servicio.</returns>
        IAppIdentity<TFluent> RoutingTo(string url, int? timeout = null);

        /// <summary>
        /// Establece la URL para las solicitudes al servicio ASPEN.
        /// </summary>
        /// <param name="url">La URL para las solicitudes hacia al API de ASPEN realizadas por esta instancia de cliente. Ejemplo: <a>http://localhost/api</a>.</param>
        /// <param name="timeout">El tiempo de espera para las respuesta de las solicitudes al servicio o <c>null</c> para establecer el valor predeterminado (15 segundos)</param>
        /// <returns>Instancia de <see cref="IAppIdentity{TFluent}"/> que permite establecer los datos de conexión con el servicio.</returns>
        IAppIdentity<TFluent> RoutingTo(Uri url, TimeSpan? timeout = null);
    }
}