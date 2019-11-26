// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.cs" company="Evertec Colombia">
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
    /// Implementa las pruebas unitarias para acceder a las operaciones de una aplicaci�n con alcance de aut�noma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests : AppBaseTests
    {
        /// <summary>
        /// Los procesos que fueron iniciados por cada servicio de prueba.
        /// </summary>
        private readonly DummyServices dummyServices = null;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AutonomousAppTests" />.
        /// </summary>
        public AutonomousAppTests()
        {
            this.dummyServices = new DummyServices().StartBifrostService();
        }

        /// <summary>
        /// Finaliza una instancia de la clase <see cref="AutonomousAppTests" />.
        /// </summary>
        ~AutonomousAppTests() => this.dummyServices.Dispose();

        /// <summary>
        /// Proporciona un conjunto com�n de funciones que se ejecutar�n antes de llamar al conjunto de pruebas.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "Bifrost:ConnectionStringName", "RabbitMQ:Local");
            SqlDataContext.SetAppSettingsKey(appIdentity.ApiKey, "DataProvider:SubsystemEnabled", "TUP");
        }
    }
}