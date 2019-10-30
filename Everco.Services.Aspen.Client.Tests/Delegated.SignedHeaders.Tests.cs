// -----------------------------------------------------------------------
// <copyright file="Delegated.SignedHeaders.Tests.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Everco.Services.Aspen.Entities;
    using Fluent;
    using Identities;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// Implementa las pruebas unitarias de las cabeceras de autenticaci�n requeridas por una aplicaci�n con alcance de delegada.
    /// </summary>
    [TestFixture]
    public class DelegatedSignedHeadersTests
    {
        /// <summary>
        /// Proporciona un conjunto com�n de funciones que se ejecutar�n antes de llamar a cada m�todo de prueba.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
        }

        [Test]
        [Category("Delegated.Signed.Headers")]
        public void SignedRequestWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();

            // Se usa una operaci�n que requiere token de autenticaci�n.
            IList<DocTypeInfo> docTypes = client.Settings.GetDocTypes();
            CollectionAssert.IsNotEmpty(docTypes);
        }

        [Test]
        [Category("Delegated.Signed.Headers")]
        public void NonceAlreadyProcessedThrows()
        {
            string nonce = Guid.NewGuid().ToString("D");
            ServiceLocator.Instance.RegisterNonceGenerator(new DuplicatedNonceGenerator(nonce));
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
                .GetClient();

            // Se usa una operaci�n luego de la autenticaci�n con el mismo nonce y debe fallar ya que se est� reutilizando.
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Nonce ya procesado para su aplicaci�n", exception.Message);
        }

        [Test]
        [Category("Delegated.Signed.Headers")]
        public void MissingAuthTokenThrows()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
                .GetClient();

            // Se intenta usar una operaci�n que requiere el token de autenticaci�n.
            ServiceLocator.Instance.RegisterHeadersManager(new MissingAuthTokenClaimOnPayloadHeader());
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Token' no puede ser nulo ni vac�o", exception.Message);
        }

        [Test]
        [Category("Delegated.Signed.Headers")]
        public void NullOrEmptyAuthTokenThrows()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
                .GetClient();

            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new MissingAuthTokenClaimOnPayloadHeader(HeaderValueBehavior.Null),
                new MissingAuthTokenClaimOnPayloadHeader(HeaderValueBehavior.Empty),
                new MissingAuthTokenClaimOnPayloadHeader(HeaderValueBehavior.WhiteSpaces)
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                // Se intenta usar una operaci�n que requiere el token de autenticaci�n.
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Token' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signed.Headers")]
        public void InvalidAuthTokenFormatThrows()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
                .GetClient();

            // Se intenta usar una operaci�n que requiere el token de autenticaci�n.
            ServiceLocator.Instance.RegisterHeadersManager(new MissingAuthTokenClaimOnPayloadHeader(HeaderValueBehavior.UnexpectedFormat));
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es v�lido", exception.Message);
        }
    }
}