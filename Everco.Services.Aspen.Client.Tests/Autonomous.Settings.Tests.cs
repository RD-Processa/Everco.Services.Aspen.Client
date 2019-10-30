// -----------------------------------------------------------------------
// <copyright file="Autonomous.Settings.Tests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using Everco.Services.Aspen.Entities;
    using Fluent;
    using Identities;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las entidades de informaci�n relacionadas con parametrizaci�n del sistema para una aplicaci�n de alcance de aut�noma. 
    /// </summary>
    [TestFixture]
    public class AutonomousSettingsTests
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
        /// Obtener los tipos de documento de la aplicaci�n funciona.
        /// </summary>
        [Test]
        [Category("Autonomous.Settings")]
        public void GetDocTypesWorks()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .AuthenticateNoCache()
                .GetClient();

            IList<DocTypeInfo> docTypes = client.Settings.GetDocTypes();
            CollectionAssert.IsNotEmpty(docTypes);
        }

        /// <summary>
        /// Obtener los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicaci�n funciona.
        /// </summary>
        [Test]
        [Category("Autonomous.Settings")]
        public void GetPaymentTypesWorks()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .AuthenticateNoCache()
                .GetClient();

            IList<PaymentTypeInfo> paymentTypes = client.Settings.GetPaymentTypes();
            CollectionAssert.IsNotEmpty(paymentTypes);
        }
    }
}