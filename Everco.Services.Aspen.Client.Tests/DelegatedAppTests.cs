// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using Everco.Services.Aspen.Client.Auth;
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones de una aplicaci�n con alcance de delegada.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests : AppBaseTests
    {
        /// <summary>
        /// Proporciona un conjunto com�n de instrucciones que se ejecutar�n una �nica vez, antes de llamar al conjunto de pruebas implementadas.
        /// </summary>
        public override void RunBeforeTestFixture()
        {
            base.RunBeforeTestFixture();
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Bifrost:Tests");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bancor:ConnectionStringName", "RabbitMQ:Bancor:Tests");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");
        }
    }
}