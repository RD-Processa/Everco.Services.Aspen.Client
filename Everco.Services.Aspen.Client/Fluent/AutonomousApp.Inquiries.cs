﻿// -----------------------------------------------------------------------
// <copyright file="AutonomousApp.Inquiries.cs" company="Evertec Colombia">
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
    public sealed partial class AutonomousApp : IInquiriesModule
    {
        /// <summary>
        /// Obtiene un objeto que permite acceder a las consultas soportadas de los productos financieros de un usuario.
        /// </summary>
        public IInquiriesModule Inquiries => this;

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del usuario.</param>
        /// <param name="docNumber">El número de documento del usuario.</param>
        /// <returns>
        /// Lista de tipo <see cref="IAccountInfo" /> con la información de las cuentas del usuario especificado.
        /// </returns>
        public IList<AccountInfo> GetAccounts(string docType, string docNumber)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddDocType(docType)
                .AddDocNumber(docNumber);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.AccountsByUserIdentity, endpointParameters);
            return this.Execute<List<AccountInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información resumida de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <returns>
        /// Lista de tipo <see cref="IAccountInfo" /> con la información de las cuentas del usuario especificado.
        /// </returns>
        public IList<AccountInfo> GetAccountsByAlias(string channelId, string enrollmentAlias)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddChannelId(channelId)
                .AddEnrollmentAlias(enrollmentAlias);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.AccountsByAlias, endpointParameters);
            return this.Execute<List<AccountInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información de saldos de las cuentas asociadas a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de tipo <see cref="IBalanceInfo" /> con la información de saldos de las cuentas del usuario especificado.
        /// </returns>
        public IList<BalanceInfo> GetBalances(string docType, string docNumber, string accountId)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddDocType(docType)
                .AddDocNumber(docNumber)
                .AddAccountId(accountId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.BalancesByUserIdentity, endpointParameters);
            return this.Execute<List<BalanceInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información de saldos de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <returns>
        /// Lista de tipo <see cref="IBalanceInfo" /> con la información de saldos de las cuentas del usuario especificado.
        /// </returns>
        public IEnumerable<IBalanceInfo> GetBalancesByAlias(string channelId, string enrollmentAlias, string accountId)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddChannelId(channelId)
                .AddEnrollmentAlias(enrollmentAlias)
                .AddAccountId(accountId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.BalancesByAlias, endpointParameters);
            return this.Execute<List<BalanceInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información de los movimientos financieros de una cuenta asociada a un usuario.
        /// </summary>
        /// <param name="docType">El tipo de documento del propietario de la cuenta.</param>
        /// <param name="docNumber">El número de documento del propietario de la cuenta.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los movimientos financieros.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Lista de tipo <see cref="IMiniStatementInfo" /> con la información de los movimientos financieros de la cuenta especificada para el usuario.
        /// </returns>
        public IList<MiniStatementInfo> GetStatements(
            string docType,
            string docNumber,
            string accountId,
            string accountTypeId = null)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddDocType(docType)
                .AddDocNumber(docNumber)
                .AddAccountId(accountId)
                .AddAccountTypeId(string.IsNullOrWhiteSpace(accountTypeId) ? "*" : accountTypeId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.StatementsByUserIdentity, endpointParameters);
            return this.Execute<List<MiniStatementInfo>>(request);
        }

        /// <summary>
        /// Obtiene la información de movimientos de las cuentas asociadas a un usuario a partir de su alias utilizado en el registro.
        /// </summary>
        /// <param name="channelId">El identificador del canal por el que se registró el usuario.</param>
        /// <param name="enrollmentAlias">El alias utilizado en el proceso de registro del usuario.</param>
        /// <param name="accountId">El identificador de la cuenta para la que se obtienen los saldos.</param>
        /// <param name="accountTypeId">El identificador del tipo de cuenta (bolsillo) que se desea filtrar o <see langword="null" /> para omitir el filtro.</param>
        /// <returns>
        /// Listado con la información de movimientos financieros de la cuenta especificada para el usuario actual.
        /// </returns>
        public IEnumerable<IMiniStatementInfo> GetStatementsByAlias(
            string channelId,
            string enrollmentAlias,
            string accountId,
            string accountTypeId = null)
        {
            EndpointParameters endpointParameters = new EndpointParameters()
                .AddChannelId(channelId)
                .AddEnrollmentAlias(enrollmentAlias)
                .AddAccountId(accountId)
                .AddAccountTypeId(string.IsNullOrWhiteSpace(accountTypeId) ? "*" : accountTypeId);
            IRestRequest request = new AspenRequest(Scope.Autonomous, EndpointMapping.StatementsByAlias, endpointParameters);
            return this.Execute<List<MiniStatementInfo>>(request);
        }
    }
}