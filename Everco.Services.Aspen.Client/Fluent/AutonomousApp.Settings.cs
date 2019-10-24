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
        /// Obtiene la lista de operadores de telefonía móvil soportados por el servicio Aspen.
        /// </summary>
        /// <returns>
        /// Lista de operadores de telefonía soportados.
        /// </returns>
        public IList<CarrierInfo> GetCarriers()
        {
            string resource = $"{Routes.AutonomousRoot}{Routes.Resources.Telcos}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<CarrierInfo>>(request);
        }

        /// <summary>
        /// Obtiene la lista de tipos de documento soportados por el servicio Aspen.
        /// </summary>
        /// <returns>
        /// Lista de tipos de documento soportados.
        /// </returns>
        public IList<DocTypeInfo> GetDocTypes()
        {
            string resource = $"{Routes.AutonomousRoot}{Routes.Resources.DocTypes}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<DocTypeInfo>>(request);
        }

        /// <summary>
        /// Obtiene los tipos de pagos que se pueden realizar a una cuenta.
        /// </summary>
        /// <returns>
        /// Lista de <see cref="PaymentTypeInfo" /> con los tipos de pago para la aplicación solicitante.
        /// </returns>
        public IList<PaymentTypeInfo> GetPaymentTypes()
        {
            string resource = $"{Routes.AutonomousRoot}{Routes.Resources.PaymentTypes}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<PaymentTypeInfo>>(request);
        }

        /// <summary>
        /// Obtiene los tipos de pagos que se pueden realizar a una cuenta.
        /// </summary>
        /// <returns>
        /// Lista de <see cref="TopUpInfo" /> con los valores admitidos de recarga por operador para la aplicación solicitante.
        /// </returns>
        public IList<TopUpInfo> GetTopUpValues()
        {
            string resource = $"{Routes.AutonomousRoot}{Routes.Resources.TopUp}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<TopUpInfo>>(request);
        }

        /// <summary>
        /// Obtiene la lista de los tipos de transacción para una aplicación.
        /// </summary>
        /// <returns>
        /// Lista de tipos de transacción soportados.
        /// </returns>
        public IList<TranTypeInfo> GetTranTypes()
        {
            string resource = $"{Routes.AutonomousRoot}{Routes.Resources.TranTypes}";
            IRestRequest request = new AspenRequest(resource, Method.GET);
            return this.Execute<List<TranTypeInfo>>(request);
        }
    }
}