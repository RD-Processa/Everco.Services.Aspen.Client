﻿// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.Settings.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Fluent
{
    using System.Collections.Generic;
    using Entities;
    using Internals;
    using Modules.Autonomous;
    using RestSharp;

    /// <summary>
    /// Expone operaciones que permite conectar con el servicio Aspen para aplicaciones con alcance autónomo.
    /// </summary>
    public sealed partial class AutonomousApp : ISettingsModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a la información relacionada con la parametrización del sistema Aspen.
        /// </summary>
        public ISettingsModule Settings => this;

        /// <summary>
        /// Obtiene la lista de operadores de telefonía móvil soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Lista de operadores de telefonía soportados.
        /// </returns>
        public IList<CarrierInfo> GetCarriers()
        {
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.Carriers);
            return this.Execute<List<CarrierInfo>>(request);
        }

        /// <summary>
        /// Obtiene la lista de tipos de documento soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Lista de tipos de documento soportados.
        /// </returns>
        public IList<DocTypeInfo> GetDocTypes()
        {
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.DocTypes);
            return this.Execute<List<DocTypeInfo>>(request);
        }

        /// <summary>
        /// Obtiene los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Lista de <see cref="PaymentTypeInfo" /> con los tipos de pago para la aplicación solicitante.
        /// </returns>
        public IList<PaymentTypeInfo> GetPaymentTypes()
        {
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.PaymentTypes);
            return this.Execute<List<PaymentTypeInfo>>(request);
        }

        /// <summary>
        /// Obtiene los valores admitidos de recarga por operador soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Lista de <see cref="TopUpInfo" /> con los valores admitidos de recarga por operador para la aplicación solicitante.
        /// </returns>
        public IList<TopUpInfo> GetTopUpValues()
        {
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.TopUp);
            return this.Execute<List<TopUpInfo>>(request);
        }

        /// <summary>
        /// Obtiene la lista de los tipos de transacción soportados para la aplicación.
        /// </summary>
        /// <returns>
        /// Lista de tipos de transacción soportados.
        /// </returns>
        public IList<TranTypeInfo> GetTranTypes()
        {
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.TranTypes);
            return this.Execute<List<TranTypeInfo>>(request);
        }
    }
}
