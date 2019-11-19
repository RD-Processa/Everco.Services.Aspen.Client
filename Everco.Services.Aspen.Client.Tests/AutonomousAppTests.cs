// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
namespace Everco.Services.Aspen.Client.Tests
{
    using Everco.Services.Aspen.Client.Fluent;
    using Everco.Services.Aspen.Client.Providers;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las operaciones de una aplicaci�n con alcance de aut�noma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Proporciona un conjunto com�n de funciones que se ejecutar�n antes de llamar a cada m�todo de prueba.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
        }

        /// <summary>
        /// Obtiene un cliente para la aplicaci�n aut�noma de pruebas a partir de la solicitud de generaci�n de un token de autenticaci�n.
        /// </summary>
        /// <returns>Instancia de <see cref="IAutonomousApp"/> para interactuar con el servicio.</returns>
        private static IAutonomousApp GetAutonomousClient() =>
            AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Master)
                .AuthenticateNoCache()
                .GetClient();
    }
}